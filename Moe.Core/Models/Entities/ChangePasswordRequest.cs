using System.ComponentModel.DataAnnotations;
using static Moe.Core.Services.AuthService;

namespace Moe.Core.Models.Entities
{
    public class ChangePasswordRequest : BaseEntity
    {
   

        [MaxLength(6)]
        public string Otp { get; set; }

        [Required]
        public string NewPasswordHash { get; set; }

        [Required]
        public string NewPasswordSalt { get; set; }

        public State State { get; set; } = State.PENDING;
    }


    [RequireEmailOrPhone]
    public class ChangePasswordRequestFormDTO
    {
        public string? Email { get; set; }
        public string? Phone { get; set; }


        [Required]
        [StringLength(30, MinimumLength = 6)]
        public string NewPassword { get; set; }
    }

    public class ChangePasswordRequestVerificationFormDTO
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Otp { get; set; }
    }


    public enum State
    {
        PENDING = 0,
        VERIFIED = 1
    }

}
