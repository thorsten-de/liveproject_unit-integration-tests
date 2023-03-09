﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ShoppingCartService.Config;
using ShoppingCartService.Controllers;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartTests.Fixtures;
using ShoppingCartService.BusinessLogic.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartTests.Controllers
{
    [Collection("Dockerized MongoDB collection")]
    public class CouponControllerIntegrationTests : IntegrationTestBase
    {
        CouponDatabaseSettings _couponDB;
        CouponRepository _repo;
       
        public CouponControllerIntegrationTests(DockerMongoFixture fixture) : base(fixture)
        {
            _couponDB = fixture.GetCouponDatabaseSettings();
            _repo = new CouponRepository(_couponDB);
        }

        [Theory]
        [InlineData(Coupon.AmountCouponType, 20.0, null)]
        [InlineData(Coupon.PercentCouponType, 10.0, null)]
        [InlineData(Coupon.FreeShippingCouponType, double.NaN, null)]
        [InlineData(Coupon.FreeShippingCouponType, 10.0, null)]
        [InlineData(Coupon.FreeShippingCouponType, 10.0, "2025-01-01")]
        public void Create_coupon_from_valid_data(string type, double amount, string? expiresOn)
        {
            var sut = new CouponController(_repo, _mapper);
            DateTime? expiresOnDate = expiresOn is null ? null : DateTime.Parse(expiresOn);

            var result = sut.Create(
                new CreateCouponDto { Type = type, Amount = amount, ExpiresOn = expiresOnDate  }
                );

            Assert.IsType<CreatedAtRouteResult>(result);
            var value = ((CreatedAtRouteResult)result).Value as CouponDto;
            Assert.NotNull(value);
            Assert.Equal(type, value.Type);
            Assert.Equal(expiresOnDate, value.ExpiresOn);
        }

        [Fact]
        public void Create_unknwon_coupon_types_throws_exception()
        {
            var sut = new CouponController(_repo, _mapper);

            Assert.Throws<CouponTypeUnknownException>(() =>
            {
                var result = sut.Create(
                    new CreateCouponDto { Type = "UnkwnownCouponType" }
                    );
            });
        }




        public void Dispose()
        {
            new MongoClient(_couponDB.ConnectionString)
                .DropDatabase(_couponDB.DatabaseName);
        }
    }
}
