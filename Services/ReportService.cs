using LamiePlatform.Models.ViewModels.DashboardViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LamiePlatform.Services
{
    public class ReportService : IReportService
    {
        private readonly IDashboardService _dashboardService;
        private readonly IRecommendationService _recommendationService; 
        private readonly IWebHostEnvironment _env;

        private const string AccentYellow = "#FFB90F";
        private const string TopIntelBg = "#e9fbfa";
        private const string TopIntelBorder = "#32C8C3";

        private static readonly Dictionary<string, string> IntelColors = new()
        {
            { "الذكاء اللغوي",    "#FF3C4B" },
            { "الذكاء المنطقي",   "#FF96AA" },
            { "الذكاء البصري",    "#A03CC8" },
            { "الذكاء الجسدي",    "#32C8C3" },
            { "الذكاء الموسيقي",  "#FFB90F" },
            { "الذكاء الاجتماعي", "#FF622E" },
            { "الذكاء الذاتي",    "#A4C83C" },
            { "الذكاء الطبيعي",   "#6B9DC0" },
        };


        private static readonly Dictionary<string, string> SectionIcons = new()
        {
            { "أبرز ذكاء",                  "🏅" },
            { "جميع الذكاءات",              "📊" },
            { "مستويات الذكاء",             "📋" },
            { "الجهد مقابل الدرجة",         "⏱" },
            { "مؤشرات الأداء التفصيلية",    "🔍" },
            { "آخر الجلسات",                "📅" },
            { "منحنى الأداء عبر الزمن",     "📈" },
            { "الأنشطة المقترحة",           "🎯" },
        };

        public ReportService(IDashboardService dashboardService, IWebHostEnvironment env, IRecommendationService recommendationService  )
        {
            _dashboardService = dashboardService;
            _env = env;
            _recommendationService = recommendationService;
        }


        private static void SectionHeading(ColumnDescriptor col, string title)
        {
            var icon = SectionIcons.TryGetValue(title, out var ic) ? ic + "  " : "";
            col.Item()
               .EnsureSpace(60)
               .PaddingTop(16)
               .BorderBottom(2)
               .BorderColor(AccentYellow)
               .PaddingBottom(4)
               .AlignRight()
               .Text($"{title}  {icon}")
               .Bold()
               .FontSize(14)
               .FontColor(AccentYellow);
        }

        private static void DrawBar(
            ColumnDescriptor col, float filledPct,
            string barColor, string label, string valueText)
        {
            col.Item().PaddingBottom(3).Row(row =>
            {
                row.ConstantItem(120).AlignLeft().AlignMiddle()
                   .Text(label).FontSize(9);

                row.RelativeItem().PaddingHorizontal(6).AlignMiddle().Row(track =>
                {
                    var safePct = Math.Max(0f, Math.Min(1f, filledPct));
                    if (safePct > 0)
                        track.RelativeItem(safePct).Height(12).Background(barColor);
                    if (safePct < 1f)
                        track.RelativeItem(1f - safePct).Height(12).Background(Colors.Grey.Lighten3);
                });

                row.ConstantItem(32).AlignMiddle()
                   .Text(valueText).FontSize(9).FontColor(Colors.Grey.Darken2);
            });
        }


        public async Task<string> GenerateReportAsync( string childId, string viewerRole, string generatedBy)
        {

            var data = _dashboardService.GetChildDashboard(childId, viewerRole);
            if (data == null)
                throw new Exception("لا توجد بيانات لهذا الطفل");

            var aiOverview = await _recommendationService.GenerateReportOverviewAsync(data, viewerRole);

            var folder = Path.Combine(_env.WebRootPath, "reports", childId);
            Directory.CreateDirectory(folder);

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
            var fileName = $"report_{childId}_{viewerRole}_{timestamp}.pdf";
            var filePath = Path.Combine(folder, fileName);

            QuestPDF.Settings.License = LicenseType.Community;

            var coverPath = Path.Combine(_env.WebRootPath, "images", "ReportCover.png");
            var logoPath = Path.Combine(_env.WebRootPath, "images", "LamieLogo.png");

            var avatarPath = !string.IsNullOrWhiteSpace(data.IconImgPath)
                ? Path.Combine(_env.WebRootPath, data.IconImgPath.TrimStart('/', '\\').Replace('/', Path.DirectorySeparatorChar))
                : string.Empty;

            Document.Create(container =>
            {

                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(0); 

                    page.Content().Column(col =>
                    {
                        if (File.Exists(coverPath))
                        {
                            col.Item()
                               .Height(PageSizes.A4.Height)
                               .Image(coverPath)
                               .FitArea();
                        }
                        else
                        {
                            col.Item()
                               .Height(PageSizes.A4.Height)
                               .Background("#FFF5CC")
                               .AlignCenter()
                               .AlignMiddle()
                               .Text("تقرير لامع")
                               .FontSize(36)
                               .Bold()
                               .FontColor(AccentYellow);
                        }
                    });
                });


                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(1.5f, Unit.Centimetre);
                    page.DefaultTextStyle(x =>x.FontFamily("Tahoma") .FontSize(12).DirectionFromRightToLeft());

                    page.Header().Column(header =>
                    {
                        header.Item().Row(row =>
                        {
                            if (File.Exists(logoPath))
                                row.ConstantItem(80).AlignMiddle().Image(logoPath).FitArea();

                            row.RelativeItem().AlignRight().Row(infoRow =>
                            {
                                infoRow.RelativeItem().AlignRight().Column(textCol =>
                                {
                                    textCol.Item().AlignRight()
                                       .Text($"تقرير الطفل: {data.ChildName}")
                                       .FontSize(16).Bold().FontColor(AccentYellow);
                                    textCol.Item().AlignRight()
                                       .Text($"العمر: {data.Age} سنوات")
                                       .FontColor(Colors.Grey.Medium);
                                    textCol.Item().AlignRight()
                                       .Text($"تاريخ التقرير: {DateTime.Now:yyyy/MM/dd}")
                                       .FontColor(Colors.Grey.Medium);
                                    textCol.Item().AlignRight()
                                       .Text($"أنشأه: {(viewerRole == "Educator" ? "المعلم" : "ولي الأمر")}")
                                       .FontColor(Colors.Grey.Medium);
                                });

                                infoRow.ConstantItem(60).AlignMiddle().Padding(4).Column(av =>
                                {
                                    if (!string.IsNullOrEmpty(avatarPath) && File.Exists(avatarPath))
                                    {
                                        av.Item()
                                          .Width(52)
                                          .Height(52)
                                          .Image(avatarPath)
                                          .FitArea();
                                    }
                                    else
                                    {
                                        av.Item()
                                          .Width(52)
                                          .Height(52)
                                          .Background(TopIntelBg)
                                          .AlignCenter()
                                          .AlignMiddle()
                                          .Text("👧")
                                          .FontSize(26);
                                    }
                                });
                            });
                        });

                        header.Item().PaddingTop(8)
                              .LineHorizontal(2).LineColor(AccentYellow);
                        header.Item().PaddingBottom(4);
                    });


                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("صفحة ").FontSize(9).FontColor(Colors.Grey.Medium);
                        text.CurrentPageNumber().FontSize(9).FontColor(Colors.Grey.Medium);
                        text.Span(" من ").FontSize(9).FontColor(Colors.Grey.Medium);
                        text.TotalPages().FontSize(9).FontColor(Colors.Grey.Medium);
                    });

                    page.Content().Column(col =>
                    {
                        if (aiOverview != null)
                        {
                            col.Item().EnsureSpace(80).PaddingTop(16)
                               .Border(0.5f).BorderColor("#9d4ebe")
                               .Background("#f2e9f7")
                               .CornerRadius(10)                      
                               .Padding(12)
                               .Column(inner =>
                               {
                                   inner.Item().AlignRight()
                                        .Text("نظرة عامة").Bold().FontSize(12)
                                        .FontColor("#9d4ebe");

                                   if (!string.IsNullOrEmpty(aiOverview.ParentSummary))
                                       inner.Item().PaddingTop(8).AlignRight()
                                            .Text(x =>
                                            {
                                                x.Span("\u200F" + aiOverview.ParentSummary.Replace(".", " "));
                                            });

                                   if (viewerRole == "Educator" && !string.IsNullOrEmpty(aiOverview.EducatorSummary))
                                   {
                                       inner.Item().PaddingTop(10).AlignRight()
                                            .Text("التحليل التربوي:").Bold().FontSize(10);
                                       inner.Item().PaddingTop(4).AlignRight()
                                            .Text(x =>
                                            {
                                                x.Span("\u200F" + aiOverview.EducatorSummary.Replace(".", " "));
                                            });
                                   }

                                   if (!string.IsNullOrEmpty(aiOverview.Recommendations))
                                   {
                                       inner.Item().PaddingTop(10).AlignRight()
                                            .Text("التوصيات").FontColor("#9d4ebe").Bold().FontSize(10);

                                       var lines = aiOverview.Recommendations
                                            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                                                .Select(l => l.Trim().TrimStart('.', '-', '•', ' '))
                                                 .ToList();

                                       foreach (var line in lines)
                                       {
                                           inner.Item()
                                               .PaddingTop(4)
                                               .PaddingRight(8)
                                               .AlignRight()
                                               .Text("\u200F" + "• " + line)   
                                               .FontSize(9.5f);
                                       }
                                   }

                                   
                                   inner.Item().PaddingTop(10).AlignRight()
                                        .Text("\u200F هذا الملخص أُنشئ بمساعدة الذكاء الاصطناعي بناءً على بيانات الأداء")
                                        .FontSize(8).FontColor("#ff6531").Bold();
                               });
                        }
                        if (data.TopIntelligence != null)
                        {
                            col.Item().PaddingBottom(10).PaddingTop(17).AlignCenter().Column(center =>
                            {
                                center.Item()
                                    .MaxWidth(420)
                                    .Border(2)
                                    .BorderColor(TopIntelBorder)
                                    .Background(TopIntelBg)
                                    .CornerRadius(8)
                                    .Padding(14)
                                    .Column(inner =>
                                    {
                                        inner.Item().AlignCenter().Row(hRow =>
                                        {
                                            hRow.AutoItem()
                                                .Text("أبرز ذكاء")
                                                .Bold()
                                                .FontSize(16)
                                                .FontColor(Colors.Grey.Darken3);
                                            hRow.AutoItem()
                                                .PaddingLeft(8)
                                                .Text("🏅")
                                                .FontSize(20);
                                        });

                                        inner.Item().PaddingTop(6).AlignCenter()
                                             .Text($"{data.TopIntelligence.IntelligenceName} : {data.TopIntelligence.Score}")
                                             .Bold().FontSize(12);

                                        inner.Item().PaddingTop(4).AlignCenter()
                                             .Text($"أفضل جانب: {data.TopIntelligence.BestAspectName} ({Math.Round(data.TopIntelligence.BestAspectScore * 100, 0)}%)")
                                             .FontColor(Colors.Green.Darken2);

                                        inner.Item().PaddingTop(2).AlignCenter()
                                             .Text($"يحتاج تعزيز: {data.TopIntelligence.WeakAspectName} ({Math.Round(data.TopIntelligence.WeakAspectScore * 100, 0)}%)")
                                             .FontColor(Colors.Red.Darken2);
                                    });
                            });
                        }

                        SectionHeading(col, "جميع الذكاءات");
                        col.Item().PaddingTop(8).Column(chartCol =>
                        {
                            var maxVal = data.AllIntelligences.Max(x => (float)x.Value);
                            if (maxVal == 0) maxVal = 100;

                            foreach (var intel in data.AllIntelligences)
                            {
                                var color = IntelColors.TryGetValue(intel.Label, out var c) ? c : "#378ADD";
                                DrawBar(chartCol, intel.Value / maxVal, color, intel.Label, intel.Value.ToString());
                            }
                        });

                        SectionHeading(col, "مستويات الذكاء");
                        col.Item().PaddingTop(6).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(3);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("المستوى").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("الدرجة").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("الذكاء").Bold();
                            });
                            foreach (var intel in data.IntelligenceDetails)
                            {
                                var levelText = intel.ProficiencyScore >= 80 ? "ممتاز"
                                              : intel.ProficiencyScore >= 60 ? "جيد"
                                              : intel.ProficiencyScore >= 40 ? "متوسط" : "يحتاج تحسين";
                                var bgColor = intel.ProficiencyScore >= 80 ? Colors.Green.Lighten5
                                              : intel.ProficiencyScore >= 60 ? Colors.Yellow.Lighten5
                                              : Colors.Red.Lighten5;
                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(levelText);
                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(intel.ProficiencyScore.ToString());
                                table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(intel.IntelligenceName);
                            }
                        });

                        SectionHeading(col, "الجهد مقابل الدرجة");
                        col.Item().PaddingTop(8).Column(chartCol =>
                        {
                            var maxTime = data.EffortVsScore.Max(x => (float)x.TotalTimeSec);
                            if (maxTime == 0) maxTime = 1;

                            foreach (var e in data.EffortVsScore)
                            {
                                var color = IntelColors.TryGetValue(e.IntelligenceName, out var c) ? c : "#378ADD";

                                DrawBar(chartCol, e.Score / 100f, color,
                                    $"{e.IntelligenceName} — درجة", e.Score.ToString());

                                DrawBar(chartCol, (float)(e.TotalTimeSec / maxTime), Colors.Grey.Medium,
                                    $"{e.IntelligenceName} — وقت", $"{Math.Round(e.TotalTimeSec, 0)}ث");

                                chartCol.Item().PaddingBottom(4);
                            }
                        });

                        SectionHeading(col, "مؤشرات الأداء التفصيلية");
                        foreach (var intel in data.IntelligenceDetails)
                        {
                            col.Item().PaddingTop(10)
                              .AlignRight() .Text(intel.IntelligenceName)
                               .Bold().FontSize(11).FontColor(Colors.Black);

                            foreach (var aspect in intel.Aspects)
                            {
                                col.Item().PaddingTop(6).PaddingLeft(8).AlignRight()
                                   .Text($"الجانب: {aspect.AspectName}  ({Math.Round(aspect.AspectScore * 100, 0)}%  —  {aspect.AspectRating})")
                                   .Bold();

                                col.Item().PaddingTop(4).PaddingLeft(12).Table(table =>
                                {
                                    table.ColumnsDefinition(cols =>
                                    {
                                        cols.RelativeColumn(2);
                                        cols.RelativeColumn(1);
                                        cols.RelativeColumn(4);
                                    });
                                    table.Header(h =>
                                    {
                                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("التقييم").Bold();
                                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("الدرجة").Bold();
                                        h.Cell().Background(Colors.Grey.Lighten3).Padding(4).AlignRight().Text("المؤشر").Bold();
                                    });
                                    foreach (var ind in aspect.Indicators)
                                    {
                                        var normalized = ind.IndicatorRating?.Trim() ?? "";
                                        var bgColor = normalized.Contains("غالب") ? Colors.Green.Lighten5
                                                    : (normalized.Contains("أحيان")
                                                    || normalized.Contains("احيان")) ? Colors.Yellow.Lighten5
                                                    : Colors.Red.Lighten5;
                                        table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(ind.IndicatorRating);
                                        table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text($"{Math.Round(ind.IndicatorScore * 100, 0)}%");
                                        table.Cell().Background(bgColor).BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(4).AlignRight().Text(ind.IndicatorName);
                                    }
                                });
                            }
                        }

                        SectionHeading(col, "آخر الجلسات");
                        col.Item().PaddingTop(6).Table(table =>
                        {
                            table.ColumnsDefinition(cols =>
                            {
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(1);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                                cols.RelativeColumn(2);
                            });
                            table.Header(h =>
                            {
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("الوقت (ث)").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("الدرجة").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("الذكاء").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("اللعبة").Bold();
                                h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignRight().Text("التاريخ").Bold();
                            });
                            foreach (var s in data.RecentSessions)
                            {
                                var pct = Math.Round(s.AspectScore * 100, 0);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(Math.Round(s.TotalTimeSec, 0).ToString());
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text($"{pct}%");
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(s.IntelligenceName);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(s.LevelName);
                                table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).AlignRight().Text(s.PlayedAt.ToString("yyyy/MM/dd"));
                            }
                        });

                        SectionHeading(col, "منحنى الأداء عبر الزمن");
                        col.Item().PaddingTop(8).Column(chartCol =>
                        {
                            var trendByIntel = data.ScoreTrend
                                .GroupBy(p => p.IntelligenceName)
                                .ToList();

                            foreach (var group in trendByIntel)
                            {
                                var color = IntelColors.TryGetValue(group.Key, out var c) ? c : "#378ADD";
                                var points = group.OrderBy(p => p.Date).ToList();

                                chartCol.Item().PaddingBottom(6).Column(inner =>
                                {
                                    inner.Item().Text(group.Key).FontSize(9).Bold().FontColor(color);
                                    foreach (var pt in points)
                                    {
                                        DrawBar(inner, (float)Math.Min(pt.Score, 1.0),
                                            color,
                                            pt.Date.ToString("yyyy/MM/dd"),
                                            $"{Math.Round(pt.Score * 100, 1)}%");
                                    }
                                });
                            }
                        });

                        if (viewerRole == "Parent" &&
                            data.TopActivities != null &&
                            data.TopActivities.Any())
                        {
                            SectionHeading(col, "الأنشطة المقترحة");
                            col.Item().PaddingTop(6).Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(2);
                                    cols.RelativeColumn(1);
                                    cols.RelativeColumn(3);
                                    cols.RelativeColumn(1);
                                });
                                table.Header(h =>
                                {
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("النشاط").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("الفئة").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(5).Text("الوصف").Bold();
                                    h.Cell().Background(Colors.Grey.Lighten3).Padding(5).AlignCenter().Text("المدة (د)").Bold();
                                });
                                foreach (var act in data.TopActivities)
                                {
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(act.ActivityName ?? "-");
                                    table.Cell().BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(5).Text(act.Category ?? "-");
                                }
                            });
                        }
                    });
                });
            }).GeneratePdf(filePath);

            return fileName;
        }

        public List<ReportFileViewModel> GetChildReports(string childId, string viewerRole)
        {
            var folder = Path.Combine(_env.WebRootPath, "reports", childId);
            if (!Directory.Exists(folder))
                return new List<ReportFileViewModel>();

            return Directory.GetFiles(folder, $"report_{childId}_{viewerRole}_*.pdf")
                .Select(f =>
                {
                    var name = Path.GetFileNameWithoutExtension(f);
                    var parts = name.Split('_');
                    var dateStr = parts.Length > 3 ? parts[3] : "";
                    var timeStr = parts.Length > 4 ? parts[4] : "0000";
                    var parsed = DateTime.TryParseExact(
                                      dateStr + timeStr, "yyyyMMddHHmm", null,
                                      System.Globalization.DateTimeStyles.None,
                                      out var dt) ? dt : File.GetCreationTime(f);
                    return new ReportFileViewModel
                    {
                        FileName = Path.GetFileName(f),
                        FilePath = $"/reports/{childId}/{Path.GetFileName(f)}",
                        GeneratedAt = parsed,
                        GeneratedBy = viewerRole == "Educator" ? "المعلم" : "ولي الأمر",
                        Role = viewerRole,
                        ChildId = childId
                    };
                })
                .OrderByDescending(r => r.GeneratedAt)
                .ToList();
        }
    }
}