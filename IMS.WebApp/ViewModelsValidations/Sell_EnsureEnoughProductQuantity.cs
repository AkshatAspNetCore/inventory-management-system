using IMS.WebApp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModelsValidations
{
    public class Sell_EnsureEnoughProductQuantity : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var sellViewModel = validationContext.ObjectInstance as SellViewModel;
            if (sellViewModel is not null)
            {
                if (sellViewModel.Product is not null && sellViewModel.Product.Quantity < sellViewModel.QuantityToSell)
                {
                    return new ValidationResult($"Not enough quantity of product '{sellViewModel.Product.ProductName}' to sell {sellViewModel.QuantityToSell} units. " +
                        $"Available: {sellViewModel.Product.Quantity}.",
                        new List<string>() { validationContext.MemberName });
                }
            }
            return ValidationResult.Success;
        }
    }
}
