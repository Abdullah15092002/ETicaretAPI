using ETicaretAPI.Application.ViewModels.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Validators.Products
{
    public class CreateProductValidator : AbstractValidator<VM_Create_Product>
    {

        public CreateProductValidator()
        {
            RuleFor(p => p.Name)

                .NotEmpty()
                .NotNull().WithMessage("İsim Boş Olamaz!")
                .MaximumLength(150).WithMessage("Ürün Adı Maximum 150 karakter olabilir")
                .MinimumLength(5).WithMessage("Ürün Adı Minimum 5 Karakter olmalı");


            RuleFor(p => p.Stock)
                .NotEmpty()
                .NotNull().WithMessage("Stok Boş Olamaz!")
                .Must(n => n > 0).WithMessage("Stok Bilgisi Negatif Olamaz");

            RuleFor(p => p.Price)
                .NotEmpty()
                .NotNull().WithMessage("Fiyat Boş Olamaz!")
                .Must(n => n > 0).WithMessage("Fiyat Negatif Olamaz");
        }
    }
}
