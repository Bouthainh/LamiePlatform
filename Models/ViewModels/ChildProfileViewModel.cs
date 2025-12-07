namespace BadeePlatform.Models.ViewModels
{
    public class ChildProfileViewModel
    {
        public string ChildId { get; set; }
        public string ChildName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string LoginCode { get; set; }
        public string IconImgPath { get; set; }

        public string SchoolName { get; set; }
        public string City { get; set; }
        public string Grade { get; set; }
        public string Class { get; set; }
        public string EducatorName { get; set; }

        public string RelationshipType { get; set; }

        public bool HasPermission { get; set; }

        public string RequestStatus { get; set; } 
    }
}