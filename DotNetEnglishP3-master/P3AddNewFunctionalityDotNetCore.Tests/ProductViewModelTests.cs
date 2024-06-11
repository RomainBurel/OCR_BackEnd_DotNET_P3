using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductViewModelTests
    {
        [Theory]
        [InlineData("", "15.5", "5", "Name", "ErrorMissingName")]
        [InlineData(null, "15.5", "5", "Name", "ErrorMissingName")]
        [InlineData("", "", "", "Name", "ErrorMissingName")]
        [InlineData("Product 1", "", "5", "Price", "ErrorMissingPrice")]
        [InlineData("Product 1", null, "5", "Price", "ErrorMissingPrice")]
        [InlineData("", "", "", "Price", "ErrorMissingPrice")]
        [InlineData("Product 1", "A", "5", "Price", "ErrorPriceNotANumber")]
        [InlineData("Product 1", "5_5", "5", "Price", "ErrorPriceNotANumber")]
        [InlineData("Product 1", "0", "5", "Price", "ErrorPriceNotGreaterThanZero")]
        [InlineData("Product 1", "-2.3", "5", "Price", "ErrorPriceNotGreaterThanZero")]
        [InlineData("Product 1", "15.5", "", "Stock", "ErrorMissingStock")]
        [InlineData("Product 1", "15.5", null, "Stock", "ErrorMissingStock")]
        [InlineData("", "", "", "Stock", "ErrorMissingStock")]
        [InlineData("Product 1", "15.5", "A", "Stock", "ErrorStockNotAnInteger")]
        [InlineData("Product 1", "15.5", "2.5", "Stock", "ErrorStockNotAnInteger")]
        [InlineData("Product 1", "15.5", "0", "Stock", "ErrorStockNotGreaterThanZero")]
        [InlineData("Product 1", "15.5", "-1", "Stock", "ErrorStockNotGreaterThanZero")]
        public void PropertyShouldHaveValidationError(string name, string price, string stock, string testedProperty, string expectedErrorName)
        {
            // Arrange
            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Name = name;
            viewModel.Price = price;
            viewModel.Stock = stock;

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(viewModel, new ValidationContext(viewModel), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains(testedProperty));
            Assert.Contains(validationResults, vr => vr.ErrorMessage == expectedErrorName);
        }

        [Theory]
        [InlineData("Product 1", "15.5", "5", "Name")]
        [InlineData("Product 1", "", "", "Name")]
        [InlineData("Product 1", "15.5", "5", "Price")]
        [InlineData("", "15.5", "", "Price")]
        [InlineData("Product 1", "15.5", "5", "Stock")]
        [InlineData("", "", "5", "Stock")]
        public void PropertyShouldHaveNoValidationError(string name, string price, string stock, string testedProperty)
        {
            // Arrange
            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Name = name;
            viewModel.Price = price;
            viewModel.Stock = stock;

            // Act
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(viewModel, new ValidationContext(viewModel), validationResults, true);

            // Assert
            Assert.DoesNotContain(validationResults, vr => vr.MemberNames.Contains(testedProperty));
        }

        [Fact]
        public void SeveralPropertiesShouldHaveValidationError()
        {
            // Arrange
            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Name = "";
            viewModel.Price = "";
            viewModel.Stock = "";

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(viewModel, new ValidationContext(viewModel), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Equal(3, validationResults.Count);
        }

        [Fact]
        public void NoPropertyShouldHaveValidationError()
        {
            // Arrange
            ProductViewModel viewModel = new ProductViewModel();
            viewModel.Name = "Product 1";
            viewModel.Price = "15.5";
            viewModel.Stock = "10";

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(viewModel, new ValidationContext(viewModel), validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}
