using MongoDB.Driver;
using ShoppingCartService.Config;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using ShoppingCartTests.Builders;

namespace ShoppingCartTests.DataAccess
{
    [Collection("Dockerized MongoDB collection")]
    public class ShoppingCartRepositoryIntegrationTests : IntegrationTestBase
    {
        private const string Unknown_ID = "507f191e810c19729de860ea";

        public ShoppingCartRepositoryIntegrationTests(DockerMongoFixture dockerMongoFixture)
            : base(dockerMongoFixture)
        {
        }

        [Fact]
        public void FindAll_NoCartsInDB_ReturnEmptyList()
        {
            ShoppingCartRepository sut = InitializeRepository();

            var result = sut.FindAll();
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void FindAll_HasTwoCartsInDB_ReturnAllCarts()
        {
            var carts = new[]            {
                new CartBuilder().Build(),
                new CartBuilder().Build()
            };
            ShoppingCartRepository sut = InitializeRepository(carts);

            var result = sut.FindAll();

            Assert.NotEmpty(result);
            Assert.True(carts.SequenceEqual(result));
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public void GetById_hasThreeCartsInDB_returnReturnOnlyCartWithCorrectId()
        {
            var carts = new[]
            {
                new CartBuilder().Build(),
                new CartBuilder().Build(),
                new CartBuilder().Build()
            };
            ShoppingCartRepository sut = InitializeRepository(carts);
            var validId = carts[1].Id;

            Cart result = sut.FindById(validId);
            Assert.NotNull(result);
            Assert.Equal(validId, result.Id);
            Assert.Equal(carts[1], result);
        }

        [Fact]
        public void GetById_CartNotFound_ReturnNull()
        {
            ShoppingCartRepository sut = InitializeRepository(new CartBuilder().Build());

            Cart result = sut.FindById(Unknown_ID);

            Assert.Null(result);
        }

        [Fact]
        public void Update_CartNotFound_DoNotFail()
        {
            ShoppingCartRepository sut = InitializeRepository();

            sut.Update(Unknown_ID, new CartBuilder().Build());
        }

        [Fact]
        public void Update_CartFound_UpdateValue()
        {
            var cart = new CartBuilder()
                .WithCustomerId("customer-01")
                .Build();
            ShoppingCartRepository sut = InitializeRepository(cart);

            cart.CustomerId = "updated-customer";
            sut.Update(cart.Id, cart);

            Cart result = sut.FindById(cart.Id);

            Assert.NotNull(result);
            Assert.Equal(cart.Id, result.Id);
            Assert.Equal("updated-customer", result.CustomerId);
        }

        [Fact]
        public void Remove_CartFound_RemoveFromDb()
        {
            var cart = new CartBuilder().Build();
            ShoppingCartRepository sut = InitializeRepository(cart);

            sut.Remove(cart);
            var result = sut.FindAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void RemoveById_CartFound_RemoveFromDb()
        {
            var cart = new CartBuilder().Build();
            ShoppingCartRepository sut = InitializeRepository(cart);

            sut.Remove(cart.Id);
            var result = sut.FindAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
