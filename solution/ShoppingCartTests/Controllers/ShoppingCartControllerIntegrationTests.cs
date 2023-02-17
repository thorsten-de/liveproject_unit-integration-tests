using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.BusinessLogic.Validation;
using ShoppingCartService.Controllers;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Mapping;
using ShoppingCartService.Models;
using ShoppingCartTests.Builders;
using ShoppingCartTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using static ShoppingCartTests.Builders.ItemBuilder;

namespace ShoppingCartTests.Controllers
{
    [Collection("Dockerized MongoDB collection")]
    public class ShoppingCartControllerIntegrationTests : IntegrationTestBase
    {
        private IMapper _mapper;
        private const string UnknownID = "123456789012345678901234";

        public ShoppingCartControllerIntegrationTests(DockerMongoFixture fixture) 
            : base(fixture)
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()))
                .CreateMapper();
        }

     
        [Fact]
        public void GetAll_HasOneCart_returnAllShoppingCartsInformation()
        {
            var cart = CreateTestCart();
            var repository = InitializeRepository(cart);

            var sut = CreateShoppingCartController(repository);
            var result = sut.GetAll();
            ShoppingCartDto expected = DtoFromCart(cart);

            Assert.Equal(expected, result.Single());
        }

      

        [Fact]
        public void FindById_HashOneCartWithSameId_returnAllShoppingCartsInformation()
        {
            var cart = CreateTestCart();
            var repository = InitializeRepository(cart, CreateTestCart());
            string cartId = cart.Id;

            var sut = CreateShoppingCartController(repository);
            var result = sut.FindById(cartId);

            var expected = DtoFromCart(cart);
            Assert.Equal(expected, result.Value);
        }

        [Fact]
        public void FindById_ItemNotFound_returnNotFound()
        {
            var repository = InitializeRepository(CreateTestCart());

            var sut = CreateShoppingCartController(repository);
            var result = sut.FindById(UnknownID);

            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void CalculateTotals_ShoppingCartNotFound_returnNotFound()
        {
            var repository = InitializeRepository();
            var sut = CreateShoppingCartController(repository);

            var actual = sut.CalculateTotals(UnknownID);

            Assert.IsType<NotFoundResult>(actual.Result);
        }

        [Fact]
        public void CalculateTotals_ShoppingCartFound_returnTotals()
        {
            var cart = CreateTestCart();
            var repository = InitializeRepository(cart);
            var sut = CreateShoppingCartController(repository);

            CheckoutDto? actual = sut.CalculateTotals(cart.Id).Value;
            Assert.NotNull(actual);
        }

        [Fact]
        public void Create_ValidData_SaveShoppingCartToDB()
        {
            var address = new AddressBuilder().Build();
            var items = new[] { CreateItemDto() };
            var createCartDto = BuildCreateCartDto(items, address);

            var repository = InitializeRepository();
            var sut = CreateShoppingCartController(repository);

            var result = sut.Create(createCartDto);
            Assert.IsType<CreatedAtRouteResult>(result.Result);
            var dbCarts = repository.FindAll();
            Assert.Single(dbCarts);


            /*
            var actual = result.Value;
            Assert.NotNull(actual);
            Assert.Equal("customer-1", actual.CustomerId);
            Assert.Equal(CustomerType.Standard, actual.CustomerType);
            Assert.Equal(ShippingMethod.Standard, actual.ShippingMethod);
            Assert.Equal(address, actual.ShippingAddress);
            Assert.True(items.SequenceEqual(actual.Items));*/
        }

        [Fact]
        public void Create_DuplicateItem_ReturnBadRequest()
        {
            var createCartDto = BuildCreateCartDto(
                new[] { CreateItemDto(), CreateItemDto() },
                new AddressBuilder().Build());

            var repository = InitializeRepository();
            var sut = CreateShoppingCartController(repository);

            var actual = sut.Create(createCartDto);
            Assert.IsType<BadRequestResult>(actual.Result);

        }

        public static List<object[]> InvalidAddresses() => new()
        {
            new object[] {null},
            new object[] {new AddressBuilder().WithoutCountry().Build()},
            new object[] {new AddressBuilder().WithoutCity().Build()},
            new object[] {new AddressBuilder().WithoutStreet().Build()},
        };

        [Theory]
        [MemberData(nameof(InvalidAddresses))]
        public void Create_InvalidAddress_ReturnBadRequest(Address address)
        {
            var createCartDto = BuildCreateCartDto(
              new[] { CreateItemDto() },
              address);

            var repository = InitializeRepository();
            var sut = CreateShoppingCartController(repository);

            var actual = sut.Create(createCartDto);
            Assert.IsType<BadRequestResult>(actual.Result);
        }

        [Fact]
        public void Delete_ValidData_RemoveShoppingCartToDB()
        {
            var createCartDto = BuildCreateCartDto(
                new[] { CreateItemDto() },
                new AddressBuilder().Build());
            
            var repository = InitializeRepository();
            var sut = CreateShoppingCartController(repository);

            sut.Create(createCartDto);            
            string id = repository.FindAll().Single().Id;
            var actual = sut.DeleteCart(id);

            Assert.IsType<NoContentResult>(actual);
            var dbCarts = repository.FindAll();
            Assert.Empty(dbCarts);
        }

        private ShoppingCartController CreateShoppingCartController(ShoppingCartRepository repository)
        {
            return new(new ShoppingCartManager(repository, new AddressValidator(), _mapper,
                new CheckOutEngine(new ShippingCalculator(), _mapper)),
                new NullLogger<ShoppingCartController>());
        }

        private Cart CreateTestCart() =>
         new CartBuilder()
             .WithCustomerId("1")
             .WithItems(new List<Item> { CreateItem() })
             .Build();

        private static ShoppingCartDto DtoFromCart(Cart cart)
        {
            return new ShoppingCartDto
            {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                CustomerType = cart.CustomerType,
                ShippingAddress = cart.ShippingAddress,
                ShippingMethod = cart.ShippingMethod,
                Items = cart.Items.Select(cartItem =>
                    new ItemDto(ProductId: cartItem.ProductId,
                    ProductName: cartItem.ProductName,
                    Price: cartItem.Price,
                    Quantity: cartItem.Quantity
                    )).ToList()
            };
        }

        private static CreateCartDto BuildCreateCartDto(IEnumerable<ItemDto> items, Address address)
        {
            return new CreateCartDto()
            {
                Customer = new CustomerDto()
                {
                    Id = "customer-1",
                    Address = address,
                    CustomerType = CustomerType.Standard
                },
                Items = items
            };
        }

    }
}
