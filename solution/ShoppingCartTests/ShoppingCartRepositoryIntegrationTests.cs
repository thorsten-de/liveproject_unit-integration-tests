﻿using MongoDB.Driver;
using ShoppingCartService.Config;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ShoppingCartTests
{
    [Collection("Dockerized MongoDB collection")]
    public class ShoppingCartRepositoryIntegrationTests : IDisposable
    {
        private IShoppingCartDatabaseSettings _databaseSettings;
        private IMongoCollection<Cart> _cartsCollection;
        private const string Unknown_ID = "507f191e810c19729de860ea";

        public ShoppingCartRepositoryIntegrationTests(DockerMongoFixture dockerMongoFixture)
        {
            _databaseSettings = dockerMongoFixture.GetDatabaseSettings();
            SetupCartsCollection();
        }

        private ShoppingCartRepository InitializeRepository(params Cart[] carts)
        {
            var repo = new ShoppingCartRepository(_databaseSettings);
            foreach (var cart in carts)
            {
                repo.Create(cart);
            }
            return repo;
        }

        private void SetupCartsCollection()
        {
            var client = new MongoClient(_databaseSettings.ConnectionString);
            var database = client.GetDatabase(_databaseSettings.DatabaseName);
            _cartsCollection = database.GetCollection<Cart>(_databaseSettings.CollectionName);
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
        }

        [Fact]
        public void GetById_CartNotFound_ReturnNull()
        {
            ShoppingCartRepository sut = InitializeRepository(new[] {
                new CartBuilder().Build(),
            });

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
            ShoppingCartRepository sut = InitializeRepository(new[] { cart });

            var foundCart = sut.FindById(cart.Id);
            foundCart.CustomerId = "updated-customer";
            sut.Update(foundCart.Id, foundCart);

            Cart result = sut.FindById(cart.Id);

            Assert.NotNull(result);
            Assert.Equal(cart.Id, result.Id);
            Assert.Equal("updated-customer", result.CustomerId);
        }

        [Fact]
        public void Remove_CartFound_RemoveFromDb()
        {
            var cart = new CartBuilder().Build();
            ShoppingCartRepository sut = InitializeRepository(new[] { cart });

            var foundCart = sut.FindById(cart.Id);
            sut.Remove(foundCart);
            var result = sut.FindAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public void RemoveById_CartFound_RemoveFromDb()
        {
            var cart = new CartBuilder().Build();
            ShoppingCartRepository sut = InitializeRepository(new[] { cart });

            sut.Remove(cart.Id);
            var result = sut.FindAll();

            Assert.NotNull(result);
            Assert.Empty(result);
        }

        public void Dispose()
        {
            new MongoClient(_databaseSettings.ConnectionString)
                .DropDatabase(_databaseSettings.DatabaseName);
        }
    }
}
