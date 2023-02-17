using AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.BusinessLogic.Validation;
using ShoppingCartService.Controllers;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Mapping;
using ShoppingCartTests.Builders;
using ShoppingCartTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static ShoppingCartTests.Builders.ItemBuilder;

namespace ShoppingCartTests.Controllers
{
    [Collection("Dockerized MongoDB collection")]
    public class ShoppingCartControllerIntegrationTests : IntegrationTestBase
    {
        private IMapper _mapper;

        public ShoppingCartControllerIntegrationTests(DockerMongoFixture fixture) 
            : base(fixture)
        {
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()))
                .CreateMapper();
        }


        [Fact]
        public void GetAll_HasOneCart_returnAllShoppingCartsInformation()
        {
            var cart = new CartBuilder()
                .WithId(null)
                .WithCustomerId("1")
                .WithItems(new List<Item> { CreateItem() })
                .Build();
            var cartItem = cart.Items[0];

            var repository = InitializeRepository(cart);

            var sut = CreateShoppingCartController(repository);
            var result = sut.GetAll();

            var expected = new ShoppingCartDto {
                Id = cart.Id,
                CustomerId = cart.CustomerId,
                CustomerType= cart.CustomerType,
                ShippingAddress= cart.ShippingAddress,
                ShippingMethod= cart.ShippingMethod,
                Items = new List<ItemDto>
                {
                    new(ProductId: cartItem.ProductId, 
                    ProductName: cartItem.ProductName,
                    Price: cartItem.Price,
                    Quantity: cartItem.Quantity
                    )
                }
            };

            Assert.Equal(expected, result.Single());
        }

        private ShoppingCartController CreateShoppingCartController(ShoppingCartRepository repository)
        {
            return new(new ShoppingCartManager(repository, new AddressValidator(), _mapper,
                new CheckOutEngine(new ShippingCalculator(), _mapper)),
                new NullLogger<ShoppingCartController>());
        }
    }
}
