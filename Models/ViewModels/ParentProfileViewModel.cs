using System.ComponentModel.DataAnnotations;

namespace BadeePlatform.Models.ViewModels
{
    public class ParentProfileViewModel
    {
        [Required(ErrorMessage = "رقم الهوية مطلوب")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "يجب أن يتكون رقم الهوية من 10 خانات.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "رقم الهوية يجب أن يتكون من 10 أرقام")]
        public string ParentId { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [StringLength(100, ErrorMessage = "الاسم يجب ألا يتجاوز 100 حرف")]
        public string ParentName { get; set; }

        [Required(ErrorMessage = "رقم الجوال مطلوب")]
        [RegularExpression(@"^05\d{8}$", ErrorMessage = "رقم الجوال يجب أن يبدأ بـ 05 ويتكون من 10 أرقام")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [MinLength(8, ErrorMessage = "اسم المستخدم يجب أن لا يقل عن 8 أحرف")]
        public string Username { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة")]
        [MinLength(8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    }

