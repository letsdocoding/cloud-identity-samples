using System.ComponentModel.DataAnnotations;

namespace letsdocoding_cognitologin.Models
{
    public class UserSecretModel
    {
        [MaxLength(6, ErrorMessage = "Length of code code should be 6 digits.")]
        [MinLength(6, ErrorMessage = "Length of code code should be 6 digits.")]
        [Required]
        public string UserCode { get; set; }
        public string DeviceName { get; set; }
        public bool IsVerified { get; set; }
    }
}
