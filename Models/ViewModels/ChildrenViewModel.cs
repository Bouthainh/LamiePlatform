using System.ComponentModel.DataAnnotations;

namespace LamiePlatform.Models.ViewModels
{
    public class ChildrenViewModel
    {

        public string ChildId { get; set; }
        public string ChildName { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public string? IconImgPath { get; set; }



    }
}
