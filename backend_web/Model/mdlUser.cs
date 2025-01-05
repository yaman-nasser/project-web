using System.ComponentModel.DataAnnotations;

namespace backend_web.Model
{
    public class mdlUser
    {
        [MaxLength(100)]
        public string? Name { get; set; }
        public string? Email { get; set; }
        [MaxLength(15), MinLength(8)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*\d)[a-zA-Z\d]{8,15}$")]
        public string? Password { get; set; }
        [RegularExpression(@"^(077|078|079)\d{7}$", ErrorMessage = "Phone number must start with 077, 078, or 079 and contain exactly 10 digits")]
        public string? Phone { get; set; }
    }
}
