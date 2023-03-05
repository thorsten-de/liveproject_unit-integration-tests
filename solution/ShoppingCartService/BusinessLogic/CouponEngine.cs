using ShoppingCartService.BusinessLogic.Exceptions;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.Controllers.Models;
using System;

namespace ShoppingCartService.BusinessLogic
{
    public class CouponEngine
    {
        public double CalculateDiscount(CheckoutDto checkoutDto, Coupon? coupon, DateTime? onDate = null)
        {
            if (coupon is null)
                return 0.00;

            return coupon.CalculateDiscount(checkoutDto, onDate);

        }
    }
}
