using System.ComponentModel.DataAnnotations;

namespace BadeePlatform.Models.ViewModels
{
    public class ParentProfileViewModel
    {
        [Required(ErrorMessage = "رقم الهوية مطلوب")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "يجب أن يتكون رقم الهوية من 10 خانات.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "رقم الهوية يجب أن يتكون من 10 أرقام")]
        public string ParentId { get; set; }

        [Required(ErrorMessage = "الاسم الأول مطلوب.")]
        [StringLength(50, ErrorMessage = "الاسم الأول لا يمكن أن يتجاوز 50 حرف.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "اسم العائلة مطلوب.")]
        [StringLength(50, ErrorMessage = "اسم العائلة لا يمكن أن يتجاوز 50 حرف.")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "رقم الجوال مطلوب")]
        [RegularExpression(@"^05\d{8}$", ErrorMessage = "رقم الجوال يجب أن يبدأ بـ 05 ويتكون من 10 أرقام")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب")]
        [MinLength(3, ErrorMessage = "اسم المستخدم يجب أن لا يقل عن 3 أحرف")]
        public string Username { get; set; }

        [MinLength(8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$",
    ErrorMessage = "يجب أن تحتوي كلمة المرور على: حرف كبير، حرف صغير، رقم، ورمز خاص مثل ! @ # $ % ^ & * ( ) _ - +")]
        public string? Password { get; set; }
    }
    }

