using System.ComponentModel.DataAnnotations;

namespace BadeePlatform.DTOs
{
    public class EditChildDTO
    {

        [Required(ErrorMessage = "الاسم الأول مطلوب")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "الاسم الأول يجب أن يحتوي على حروف فقط")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "اسم الأب مطلوب")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "اسم الأب يجب أن يحتوي على حروف فقط")]
        public string FatherName { get; set; }

        [Required(ErrorMessage = "اسم الجد مطلوب")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "اسم الجد يجب أن يحتوي على حروف فقط")]
        public string GrandFatherName { get; set; }

        [Required(ErrorMessage = "اسم العائلة مطلوب")]
        [StringLength(50)]
        [RegularExpression(@"^[a-zA-Z\u0600-\u06FF\s]+$", ErrorMessage = "اسم العائلة يجب أن يحتوي على حروف فقط")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "الجنس مطلوب")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "العمر مطلوب")]
        [Range(4, 8, ErrorMessage = "العمر يجب أن يكون بين 4 و 8 سنوات")]
        public int Age { get; set; }

        [Required(ErrorMessage = "يجب اختيار المدينة")]
        public string City { get; set; }

        [Required(ErrorMessage = "يجب اختيار المدرسة")]
        public Guid? SchoolId { get; set; }

        [Required(ErrorMessage = "يجب اختيار المرحلة الدراسية")]
        public Guid? GradeId { get; set; }

        [Required(ErrorMessage = "يجب اختيار الفصل")]
        public Guid? ClassId { get; set; }

    }
}
