namespace LamiePlatform.Models.ViewModels.DashboardViewModels
{
    public class ChildDashboardViewModel
    {
        public string ChildId { get; set; }
        public string ChildName { get; set; }
        public int Age { get; set; }
        public string IconImgPath { get; set; }

        public string ViewerRole { get; set; }   

        public ContactCardViewModel EducatorContact { get; set; }
        public ContactCardViewModel ParentContact { get; set; }

        public List<ChartItemViewModel> Top3Intelligences { get; set; }  
        public List<ChartItemViewModel> AllIntelligences { get; set; }    

        public List<EffortScoreViewModel> EffortVsScore { get; set; }

        public TopIntelligenceSummaryViewModel TopIntelligence { get; set; }

        public List<RecentSessionViewModel> RecentSessions { get; set; }

        public List<ScoreTrendPointViewModel> ScoreTrend { get; set; }

        public List<IntelligenceDetailViewModel> IntelligenceDetails { get; set; }

        public List<RecommendationListItem> TopActivities { get; set; }

    }
}
