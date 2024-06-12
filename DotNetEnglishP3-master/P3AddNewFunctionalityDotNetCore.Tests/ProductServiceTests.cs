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

        [Fact]
        public void SaveProductInDB()
        {
            // Arrange
            ProductViewModel newProduct = new ProductViewModel
            {
                Name = "Produit pour test",
                Price = "9.99",
                Stock = "10"
            };

            // Act
            _productService.SaveProduct(newProduct);

            // Assert
            Product productInDb = _dbContext.Product.FirstOrDefault(p => p.Name == newProduct.Name);
            Assert.NotNull(productInDb);
            Assert.Equal(9.99, productInDb.Price);
            Assert.Equal(10, productInDb.Quantity);

            // Clean DB
            _productService.DeleteProduct(productInDb.Id);
        }

        [Fact]
        public void RemoveProductFromDB()
        {
            // Arrange
            ProductViewModel newProduct = new ProductViewModel
            {
                Name = "Produit pour test",
                Price = "99.50",
                Stock = "5"
            };
            _productService.SaveProduct(newProduct);

            Product productInDb = _dbContext.Product.FirstOrDefault(p => p.Name == newProduct.Name);
            Assert.NotNull(productInDb);

            // Act
            _productService.DeleteProduct(productInDb.Id);

            // Assert
            Product deletedProduct = _dbContext.Product.FirstOrDefault(p => p.Id == productInDb.Id);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public void RemoveProductAddedToACartFromDB()
        {
            // Arrange
            ProductViewModel newProduct = new ProductViewModel
            {
                Name = "Produit pour test",
                Price = "99.50",
                Stock = "5"
            };
            _productService.SaveProduct(newProduct);
            Product productInDb = _dbContext.Product.FirstOrDefault(p => p.Name == newProduct.Name);
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
        public void UpdateProductQuantities()
        {
            // Arrange
            ProductViewModel newProduct = new ProductViewModel
            {
                Name = "Produit pour test",
                Price = "10.00",
                Stock = "5"
            };
            _productService.SaveProduct(newProduct);

            Product addedProduct = _dbContext.Product.FirstOrDefault(p => p.Name == newProduct.Name);
            Assert.NotNull(addedProduct);

            _cart.AddItem(new Product { Id = addedProduct.Id, Name = addedProduct.Name }, 2);

            // Act
            _productService.UpdateProductQuantities();

            // Assert
            Product updatedProduct = _dbContext.Product.FirstOrDefault(p => p.Name == newProduct.Name);
            Assert.NotNull(updatedProduct);
            Assert.Equal(3, updatedProduct.Quantity);
            _productService.DeleteProduct(updatedProduct.Id);
        }

        [Fact]
        public void GetProductById()
        {
            // Arrange
            ProductViewModel productViewModelSelectTest = new ProductViewModel
            {
                Name = "Test product infos",
                Price = "545.45",
                Stock = "130"
            };
            _productService.SaveProduct(productViewModelSelectTest);

            Product addedProduct = _dbContext.Product.FirstOrDefault(p => p.Name == productViewModelSelectTest.Name);
            Assert.NotNull(addedProduct);

            // Act
            Product productInfo = _productService.GetProductById(addedProduct.Id);

            // Assert
            Assert.NotNull(productInfo);
            Assert.Equal("Test product infos", productInfo.Name);
            Assert.Equal(545.45, productInfo.Price);
            Assert.Equal(130, productInfo.Quantity);

            // Clean DB
            _productService.DeleteProduct(productInfo.Id);
        }
    }
}