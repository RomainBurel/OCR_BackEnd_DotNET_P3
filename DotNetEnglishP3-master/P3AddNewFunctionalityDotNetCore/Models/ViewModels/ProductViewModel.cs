using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace P3AddNewFunctionalityDotNetCore.Models.ViewModels
{
    public class ProductViewModel
    {
        [BindNever]
        public int Id { get; set; }

        [Required(ErrorMessage = "ErrorMissingName")]
        public string Name { get; set; }

        public string Description { get; set; }

        public string Details { get; set; }

        [Required(ErrorMessage = "ErrorMissingStock")]
        [RegularExpression(@"^-?\d+$", ErrorMessage = "ErrorStockNotAnInteger")]
        [Range(1, int.MaxValue, ErrorMessage = "ErrorStockNotGreaterThanZero")]
        public string Stock { get; set; }

        [Required(ErrorMessage = "ErrorMissingPrice")]
        [RegularExpression(@"^-?\d+(\.\d{1,2})?$", ErrorMessage = "ErrorPriceNotANumber")]
        [Range(0.01, double.MaxValue, ErrorMessage = "ErrorPriceNotGreaterThanZero")]
        public string Price { get; set; }
    }
}
