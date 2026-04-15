using System.ComponentModel.DataAnnotations;

namespace LamiePlatform.DTOs.PlatformDTOs
{
    public class LoginEducatorDTO
    {
        [Required(ErrorMessage = "اسم المستخدم أو البريد الإلكتروني مطلوب.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "يجب أن يتراوح المدخل بين 3 و 100 حرف.")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "كلمة المرور مطلوبة.")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "كلمة المرور يجب أن تكون 8 أحرف على الأقل.")]
        public string Password { get; set; }
    }
}
