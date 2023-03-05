using ShoppingCartService.Config;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartTests.DataAccess
{
    [Collection("Dockerized MongoDB collection")]
    public class CouponRepositoryIntegrationTests : IntegrationTestBase
    {
        private const string Unknown_ID = "507f191e810c19729de860ea";
        
        protected CouponDatabaseSettings _dbSettings;

        public CouponRepositoryIntegrationTests(DockerMongoFixture fixture)
            : base(fixture)
        {
            _dbSettings = fixture.GetCouponDatabaseSettings();
        }

        [Fact]
        public void Create_adds_a_new_coupon_to_DB()
        {
            var coupon = Coupon.WithAmount(20).Expire(new DateTime(2023, 01, 01));
            CouponRepository target = new CouponRepository(_dbSettings);

            Coupon result = target.Create(coupon);

            Assert.NotNull(result);
            Assert.NotNull(result.Id);
        }

        [Fact]
        public void FindById_returns_null_if_coupon_is_not_found()
        {
            CouponRepository target = new CouponRepository(_dbSettings);

            var result = target.FindById(Unknown_ID);

            Assert.Null(result);
        }

        [Fact]
        public void FindById_returns_coupon_if_exists()
        {
        
            Coupon coupon = Coupon.WithPercentage(10).Expire(new DateTime(2023, 1, 1));
            CouponRepository target = new CouponRepository(_dbSettings);
            coupon = target.Create(coupon);

            var result = target.FindById(coupon.Id);

            Assert.NotNull(result);
            Assert.Equal(coupon.Id, result.Id);
            Assert.Equal(coupon.ExpiresOn, result.ExpiresOn);
        }

        [Fact]
        public void RemoveById_removes_existing_coupon_from_db() { 

            Coupon coupon = Coupon.WithFreeShipping();
            CouponRepository target = new CouponRepository(_dbSettings);
            coupon = target.Create(coupon);

            target.Remove(coupon.Id);

            Assert.Null(target.FindById(coupon.Id));
        }

        [Fact]
        public void Remove_removes_existing_coupon_from_db()
        {

            Coupon coupon = Coupon.WithFreeShipping();
            CouponRepository target = new CouponRepository(_dbSettings);
            coupon = target.Create(coupon);

            target.Remove(coupon);

            Assert.Null(target.FindById(coupon.Id));
        }
    }
}
