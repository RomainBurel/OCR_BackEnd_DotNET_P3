using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Moq;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using System.IO;
using System.Linq;
using Xunit;

namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        private readonly DbContextOptions<P3Referential> _options;
        private readonly P3Referential _dbContext;
        private readonly ProductService _productService;
        private readonly Cart _cart;
        private ProductViewModel _newProduct;

        public ProductServiceTests()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            string connectionString = configuration.GetConnectionString("P3Referential");
            DbContextOptionsBuilder<P3Referential> optionsBuilder = new DbContextOptionsBuilder<P3Referential>();
            optionsBuilder.UseSqlServer(connectionString);
            _options = optionsBuilder.Options;
            _dbContext = new P3Referential(_options, null);
            _cart = new Cart();
            ProductRepository productRepository = new ProductRepository(_dbContext);
            OrderRepository orderRepository = new OrderRepository(_dbContext);
            Mock<IStringLocalizer<ProductService>> mockLocalizer = new Mock<IStringLocalizer<ProductService>>();
            _productService = new ProductService(_cart, productRepository, orderRepository, mockLocalizer.Object);
        }

        private void InitProduct()
        {
            _newProduct = new ProductViewModel
            {
                Name = "Produit pour test",
                Price = "9.99",
                Stock = "10"
            };
        }

        [Fact]
        public void ProductShouldBeAvailableInDbAfterBeingCreatedByAdmin()
        {
            // Arrange
            InitProduct();

            // Act
            _productService.SaveProduct(_newProduct);

            // Assert
            Product productInDb = _dbContext.Product.FirstOrDefault(p => p.Name == _newProduct.Name);
            Assert.NotNull(productInDb);
            Assert.Equal(9.99, productInDb.Price);
            Assert.Equal(10, productInDb.Quantity);

            // Clean DB
            _productService.DeleteProduct(productInDb.Id);
        }

        [Fact]
        public void ProductShouldNotBeAvailableInDbAfterBeingDeletedByAdmin()
        {
            // Arrange
            InitProduct();
            _productService.SaveProduct(_newProduct);

            Product productInDb = _dbContext.Product.FirstOrDefault(p => p.Name == _newProduct.Name);
            Assert.NotNull(productInDb);

            // Act
            _productService.DeleteProduct(productInDb.Id);

            // Assert
            Product deletedProduct = _dbContext.Product.FirstOrDefault(p => p.Id == productInDb.Id);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public void ProductAddedToCartShouldNotBeAvailableInDbAfterBeingDeletedByAdmin()
        {
            // Arrange
            InitProduct();
            _productService.SaveProduct(_newProduct);
            Product productInDb = _dbContext.Product.FirstOrDefault(p => p.Name == _newProduct.Name);
            Assert.NotNull(productInDb);
            _cart.AddItem(productInDb, 1);

            // Act
            _productService.DeleteProduct(productInDb.Id);

            // Assert
            Product deletedProduct = _dbContext.Product.FirstOrDefault(p => p.Id == productInDb.Id);
            Assert.Null(deletedProduct);
            CartLine deletedCartLine = _cart.Lines.FirstOrDefault(l => l.Product.Id == productInDb.Id);
            Assert.Null(deletedCartLine);
        }

        [Fact]
        public void ProductAddedToCartShouldBeUpdatedAfterCallingUpdateProductQuantities()
        {
            // Arrange
            InitProduct();
            _productService.SaveProduct(_newProduct);
            Product addedProduct = _dbContext.Product.FirstOrDefault(p => p.Name == _newProduct.Name);
            Assert.NotNull(addedProduct);
            int productIntialQty = addedProduct.Quantity;
            _cart.AddItem(addedProduct, 2);

            // Act
            _productService.UpdateProductQuantities();

            // Assert
            Assert.NotNull(addedProduct);
            Assert.Equal(productIntialQty - 2, addedProduct.Quantity);
            _productService.DeleteProduct(addedProduct.Id);
        }

        [Fact]
        public void ProductInfosShouldBeAccessibleWithGetProductById()
        {
            // Arrange
            InitProduct();
            _productService.SaveProduct(_newProduct);

            Product addedProduct = _dbContext.Product.FirstOrDefault(p => p.Name == _newProduct.Name);
            Assert.NotNull(addedProduct);

            // Act
            Product productInfo = _productService.GetProductById(addedProduct.Id);

            // Assert
            Assert.NotNull(productInfo);
            Assert.Equal("Produit pour test", productInfo.Name);
            Assert.Equal(9.99, productInfo.Price);
            Assert.Equal(10, productInfo.Quantity);

            // Clean DB
            _productService.DeleteProduct(productInfo.Id);
        }
    }
}