using MongoDB.Driver;
using ShoppingCartService.Config;
using ShoppingCartService.DataAccess.Entities;
using System;

namespace ShoppingCartService.DataAccess
{
    public class CouponRepository
    {
        private readonly IMongoCollection<Coupon> _coupons;

        public CouponRepository(CouponDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _coupons = database.GetCollection<Coupon>(settings.CollectionName);
        }

        public Coupon Create(Coupon coupon)
        {
            _coupons.InsertOne(coupon);
            return coupon;
        }

        public Coupon FindById(string id) =>
            _coupons.Find(x => x.Id == id).FirstOrDefault();

        public void Remove(string id) => _coupons.DeleteOne(x => x.Id == id);

        public void Remove(Coupon coupon) => Remove(coupon.Id);
    }
}
