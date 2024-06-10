using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System.Linq;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class CartTests
    {
        private readonly Product _product1;
        private readonly Product _product2;
        private readonly Product _product3;
        private readonly Cart _cart;

        public CartTests()
        {
            // Arrange
            _product1 = new Product { Id = 1, Name = "Product 1", Price = 0.5, Quantity = 5 };
            _product2 = new Product { Id = 2, Name = "Product 2", Price = 10d, Quantity = 10 };
            _product3 = new Product { Id = 3, Name = "Product 3", Price = 20d, Quantity = 500 };
            _cart = new Cart();
        }

        private void InitCart()
        {
            _cart.AddItem(_product1, 1);
            _cart.AddItem(_product2, 5);
            _cart.AddItem(_product3, 10);
        }

        [Fact]
        public void AddItem()
        {
            // Act
            InitCart();

            // Assert
            Assert.NotEmpty(_cart.Lines);
            Assert.Equal(3, _cart.Lines.Count());
            Assert.Equal(5, _cart.Lines.FirstOrDefault(p => p.Product.Id == _product2.Id).Quantity);
        }

        [Fact]
        public void AddTwiceSameItem()
        {
            // Act
            InitCart();
            _cart.AddItem(_product1, 1);

            // Assert
            Assert.Equal(3, _cart.Lines.Count());
            Assert.Equal(2, _cart.Lines.FirstOrDefault(p => p.Product.Id == _product1.Id).Quantity);
        }

        [Fact]
        public void RemoveLine()
        {
            // Act
            InitCart();
            _cart.RemoveLine(_product2);

            // Assert
            Assert.Equal(2, _cart.Lines.Count());
            Assert.Null(_cart.Lines.FirstOrDefault(p => p.Product.Id == _product2.Id));
        }

        [Fact]
        public void Clear()
        {
            // Act
            InitCart();
            _cart.Clear();

            // Assert
            Assert.Empty(_cart.Lines);
        }

        [Fact]
        public void GetTotalValue()
        {
            // Act
            InitCart();

            // Assert
            Assert.Equal(250.5, _cart.GetTotalValue());
        }

        [Fact]
        public void GetAverageValue()
        {
            // Act
            InitCart();

            // Assert
            Assert.Equal(15.66, _cart.GetAverageValue(), 0.01);
        }
    }
}
