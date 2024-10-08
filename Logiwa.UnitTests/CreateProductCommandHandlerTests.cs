using FluentValidation;
using Logiwa.Business.CQRS.Commands.Products;
using Logiwa.Common.Exceptions;
using Logiwa.Common.Utils;
using Logiwa.Domain.Entities;
using Logiwa.Infrastructure.DbContexts;
using Logiwa.Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Xunit;

namespace Logiwa.UnitTests
{
    public class CreateProductCommandHandlerTests
    {
        private readonly Mock<IGenericWriteRepository<BaseDbContext>> _mockWriteRepository;
        private readonly Mock<ILogger<CreateProductCommandHandler>> _mockLogger;
        private readonly CreateProductCommandHandler _handler;

        public CreateProductCommandHandlerTests()
        {
            _mockWriteRepository = new Mock<IGenericWriteRepository<BaseDbContext>>();
            _mockLogger = new Mock<ILogger<CreateProductCommandHandler>>();

            _handler = new CreateProductCommandHandler(_mockWriteRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task ShouldThrowExceptionWhenStockCodeLengthGreaterThanMaxCharacter()
        {
            string longStockCode = StringBuilderUtils.GenerateRandomString(201);

            var constructionException = Record.Exception(() => new CreateProductCommand(longStockCode, "New Product Description", 1, 10));

            if (constructionException == null)
            {
                var validCommand = new CreateProductCommand("ValidStockCode", "New Product Description", 1, 10);

                await Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(validCommand, CancellationToken.None)
                );
            }

            Assert.NotNull(constructionException);
            Assert.IsType<ValidationException>(constructionException);
        }

        [Fact]
        public async Task ShouldThrowExceptionWhenCategoryNotFound()
        {
            var notExistsCategoryId = 3;
            var existsCategoryId = 2;

            var command = new CreateProductCommand("New Product", "New Product Description", notExistsCategoryId, 10);

            _mockWriteRepository.Setup(repo => repo.GetAll<Product>()).Returns(Enumerable.Empty<Product>().AsQueryable().BuildMock());

            var categories = new List<Category> { new Category("Existing Category", "Existing Category Description", 1) };

            PropertySetter.SetPropertyValue(categories.First(), nameof(Category.Id), existsCategoryId);

            _mockWriteRepository.Setup(x => x.GetAll<Category>()).Returns(categories.ToList().AsQueryable().BuildMock());

            await Assert.ThrowsAsync<ResourceNotFoundException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task ShouldThrowExceptionWhenCreatedProductAlreadyExistsWithSameStockCode()
        {
            var existsStockCode = "Exists Product";
            var existsProductDescription = "Exists Product Description";
            var existsCategoryId = 2;
            var stockQuantity = 5;

            var categories = new List<Category> { new Category("Existing Category", "Existing Category Description", 1) };

            PropertySetter.SetPropertyValue(categories.First(), nameof(Category.Id), existsCategoryId);

            _mockWriteRepository.Setup(x => x.GetAll<Category>()).Returns(categories.AsQueryable().BuildMock());

            var products = new List<Product>() { new Product(existsStockCode, existsProductDescription, stockQuantity, categories.First()) };

            _mockWriteRepository.Setup(repo => repo.GetAll<Product>()).Returns(products.AsQueryable().BuildMock());

            var command = new CreateProductCommand(existsStockCode, existsProductDescription, existsCategoryId, stockQuantity);

            await Assert.ThrowsAsync<AlreadyExistsException>(async () => await _handler.Handle(command, CancellationToken.None));
        }

        [Fact]
        public async Task ShouldCreateProductWhenCommandIsValid()
        {
            var command = new CreateProductCommand("New Product", "New Product Description", 1, 10);

            _mockWriteRepository.Setup(repo => repo.GetAll<Product>()).Returns(Enumerable.Empty<Product>().AsQueryable().BuildMock());

            var categories = new List<Category> { new Category("Existing Category", "Existing Category Description", 1) };

            PropertySetter.SetPropertyValue(categories.First(), nameof(Category.Id), 1);

            _mockWriteRepository.Setup(x => x.GetAll<Category>()).Returns(categories.ToList().AsQueryable().BuildMock());

            var result = await _handler.Handle(command, CancellationToken.None);

            _mockWriteRepository.Verify(x => x.AddAsync(It.IsAny<Product>(), CancellationToken.None, false), Times.Once);
        }
    }
}
