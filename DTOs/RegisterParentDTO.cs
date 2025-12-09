namespace BadeePlatform.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class RegisterParentDTO
    {
        [Required(ErrorMessage = "الهوية الوطنية مطلوبة.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "يجب أن يتكون رقم الهوية من 10 خانات.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "يجب أن يتكون رقم الهوية من أرقام فقط.")]
        public string ParentId { get; set; }

        // NEW: First Name
        [Required(ErrorMessage = "الاسم الأول مطلوب.")]
        [StringLength(50, ErrorMessage = "الاسم الأول لا يمكن أن يتجاوز 50 حرف.")]
        public string FirstName { get; set; }

        // NEW: Last Name
        [Required(ErrorMessage = "اللقب مطلوب.")]
        [StringLength(50, ErrorMessage = "اللقب لا يمكن أن يتجاوز 50 حرف.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "رقم الهاتف مطلوب.")]
        [RegularExpression(@"^05[0-9]{8}$", ErrorMessage = "يجب أن يبدأ رقم الهاتف بـ 05 ويتكون من 10 أرقام.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "يجب أن يتكون رقم الهاتف من 10 أرقام.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة.")]
        [StringLength(100, ErrorMessage = "البريد الإلكتروني لا يمكن أن يتجاوز 100 حرف.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "اسم المستخدم مطلوب.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "يجب أن يتراوح طول اسم المستخدم بين 3 و 50 حرف.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "يجب أن تكون كلمة المرور 8 أحرف على الأقل.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[\W_]).{8,}$",
    ErrorMessage = "يجب أن تحتوي كلمة المرور على: حرف كبير، حرف صغير، رقم، ورمز خاص مثل ! @ # $ % ^ & * ( ) _ - +")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "كلمة المرور وتأكيدها غير متطابقتين.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "يجب اختيار دورك (أم/أب).")]
        public string Role { get; set; }


    }
}