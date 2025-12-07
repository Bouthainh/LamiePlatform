using BadeePlatform.Data;
using BadeePlatform.Helpers;
using BadeePlatform.Models;
using BadeePlatform.Models.ViewModels;

namespace BadeePlatform.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly BadeedbContext _db;

        public DashboardService(BadeedbContext db)
        {
            _db = db;
        }
        public ChildDashboardViewModel GetChildDashboard(string childId)
        {
            var child = _db.Children.FirstOrDefault(x => x.ChildId == childId);

            //Detection of null values

            if (child == null)
                return null;

            bool hasData = _db.ChildIntelligences.Any(x => x.ChildId == childId);

            if (!hasData)
                return null;

            // Get Top 3 intelligences
            var top3 = _db.ChildIntelligences
           .Where(x => x.ChildId == childId)
           .OrderByDescending(x => x.ProficiencyScore)
           .Take(3)
           .Select(x => new ChartItemViewModel
           {
               Label = x.Intelligence != null ? x.Intelligence.IntelligenceName : "غير معروف",
               Value = x.ProficiencyScore.HasValue ? (int)x.ProficiencyScore.Value : 0
           })
           .ToList();

            // Get All intelligences
            var radar = _db.ChildIntelligences
            .Where(x => x.ChildId == childId)
            .Select(x => new ChartItemViewModel
            {
                Label = x.Intelligence != null ? x.Intelligence.IntelligenceName : "غير معروف",
                Value = x.ProficiencyScore.HasValue ? (int)x.ProficiencyScore.Value : 0
            })
               .ToList();

            // Get Educator 
            var educator = (from ep in _db.EducatorPermissions
                            join e in _db.Educators on ep.EducatorId equals e.EducatorId
                            where ep.ChildId == childId
                            select e).FirstOrDefault();

            //return the Full View Model after retrieving data
            return new ChildDashboardViewModel
            {
                ChildName = child.ChildName,
                Age = (int)child.Age,
                IconImgPath = ChildExtensions.GetIconPathByGender(child.Gender),

                EducatorName = educator?.EducatorName ?? "لا يوجد تصريح للمعلم لهذا الطفل",
                EducatorEmail = educator?.Email ?? "لا يمكن العثور على وسيلة تواصل مع المعلم",

                Top3Intelligences = top3,
                AllIntelligences = radar,

                // HardCoded UI data: (Report & activity) TO BE DYNAMIC LATER
                Activities = new List<string>
                            {
                                "نشاط المقارنة",
                                "نشاط تركيب الأشكال",
                                "نشاط الألوان المتشابهة"
                            },

                Reports = new List<string>
                            {
                                "تقرير مرحلة الاستكشاف 2025/01/11",
                                "تقرير مرحلة التطوير الأولى 2025/01/15"
                            },

                challenges = "50/13",
                level = "المستوى الأول"
            };
        }
    }
}
