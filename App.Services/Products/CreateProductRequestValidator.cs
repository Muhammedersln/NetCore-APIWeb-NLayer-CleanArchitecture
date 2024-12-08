using FluentValidation;

namespace App.Services.Products
{
    public class CreateProductRequestValidator:AbstractValidator<CreateProductRequest>
    {
        private readonly IProductService _productService;
        public CreateProductRequestValidator(IProductService productService)
        {
            _productService = productService;
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .Length(3, 10).WithMessage("3-10 arasında karakter olmalı");

            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.Stock).InclusiveBetween(1, 100).WithMessage("Stock must be between 1-100");
            
        }

    }
}
