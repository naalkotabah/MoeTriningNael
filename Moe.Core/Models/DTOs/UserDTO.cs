using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Moe.Core.Models.Entities;

namespace Moe.Core.Models.DTOs.User;

public class UserDTO : BaseDTO
{
    #region Auto
    public StaticRole Role { get; set; }
    
    
    public string? Email { get; set; }
    
    public string? Phone { get; set; }
    public string? PhoneCountryCode { get; set; }
    
    
    public string? Name { get; set; }
    //public string? ProfileImg { get; set; }
    //public string? CoverImg { get; set; }
    #endregion

    #region Manual
    #endregion
}

public class UserFormDTO : BaseFormDTO
{
    [Required] public StaticRole StaticRole { get; set; }

    
    [EmailAddress] 
    [MaxLength(320)] 
    public string? Email { get; set; }

    [MaxLength(16)] 
    public string? Phone { get; set; }
    [MaxLength(8)] 
    public string? PhoneCountryCode { get; set; }
    
    [Required] 
    [MinLength(6)] 
    public string Password { get; set; }
    
    
    [Required]
    [StringLength(128, MinimumLength = 3)] 
    public string Name { get; set; }


    public Guid? WarehouseId { get; set; }

    //[MaxLength(64)] public string? ProfileImg { get; set; }
    //[MaxLength(64)] public string? CoverImg { get; set; }
}
public class UserFormValidator : AbstractValidator<UserFormDTO>
{
    public UserFormValidator()
    {

        RuleFor(x => x.WarehouseId)
    .NotNull()
    .WithMessage("Warehouse is required for WAREHOUSE_ADMIN")
    .When(x => x.StaticRole == StaticRole.WAREHOUSE_ADMIN);


    }
}

public class UserUpdateDTO : BaseUpdateDTO
{
    [EmailAddress] 
    [MaxLength(320)] 


    public string? Email { get; set; }

    [Required] public StaticRole StaticRole { get; set; }


    [MinLength(6)]
    public string Password { get; set; }

    [MaxLength(16)] 
    public string? Phone { get; set; }
    [MaxLength(8)] 
    public string? PhoneCountryCode { get; set; }



    [MaxLength(128)] public string? Name { get; set; }
    
    //[MaxLength(64)] public string? ProfileImg { get; set; }
    //[MaxLength(64)] public string? CoverImg { get; set; }
}




public class UserFilterDTO : BaseFilter
{
    public StaticRole? Role { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Name { get; set; }
}



public class UserCustomDTO
{
    public string Role { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string PhoneCountryCode { get; set; }
    public string Name { get; set; }
}
