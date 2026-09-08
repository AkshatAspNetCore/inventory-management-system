using IMS.WebApp.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace IMS.WebApp.ViewModelsValidations
{
    public class Produce_EnsureEnoughInventoryQuantity :  ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var produceViewModel = validationContext.ObjectInstance as ProduceViewModel;
            if(produceViewModel is not null)
            {
                if (produceViewModel.Product is not null && produceViewModel.Product.ProductInventories is not null) 
                {
                    foreach (var pi in produceViewModel.Product.ProductInventories) 
                    {
                        if (pi.Inventory is not null &&
                            (pi.InventoryQuantity * produceViewModel.QuantityToProduce) > pi.Inventory.Quantity)
                        {
                            return new ValidationResult($"Not enough quantity of inventory '{pi.Inventory.InventoryName}' to produce {produceViewModel.QuantityToProduce} units of product '{produceViewModel.Product.ProductName}'. " +
                                $"Required: {pi.InventoryQuantity * produceViewModel.QuantityToProduce}, Available: {pi.Inventory.Quantity}.",
                                new List<string>() { validationContext.MemberName });
                        }
                    }
                }
            }
            return ValidationResult.Success;    
        }
    }
}
