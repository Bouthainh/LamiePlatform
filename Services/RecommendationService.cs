using LamiePlatform.Data;
using LamiePlatform.DTOs.PlatformDTOs;
using LamiePlatform.Models;
using LamiePlatform.Models.ViewModels;
using LamiePlatform.Models.ViewModels.DashboardViewModels;
using LamiePlatform.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace LamiePlatform.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly LamieDbContext _db;
        private readonly IHttpClientFactory _httpFactory;
        private readonly string _openAiApiKey;

        //defining the ai model: GPT-4o-mini
        private const string GptModel = "gpt-4o-mini";
        // OpenAI API endpoint 
        private const string OpenAiEndpoint = "https://api.openai.com/v1/chat/completions";

        public RecommendationService(LamieDbContext db, IHttpClientFactory httpFactory, IConfiguration config)
        {
            _db = db;
            _httpFactory = httpFactory;
            _openAiApiKey = config["OpenAI:ApiKey"]
                ?? throw new InvalidOperationException("OpenAI:ApiKey not found in appsettings.json.");
        }

        //generate a recommendation for an individual child based on their intelligences and save it to the database
        public async Task<ServiceResult> GenerateAndSaveAsync(string childId, string? educatorId = null)
        {
            var child = await _db.Children
                .Include(c => c.ChildIntelligences)
                    .ThenInclude(ci => ci.Intelligence)
                .FirstOrDefaultAsync(c => c.ChildId == childId);

            if (child == null)
                return new ServiceResult(false, "الطفل غير موجود.");

            if (!child.ChildIntelligences.Any())
                return new ServiceResult(false, "لا توجد درجات ذكاء مسجلة لهذا الطفل.");

            var prompt = BuildPrompt(child);
            var gptResult = await CallGptAsync(prompt);

            if (gptResult == null)
                return new ServiceResult(false, "تعذّر الحصول على توصية من الذكاء الاصطناعي. حاول مرة أخرى.");

            var recommendation = new ActivityRecommendation
            {
                ChildId = childId,
                EducatorId = educatorId,
                ActivityName = gptResult.ActivityName ?? "",
                Category = gptResult.Category,
                ActivityDescription = gptResult.ActivityDescription,
                Duration = int.TryParse(gptResult.Duration, out var dur) ? dur : null
            };

            _db.ActivityRecommendations.Add(recommendation);
            await _db.SaveChangesAsync();

            return new ServiceResult(true, "تم توليد التوصية بنجاح.", data: recommendation.RecommendationId.ToString());
        }

        //generate recommendation for a group based on averaged intelligences and save it to the database
        public async Task<ServiceResult> GenerateAndSaveForGroupAsync(Guid groupId, string educatorId)
        {
            var group = await _db.ChildGroups
                .Include(g => g.Children)
                    .ThenInclude(c => c.ChildIntelligences)
                        .ThenInclude(ci => ci.Intelligence)
                .Include(g => g.Class)
                      .ThenInclude(c => c.Grade)
                        .FirstOrDefaultAsync(g => g.ChildGroupId == groupId);

            if (group == null)
                return new ServiceResult(false, "المجموعة غير موجودة.");

            if (!group.Children.Any())
                return new ServiceResult(false, "لا يوجد أطفال في هذه المجموعة.");

            var prompt = BuildPrompt(group);
            var gptResult = await CallGptAsync(prompt);

            if (gptResult == null)
                return new ServiceResult(false, "تعذّر الحصول على توصية من الذكاء الاصطناعي. حاول مرة أخرى.");

            var recommendation = new ActivityRecommendation
            {
                GroupId = groupId,
                ClassId = group.ClassId,
                EducatorId = educatorId,
                ActivityName = gptResult.ActivityName ?? "",
                Category = gptResult.Category,
                ActivityDescription = gptResult.ActivityDescription,
                Duration = int.TryParse(gptResult.Duration, out var dur) ? dur : null
            };

            _db.ActivityRecommendations.Add(recommendation);
            await _db.SaveChangesAsync();

            return new ServiceResult(true, "تم توليد التوصية بنجاح.", data: recommendation.RecommendationId.ToString());
        }



        // Parent prompt >> individual child context
        private string BuildPrompt(Child child)
        {
            var sb = new StringBuilder();

            sb.AppendLine("أنت متخصص في تطوير قدرات وذكاءات الأطفال.");
            sb.AppendLine($"عمر الطفل: {child.Age} سنوات.");
            sb.AppendLine();
            sb.AppendLine("فيما يلي درجات الطفل في نظرية الذكاءات المتعددة لهوارد غاردنر (من 100):");

            foreach (var ci in child.ChildIntelligences)
                sb.AppendLine($"  {ci.Intelligence?.IntelligenceName}: {ci.ProficiencyScore}");

            sb.AppendLine();
            //control instruction to ensure homefriendly activities for parents
            sb.AppendLine("تأكد أن النشاط المقترح مناسب للتطبيق في المنزل، ولا يتطلب معدات متخصصة أو بيئة مدرسية"); 
            AppendSharedInstructions(sb);

            return sb.ToString();
        }

        // Educator prompt >> group context with averaged intelligences
         private string BuildPrompt(ChildGroup group)
        {
            var grade = group.Class?.Grade?.GradeName ?? "غير محدد";
            var memberDominants = ExtractMemberDominantIntelligences(group);
 
            var sb = new StringBuilder();
            //set the context for the educator
            sb.AppendLine("أنت متخصص في تطوير قدرات الأطفال داخل بيئة تعليمية.");
            sb.AppendLine($"الصف الدراسي: {grade}.");
            sb.AppendLine($"حجم المجموعة: {memberDominants.Count} أطفال.");
            sb.AppendLine();
            sb.AppendLine("تم تشكيل هذه المجموعة بحيث يتميز كل طفل بذكاء مختلف، وفيما يلي الذكاء الأبرز لكل عضو:");
 
            int memberNumber = 1;
            foreach (var (childName, dominantIntelligence) in memberDominants)
                sb.AppendLine($"  الطفل {memberNumber++} ({childName}): الذكاء الأبرز هو {dominantIntelligence}");

            //control instruction to ensure classroomfriendly activities for educators
            sb.AppendLine();
            sb.AppendLine("المطلوب: اقترح نشاطاً جماعياً واحداً داخل الفصل يستثمر الذكاء الأبرز لكل عضو في المجموعة،");
            sb.AppendLine("بحيث يُسهم كل طفل من موضع قوته ويكمل الآخرين، مما يُنمّي مهاراتهم جميعاً معاً.");
            sb.AppendLine("تأكد أن النشاط قابل للتطبيق الجماعي داخل الفصل الدراسي.");
            AppendSharedInstructions(sb);
 
            return sb.ToString();
        }
 

        // Shared instructions for both prompts 
        private static void AppendSharedInstructions(StringBuilder sb)
        {
            sb.AppendLine("المهام المطلوبة:");
            sb.AppendLine("1.  حدّد أعلى نوع ذكاء لدى الطفل/المجموعة بناءً على الدرجات");
            sb.AppendLine("2. اقترح نشاطاً واحداً محدداً يساعد على تنمية وتطوير هذا الذكاء بشكل عملي");
            sb.AppendLine("3. اكتب وصفاً للنشاط من 3 إلى 4 أسطر كحد أقصى، يكون واضحاً ومباشراً");
            sb.AppendLine("4. حدّد المدة الموصى بها للنشاط (بالدقائق، رقم فقط)");
            sb.AppendLine("5. صنّف النشاط تحت إحدى الفئات التالية فقط:");
            sb.AppendLine("   الذكاء اللغوي | الذكاء المنطقي | الذكاء المكاني | الذكاء الجسدي");
            sb.AppendLine("   الذكاء الموسيقي | الذكاء الاجتماعي | الذكاء الذاتي | الذكاء الطبيعي");
            sb.AppendLine();
            sb.AppendLine("أجب فقط بكائن JSON صالح، بدون markdown، بدون أي نص إضافي، باللغة العربية:");
            sb.AppendLine("{");
            sb.AppendLine("  \"activity_name\": \"...\",");
            sb.AppendLine("  \"description\": \"...\",");
            sb.AppendLine("  \"duration\": \"...\",");
            sb.AppendLine("  \"category\": \"...\"");
            sb.AppendLine("}");
        }

        // Extract each child's single dominant intelligence (name + score) from the group
        private static List<(string ChildName, string DominantIntelligence)> ExtractMemberDominantIntelligences(ChildGroup group)
        {
            return group.Children
                .Select(c =>
                {
                    var top = c.ChildIntelligences
                        .OrderByDescending(ci => ci.ProficiencyScore)
                        .FirstOrDefault();

                    var intelligenceName = top?.Intelligence?.IntelligenceName ?? "غير محدد";
                    var childName = c.ChildName ?? $"طفل";

                    return (childName, intelligenceName);
                })
                .ToList();
        }

        //call the GPT API
        private async Task<GptRecommendationResult?> CallGptAsync(string userPrompt)
        {
            try
            {
                var http = _httpFactory.CreateClient("OpenAI");

                var bodyObj = new
                {
                    model = GptModel,
                    max_tokens = 600,
                    temperature = 0.7,
                    messages = new[]
                    {
                        new { role = "system", content = "أنت متخصص في تطوير قدرات الأطفال. أجب دائماً باللغة العربية وبصيغة JSON صالحة فقط، بدون markdown." },
                        new { role = "user",   content = userPrompt }
                    }
                };

                using var content = new StringContent(
                    JsonSerializer.Serialize(bodyObj), Encoding.UTF8, "application/json");

                var response = await http.PostAsync(OpenAiEndpoint, content);
                var raw = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GPT API error {(int)response.StatusCode}: {raw}");
                    return null;
                }

                var envelope = JsonNode.Parse(raw);
                var text = envelope?["choices"]?[0]?["message"]?["content"]
                                   ?.GetValue<string>()?.Trim();

                if (string.IsNullOrWhiteSpace(text)) return null;

                text = text.Replace("```json", "").Replace("```", "").Trim();

                var parsed = JsonNode.Parse(text);
                return new GptRecommendationResult
                {
                    ActivityName = parsed?["activity_name"]?.GetValue<string>(),
                    ActivityDescription = parsed?["description"]?.GetValue<string>(),
                    Duration = parsed?["duration"]?.ToString(),
                    Category = parsed?["category"]?.GetValue<string>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GPT call failed: {ex.Message}");
                return null;
            }
        }

        //retrieve all recommendations
        public async Task<List<RecommendationListItem>> GetRecommendationsByChildIdAsync(string childId)
        {
            return await _db.ActivityRecommendations
                .Where(r => r.ChildId == childId)
                .OrderByDescending(r => r.RecommendationId)
                .Select(r => new RecommendationListItem
                {
                    RecommendationId = r.RecommendationId,
                    ActivityName = r.ActivityName,
                    Category = r.Category,
                })
                .ToListAsync();
        }

        //retrieve recommendation details by rec id
        public async Task<RecommendationDetailViewModel?> GetRecommendationDetailAsync(Guid recommendationId)
        {
            var rec = await _db.ActivityRecommendations
                .Include(r => r.Child)
                .FirstOrDefaultAsync(r => r.RecommendationId == recommendationId);

            if (rec == null) return null;

            return new RecommendationDetailViewModel
            {
                RecommendationId = rec.RecommendationId,
                ChildId = rec.ChildId,
                ChildName = rec.Child?.ChildName ?? "",
                ActivityName = rec.ActivityName,
                Category = rec.Category,
                ActivityDescription = rec.ActivityDescription ?? "",
                Duration = rec.Duration
            };
        }

        // List all recommendations associated to an educator
        public async Task<List<RecommendationListItem>> GetRecommendationsByEducatorIdAsync(string educatorId)
        {
            return await _db.ActivityRecommendations
                .Where(r => r.EducatorId == educatorId)
                .OrderByDescending(r => r.RecommendationId)
                .Select(r => new RecommendationListItem
                {
                    RecommendationId = r.RecommendationId,
                    ActivityName = r.ActivityName,
                    Category = r.Category,
                })
                .ToListAsync();
        }

        // Detail for a group recommendation
        public async Task<RecommendationDetailViewModel?> GetGroupRecommendationDetailAsync(Guid recommendationId)
        {
            var rec = await _db.ActivityRecommendations
                .Include(r => r.Group)
                .FirstOrDefaultAsync(r => r.RecommendationId == recommendationId);

            if (rec == null) return null;

            return new RecommendationDetailViewModel
            {
                RecommendationId = rec.RecommendationId,
                GroupId = rec.GroupId,
                GroupName = rec.Group?.GroupName ?? "",
                ActivityName = rec.ActivityName,
                Category = rec.Category,
                ActivityDescription = rec.ActivityDescription ?? "",
                Duration = rec.Duration
            };
        }

        public async Task<List<RecommendationListItem>> GetRecommendationsByGroupIdAsync(Guid groupId)
        {
            return await _db.ActivityRecommendations
                .Where(r => r.GroupId == groupId)
                .OrderByDescending(r => r.RecommendationId)
                .Select(r => new RecommendationListItem
                {
                    RecommendationId = r.RecommendationId,
                    ActivityName = r.ActivityName,
                    Category = r.Category,
                })
                .ToListAsync();
        }

        public async Task<GptReportOverviewResult?> GenerateReportOverviewAsync(ChildDashboardViewModel data, string viewerRole)
        {
            var prompt = BuildReportOverviewPrompt(data, viewerRole);
            return await CallGptReportOverviewAsync(prompt, viewerRole);
        }

        private string BuildReportOverviewPrompt(ChildDashboardViewModel data, string viewerRole)
        {
            var sb = new StringBuilder();

            sb.AppendLine("أنت خبير في تحليل أداء الأطفال وفق نظرية الذكاءات المتعددة، وهدفك تقديم تحليل عميق ومفيد، وليس مجرد وصف للبيانات");
            sb.AppendLine($"اسم الطفل: {data.ChildName}، العمر: {data.Age} سنوات.");
            sb.AppendLine();

            sb.AppendLine("درجات الذكاءات المتعددة (من 100):");
            foreach (var intel in data.AllIntelligences)
                sb.AppendLine($"  {intel.Label}: {intel.Value}");

            sb.AppendLine();
            sb.AppendLine($"أبرز ذكاء: {data.TopIntelligence?.IntelligenceName} — الدرجة: {data.TopIntelligence?.Score}");

            sb.AppendLine();
            sb.AppendLine("تفاصيل الجوانب والمؤشرات لكل ذكاء:");
            foreach (var intel in data.IntelligenceDetails)
            {
                sb.AppendLine($"  [{intel.IntelligenceName}] — الدرجة الكلية: {intel.ProficiencyScore}");
                foreach (var aspect in intel.Aspects)
                {
                    sb.AppendLine($"    الجانب: {aspect.AspectName} ({Math.Round(aspect.AspectScore * 100, 0)}%) — {aspect.AspectRating}");
                    foreach (var ind in aspect.Indicators)
                        sb.AppendLine($"      - {ind.IndicatorName}: {Math.Round(ind.IndicatorScore * 100, 0)}% ({ind.IndicatorRating})");
                }
            }

            sb.AppendLine();
            sb.AppendLine("آخر الجلسات:");
            foreach (var s in data.RecentSessions)
                sb.AppendLine($"  {s.PlayedAt:yyyy/MM/dd} | {s.LevelName} | {s.IntelligenceName} | {Math.Round(s.AspectScore * 100, 0)}% | {Math.Round(s.TotalTimeSec, 0)}ث");

            sb.AppendLine();
            sb.AppendLine("المطلوب منك التحليل كالتالي:");

            sb.AppendLine("1) نظرة عامة: تحليل عام لأداء الطفل (ليس مجرد إعادة أرقام، بل ماذا تعني).");
            sb.AppendLine("2) تحليل أعلى ذكاء:");
            sb.AppendLine("   - ما الذي يميز هذا الذكاء لدى الطفل؟");
            sb.AppendLine("   - أقوى جانب داخله وأضعف جانب.");
            sb.AppendLine("   - ماذا يعني هذا عملياً لسلوك الطفل أو تعلمه؟");

            sb.AppendLine("3) تحليل أضعف ذكاء:");
            sb.AppendLine("   - هل يعتبر ضعفاً يحتاج تدخل أم ضمن الطبيعي؟");
            sb.AppendLine("   - متى يجب القلق أو التركيز عليه؟");

            sb.AppendLine("4) التوصيات:");
            sb.AppendLine("   - كيف ننمّي الذكاء الأعلى بشكل عملي؟");
            sb.AppendLine("   - كيف ندعم الذكاء الأضعف بدون ضغط على الطفل؟");
            sb.AppendLine("   - توصيات عامة متوازنة.");

            sb.AppendLine();

            if (viewerRole == "Parent")
            {
                sb.AppendLine("الجمهور: ولي أمر.");
                sb.AppendLine("الأسلوب: بسيط، دافئ، مشجع، بدون مصطلحات معقدة.");
                sb.AppendLine("ركّز على التفسير والمعنى أكثر من الأرقام.");
                sb.AppendLine("اكتب parent_summary فقط.");
            }
            else
            {
                sb.AppendLine("الجمهور: معلم/مختص.");
                sb.AppendLine("الأسلوب: تحليلي وتربوي مهني.");
                sb.AppendLine("اكتب parent_summary و educator_summary.");
            }

            sb.AppendLine();
            sb.AppendLine("أجب فقط بصيغة JSON صالحة، بدون markdown:");

            sb.AppendLine("{");
            sb.AppendLine("  \"parent_summary\": \"ملخص تفسيري شامل (5-6 جمل) يشرح الأداء العام، معنى أعلى ذكاء، وأضعف ذكاء بطريقة مفهومة\",");
            sb.AppendLine("  \"educator_summary\": \"تحليل مهني أعمق يربط بين الجوانب والمؤشرات ويشرح دلالاتها التربوية، أو null إذا Parent\",");
            sb.AppendLine("  \"recommendations\": \"توصيات عملية متوازنة: تطوير القوة + دعم الضعف (3-5 نقاط، كل نقطة سطر مستقل)\"");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private async Task<GptReportOverviewResult?> CallGptReportOverviewAsync(string userPrompt, string viewerRole)
        {
            try
            {
                var http = _httpFactory.CreateClient("OpenAI");

                var bodyObj = new
                {
                    model = GptModel,
                    max_tokens = 1000,
                    temperature = 0.6,
                    messages = new[]
                    {
                new { role = "system", content = "أنت متخصص في تقييم ذكاءات الأطفال. أجب دائماً باللغة العربية وبصيغة JSON صالحة فقط، بدون markdown." },
                new { role = "user", content = userPrompt }
            }
                };

                using var content = new StringContent(
                    JsonSerializer.Serialize(bodyObj), Encoding.UTF8, "application/json");

                var response = await http.PostAsync(OpenAiEndpoint, content);
                var raw = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"GPT Report Overview error {(int)response.StatusCode}: {raw}");
                    return null;
                }

                var envelope = JsonNode.Parse(raw);
                var text = envelope?["choices"]?[0]?["message"]?["content"]
                                   ?.GetValue<string>()?.Trim();

                if (string.IsNullOrWhiteSpace(text)) return null;

                text = text.Replace("```json", "").Replace("```", "").Trim();

                var parsed = JsonNode.Parse(text);
                return new GptReportOverviewResult
                {
                    ParentSummary = parsed?["parent_summary"]?.GetValue<string>(),
                    EducatorSummary = parsed?["educator_summary"]?.GetValue<string>(),
                    Recommendations = parsed?["recommendations"]?.GetValue<string>()
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GPT Report Overview call failed: {ex.Message}");
                return null;
            }
        }
    }
}
