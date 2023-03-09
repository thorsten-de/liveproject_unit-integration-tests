using ShoppingCartService.DataAccess.Entities;

namespace ShoppingCartService.DataAccess
{
    public interface ICouponRepository
    {
        Coupon Create(Coupon coupon);
        Coupon FindById(string id);
        void Remove(Coupon coupon);
        void Remove(string id);
    }
}