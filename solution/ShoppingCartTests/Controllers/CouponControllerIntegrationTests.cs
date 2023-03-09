using Microsoft.AspNetCore.Mvc;
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
using ShoppingCartService.BusinessLogic;

namespace ShoppingCartTests.Controllers
{
    [Collection("Dockerized MongoDB collection")]
    public class CouponControllerIntegrationTests : IntegrationTestBase
    {
        CouponDatabaseSettings _couponDB;
        ICouponRepository _repo;
        CouponManager _couponManager;
       
        public CouponControllerIntegrationTests(DockerMongoFixture fixture) : base(fixture)
        {
            _couponDB = fixture.GetCouponDatabaseSettings();
            _repo = new CouponRepository(_couponDB);
            _couponManager = new CouponManager(_repo, _mapper);
        }

        [Theory]
        [InlineData(Coupon.AmountCouponType, 20.0, null)]
        [InlineData(Coupon.PercentCouponType, 10.0, null)]
        [InlineData(Coupon.FreeShippingCouponType, double.NaN, null)]
        [InlineData(Coupon.FreeShippingCouponType, 10.0, null)]
        [InlineData(Coupon.FreeShippingCouponType, 10.0, "2025-01-01")]
        public void Create_coupon_from_valid_data(string type, double amount, string? expiresOn)
        {
            var sut = new CouponController(_couponManager);
            DateTime? expiresOnDate = expiresOn is null ? null : DateTime.Parse(expiresOn);

            var result = sut.Create(
                new CreateCouponDto { Type = type, Amount = amount, ExpiresOn = expiresOnDate  }
                );

            Assert.IsType<CreatedAtRouteResult>(result);
            var value = ((CreatedAtRouteResult)result).Value as CouponDto;
            Assert.NotNull(value);
            Assert.Equal(type, value.Type);
            Assert.Equal(expiresOnDate, value.ExpiresOn);

            Assert.NotNull(_repo.FindById(value.Id));
        }

        [Fact]
        public void Create_unknwon_coupon_types_throws_exception()
        {
            var sut = new CouponController(_couponManager);

            Assert.Throws<CouponTypeUnknownException>(() =>
            {
                var result = sut.Create(
                    new CreateCouponDto { Type = "UnkwnownCouponType" }
                    );
            });
        }

        [Fact]
        public void FindById_returns_a_known_coupon()
        {
            var coupon = _repo.Create(Coupon.WithFreeShipping());
            var sut = new CouponController(_couponManager);

            var result = sut.FindById(coupon.Id).Value;

            Assert.NotNull(result);
            Assert.Equal(coupon.Id, result.Id);
            Assert.Equal(coupon.Type, result.Type);
        }

        [Fact]
        public void FindById_on_uknowwn_id_returns_not_found()
        {
            var sut = new CouponController(_couponManager);

            var result = sut.FindById(UnknownID).Result;

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_known_coupon_from_DB()
        {
            var coupon = _repo.Create(Coupon.WithFreeShipping());
            var sut = new CouponController(_couponManager);

            var result = sut.Delete(coupon.Id);
            
            Assert.IsType<NoContentResult>(result);
            var value = _repo.FindById(coupon.Id);
            Assert.Null(value);
        }

        public void Dispose()
        {
            new MongoClient(_couponDB.ConnectionString)
                .DropDatabase(_couponDB.DatabaseName);
        }
    }
}
