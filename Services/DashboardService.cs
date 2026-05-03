using LamiePlatform.Data;
using LamiePlatform.Helpers;
using LamiePlatform.Models;
using LamiePlatform.Models.ViewModels;
using LamiePlatform.Models.ViewModels.DashboardViewModels;
using Microsoft.EntityFrameworkCore;

namespace LamiePlatform.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly LamieDbContext _db;

        public DashboardService(LamieDbContext db)
        {
            _db = db;
        }
        private void CalculateAndUpdateIntelligenceScores(string childId)
        {
            var childIntelligences = _db.ChildIntelligences
                .Where(ci => ci.ChildId == childId)
                .ToList();

            foreach (var ci in childIntelligences)
            {
                var allAspectResults = _db.AspectResults
                    .Where(a => a.IntelligenceId == ci.IntelligenceId
                             && a.GameSession.ChildId == childId)
                    .ToList();

                if (!allAspectResults.Any())
                    continue;

                double calculatedScore = allAspectResults
                    .GroupBy(a => a.AspectName)
                    .Select(g => g.Average(a => a.AspectScore))
                    .Average()   
                    * 100;       

                ci.ProficiencyScore = (decimal)Math.Round(calculatedScore, 2);
                ci.AssessmentDate = DateTime.Now;
            }

            _db.SaveChanges();
        }

        public ChildDashboardViewModel GetChildDashboard(string childId, string viewerRole)
        {

            CalculateAndUpdateIntelligenceScores(childId); //calculate latest scores before fetching data for dashboard

            var child = _db.Children
                .Include(c => c.Class)
                .Include(c => c.Grade)
                .FirstOrDefault(x => x.ChildId == childId);

            if (child == null) return null;

            bool hasData = _db.ChildIntelligences.Any(x => x.ChildId == childId);
            if (!hasData) return null;

            var educator = (from ep in _db.EducatorPermissions
                            join e in _db.Educators on ep.EducatorId equals e.EducatorId
                            where ep.ChildId == childId
                            select e).FirstOrDefault();

            var educatorContact = new ContactCardViewModel
            {
                Name = educator?.EducatorName ?? "لا يوجد معلم مرتبط",
                Email = educator?.Email ?? "-",
                ClassName = child.Class?.ClassName ?? "-",
                GradeName = child.Grade?.GradeName ?? "-",
                UserId = educator?.EducatorId ?? "",
                Role = "Educator"
            };

            var parent = (from pc in _db.ParentChildren
                          join p in _db.Parents on pc.ParentId equals p.ParentId
                          where pc.ChildId == childId
                          select p).FirstOrDefault();

            var parentContact = new ContactCardViewModel
            {
                Name = parent?.ParentName ?? "لا يوجد ولي أمر مرتبط",
                Email = parent?.Email ?? "-",
                ClassName = child.Class?.ClassName ?? "-",
                GradeName = child.Grade?.GradeName ?? "-",
                UserId = parent?.ParentId ?? "",
                Role = "Parent"
            };

            var allIntelligences = _db.ChildIntelligences
                .Where(x => x.ChildId == childId)
                .Include(x => x.Intelligence)
                .OrderByDescending(x => x.ProficiencyScore)
                .ToList();

            var allIntelChart = allIntelligences
                .Select(x => new ChartItemViewModel
                {
                    Label = x.Intelligence?.IntelligenceName ?? "غير معروف",
                    Value = x.ProficiencyScore.HasValue ? (int)x.ProficiencyScore.Value : 0
                }).ToList();

            var top3 = allIntelChart.Take(3).ToList();

            var topIntel = allIntelligences.FirstOrDefault();
            TopIntelligenceSummaryViewModel topSummary = null;

            if (topIntel != null)
            {
                var aspectAverages = _db.AspectResults
                    .Where(a => a.IntelligenceId == topIntel.IntelligenceId
                             && a.GameSession.ChildId == childId)
                    .ToList()
                    .GroupBy(a => a.AspectName)
                    .Select(g => new { Name = g.Key, Avg = g.Average(a => a.AspectScore) })
                    .ToList();

                var best = aspectAverages.OrderByDescending(a => a.Avg).FirstOrDefault();
                var weak = aspectAverages.Count > 1
                    ? aspectAverages.OrderBy(a => a.Avg).FirstOrDefault()
                    : null;

                topSummary = new TopIntelligenceSummaryViewModel
                {
                    IntelligenceName = topIntel.Intelligence?.IntelligenceName ?? "غير معروف",
                    Score = topIntel.ProficiencyScore.HasValue ? (int)topIntel.ProficiencyScore.Value : 0,
                    BestAspectName = best?.Name ?? "-",
                    BestAspectScore = Math.Round(best?.Avg ?? 0, 2),
                    WeakAspectName = weak?.Name ?? "-",
                    WeakAspectScore = Math.Round(weak?.Avg ?? 0, 2)
                };
            }

            var effortData = _db.ChildIntelligences
                .Where(ci => ci.ChildId == childId)
                .Include(ci => ci.Intelligence)
                .Select(ci => new EffortScoreViewModel
                {
                    IntelligenceName = ci.Intelligence != null ? ci.Intelligence.IntelligenceName : "غير معروف",
                    Score = ci.ProficiencyScore.HasValue ? (int)ci.ProficiencyScore.Value : 0,
                    TotalTimeSec = _db.GameSessions
                        .Where(gs => gs.ChildId == childId
                                  && gs.Level != null
                                  && gs.Level.IntelligenceId == ci.IntelligenceId
                                  && gs.TotalTime.HasValue)
                        .Sum(gs => gs.TotalTime ?? 0),
                    SessionCount = _db.GameSessions
                        .Count(gs => gs.ChildId == childId
                                  && gs.Level != null
                                  && gs.Level.IntelligenceId == ci.IntelligenceId)
                })
                .ToList();

            var recentSessions = _db.GameSessions
                .Where(gs => gs.ChildId == childId)
                .Include(gs => gs.Level).ThenInclude(l => l.Intelligence)
                .Include(gs => gs.AspectResults)
                .OrderByDescending(gs => gs.PlayedAt)
                .Take(4)
                .ToList()
                .Select(gs => new RecentSessionViewModel
                {
                    PlayedAt = gs.PlayedAt ?? DateTime.MinValue,
                    LevelName = gs.Level?.LevelName ?? "غير معروف",
                    IntelligenceName = gs.Level?.Intelligence?.IntelligenceName ?? "غير معروف",
                    AspectScore = gs.AspectResults.Any()
                        ? Math.Round(gs.AspectResults.Average(a => a.AspectScore), 2)
                        : 0,
                    TotalTimeSec = gs.TotalTime ?? 0
                })
                .ToList();

            var scoreTrend = _db.AspectResults
                .Where(a => a.GameSession.ChildId == childId)
                .Include(a => a.GameSession)
                .Include(a => a.Intelligence)
                .ToList()
                .GroupBy(a => new
                {
                    Date = a.GameSession.PlayedAt?.Date ?? DateTime.MinValue.Date,
                    IntelligenceName = a.Intelligence?.IntelligenceName ?? "غير معروف"
                })
                .Select(g => new ScoreTrendPointViewModel
                {
                    Date = g.Key.Date,
                    IntelligenceName = g.Key.IntelligenceName,
                    Score = Math.Round(g.Average(a => a.AspectScore), 2)
                })
                .OrderBy(p => p.Date)
                .ToList();

            var intelligenceDetails = new List<IntelligenceDetailViewModel>();

            foreach (var ci in allIntelligences)
            {
                var allLevels = _db.GameLevels
                    .Where(l => l.IntelligenceId == ci.IntelligenceId)
                    .Select(l => new { l.LevelId, l.LevelName })
                    .ToList();

                var playedLevelIds = _db.GameSessions
                    .Where(gs => gs.ChildId == childId
                              && gs.Level != null
                              && gs.Level.IntelligenceId == ci.IntelligenceId)
                    .Select(gs => gs.LevelId)
                    .Distinct()
                    .ToHashSet();

                var levelViewModels = allLevels.Select(l => new LevelStatusViewModel
                {
                    LevelId = l.LevelId,
                    LevelName = l.LevelName ?? "غير معروف",
                    IsPlayed = playedLevelIds.Contains(l.LevelId)
                }).ToList();


                bool isUnderMeasurement = allLevels.Count > 0
                       && !allLevels.All(l => playedLevelIds.Contains(l.LevelId));

                var latestSession = _db.GameSessions
                    .Where(gs => gs.ChildId == childId
                              && gs.Level != null
                              && gs.Level.IntelligenceId == ci.IntelligenceId)
                    .OrderByDescending(gs => gs.PlayedAt)
                    .FirstOrDefault();

                if (latestSession == null) continue;

                var aspectResults = _db.AspectResults
                    .Where(a => a.GameSessionId == latestSession.GameSessionId
                             && a.IntelligenceId == ci.IntelligenceId)
                    .Include(a => a.IndicatorResults)
                        .ThenInclude(ind => ind.AssessmentItems)
                    .ToList();


                intelligenceDetails.Add(new IntelligenceDetailViewModel
                {
                    IntelligenceId = ci.IntelligenceId,
                    IntelligenceName = ci.Intelligence?.IntelligenceName ?? "غير معروف",
                    ProficiencyScore = ci.ProficiencyScore.HasValue ? (int)ci.ProficiencyScore.Value : 0,
                    IntelligenceLevel = ci.IntelligenceLevel ?? "-",
                    TotalLevels = allLevels.Count,
                    CompletedLevels = playedLevelIds.Count,
                    IsUnderMeasurement = isUnderMeasurement,
                    Levels = levelViewModels,
                    Aspects = aspectResults.Select(a => new AspectDetailViewModel
                    {
                        AspectName = a.AspectName,
                        AspectScore = Math.Round(a.AspectScore, 2),
                        AspectRating = a.AspectRating,
                        Indicators = a.IndicatorResults.Select(ind => new IndicatorDetailViewModel
                        {
                            IndicatorName = ind.IndicatorName,
                            IndicatorScore = Math.Round(ind.IndicatorScore, 2),
                            IndicatorRating = ind.IndicatorRating,
                            PsychometricPts = ind.PsychometricPts,
                            AssessmentItems = ind.AssessmentItems.Select(item => new AssessmentItemViewModel
                            {
                                ItemName = item.ItemName ?? "-",
                                FinalScore = Math.Round(item.FinalScore, 2),
                                PsychometricPts = item.PsychometricPts,
                                Rating = item.Rating
                            }).ToList()
                        }).ToList()
                    }).ToList()
                });
            }

            List<RecommendationListItem> topActivities = null;

            if (viewerRole == "Parent")
            {
                topActivities = _db.ActivityRecommendations
                    .Where(a => a.ChildId == childId)
                    .OrderByDescending(a => a.RecommendationId)
                    .Take(3)
                    .Select(a => new RecommendationListItem
                    {
                        RecommendationId = a.RecommendationId,
                        ActivityName = a.ActivityName,
                        Category = a.Category,
                    })
                    .ToList();
            }

            return new ChildDashboardViewModel
            {
                ChildId = child.ChildId,
                ChildName = child.ChildName,
                Age = child.Age ?? 0,
                IconImgPath = ChildExtensions.GetIconPathByGender(child.Gender),
                ViewerRole = viewerRole,

                EducatorContact = educatorContact,
                ParentContact = parentContact,

                Top3Intelligences = top3,
                AllIntelligences = allIntelChart,
                EffortVsScore = effortData,
                TopIntelligence = topSummary,

                RecentSessions = recentSessions,
                ScoreTrend = scoreTrend,

                IntelligenceDetails = intelligenceDetails,
                TopActivities = topActivities
            };
        }
    }
}