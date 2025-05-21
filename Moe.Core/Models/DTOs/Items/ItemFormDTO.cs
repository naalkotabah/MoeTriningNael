using FluentValidation;
using Moe.Core.Models.DTOs.Warehouse;
using System.ComponentModel.DataAnnotations;

namespace Moe.Core.Models.DTOs.Items
{
    public class ItemFormDTO
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required, MaxLength(50)]
        public string Code { get; set; }

        [Required, MaxLength(20)]
        public string Unit { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }


        public string? ImagePath { get; set; }
    }


public class ItemFormValidator : AbstractValidator<ItemFormDTO>
    {
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

        public ItemFormValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("اسم المنتج مطلوب.")
                .MaximumLength(100).WithMessage("اسم المنتج لا يجب أن يتجاوز 100 حرف.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("رمز المنتج مطلوب.")
                .MaximumLength(50).WithMessage("رمز المنتج لا يجب أن يتجاوز 50 حرف.");

            RuleFor(x => x.Unit)
                .NotEmpty().WithMessage("وحدة القياس مطلوبة.")
                .MaximumLength(20).WithMessage("وحدة القياس لا يجب أن تتجاوز 20 حرف.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("الوصف لا يجب أن يتجاوز 500 حرف.");

            RuleFor(x => x.ImagePath)
                .NotEmpty().WithMessage("صورة المنتج مطلوبة.")
                .Must(HaveValidImageExtension).WithMessage("امتداد الصورة غير مدعوم. يجب أن تكون (jpg, jpeg, png, gif).");
        }

        private bool HaveValidImageExtension(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return false;

            var extension = Path.GetExtension(imagePath).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }
    }


    public class ItemDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Unit { get; set; }
        public string? Description { get; set; }
        public string? ImagePath { get; set; }
    }

    public class ItemFilterDTO : BaseFilter
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }

}
