using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Moe.Core.Models.Entities
{
    public class ChangeEmailRequest : BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(128)]
        public string NewEmail { get; set; }

        [Required]
        [MaxLength(6)]
        public string Otp { get; set; }

        public State State { get; set; } = State.PENDING;
    }



    public class ChangeEmailRequestFormDTO
    {
        [BindNever]
        [JsonIgnore]
        public Guid UserId { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(128)]
        public string NewEmail { get; set; }
    }


    public class ChangeEmailRequestVerificationFormDTO
    {
        [Required]
        public Guid Id { get; set; }  

        [BindNever]
        [JsonIgnore]
        public Guid CurId { get; set; }  

        [Required]
        [MaxLength(6)]
        public string Otp { get; set; }
    }


}
