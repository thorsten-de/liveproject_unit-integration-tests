using AutoMapper.Configuration.Annotations;
using ShoppingCartService.BusinessLogic.Exceptions;
using ShoppingCartService.Controllers.Models;
using System;

namespace ShoppingCartService.BusinessLogic.Models
{
    public abstract class Coupon
    {
        public static Coupon WithAmount(double amount) =>
            new AmountCoupon(amount);

        public static Coupon WithPercentage(double value)
            => new PercentageCoupon(value);

        /// <summary>
        /// Validates given data for the concrete coupon type and calculates
        /// the discount
        /// </summary>
        /// <param name="checkoutDto">checkout data</param>
        /// <returns>calculated discount</returns>
        public abstract double CalculateDiscount(CheckoutDto checkoutDto);

        /// <summary>
        /// Implementation for fixed amount dicounts
        /// </summary>
        private class AmountCoupon : Coupon
        {
            private readonly double _amount;

            public AmountCoupon(double amount)
            {
                _amount = amount;
            }

            public override double CalculateDiscount(CheckoutDto checkoutDto)
            {
                if (_amount < 0)
                    throw new InvalidCouponException("A coupon amount must not be negative.");

                if (_amount > checkoutDto.Total + checkoutDto.ShippingCost)
                    throw new InvalidCouponException("Coupon amount must not exceed total cart amount.");

                return _amount;
            }
        }

        /// <summary>
        /// Implementation for coupons that discount relative to total costs
        /// </summary>
        private class PercentageCoupon : Coupon
        {
            private readonly double _value;

            public PercentageCoupon(double value)
            {
                _value = value;
            }

            public override double CalculateDiscount(CheckoutDto checkoutDto)
            {
                if (_value < 0)
                    throw new InvalidCouponException("A coupon must not have a negative percentage.");

                if (_value > 100)
                    throw new InvalidCouponException("A coupon cannot discount more than 100 percent.");

                return checkoutDto.Total * _value / 100.0;
            }
        }
    }

}
