using ShoppingCartService.BusinessLogic.Exceptions;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.Controllers.Models;

namespace ShoppingCartService.BusinessLogic
{
    public class CouponEngine
    {
        public object CalculateDiscount(CheckoutDto checkoutDto, Coupon? coupon)
        {
            if (coupon is null)
                return 0.00;

            if (coupon.Amount < 0)
               throw new InvalidCouponException("A coupon amount must not be negative.");
 
            if (coupon.Amount > checkoutDto.Total + checkoutDto.ShippingCost)
                throw new InvalidCouponException("Coupon must not exceed total cart amount");

            return coupon.Amount;
        }
    }
}
