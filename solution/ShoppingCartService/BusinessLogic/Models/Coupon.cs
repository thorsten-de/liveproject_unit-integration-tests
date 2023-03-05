﻿using AutoMapper.Configuration.Annotations;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.BusinessLogic.Exceptions;
using ShoppingCartService.Controllers.Models;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCartService.BusinessLogic.Models
{
    /// <summary>
    /// A coupon can calculate a discount based on the Checkout data 
    /// with several strategies
    /// </summary>
    public abstract class Coupon
    {
        public static Coupon WithAmount(double amount) =>
            new AmountCoupon(amount);

        public static Coupon WithPercentage(double value)
            => new PercentageCoupon(value);

        public static Coupon WithFreeShipping()
            => new FreeShippingCoupon();

        public Coupon ExpiresOn(DateTime expiresOn)
        {
            _expiresOn = expiresOn;
            return this;
        }

        private DateTime? _expiresOn;

        /// <summary>
        /// Validates given data for the concrete coupon type and calculates
        /// the discount
        /// </summary>
        /// <param name="checkoutDto">checkout data</param>
        /// <returns>calculated discount</returns>
        public double CalculateDiscount(CheckoutDto checkoutDto, DateTime? onDate = null)
        {
            onDate ??= DateTime.Now;
            if (_expiresOn < onDate)
                throw new CouponExdpiredException($"The coupon has expired on {_expiresOn}.");

            Validate(checkoutDto);
            return Calculate(checkoutDto);
        }

        protected virtual void Validate(CheckoutDto checkoutDto) { }

        protected abstract double Calculate(CheckoutDto checkoutDto);

        #region Coupon implementations

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

            protected override void Validate(CheckoutDto checkoutDto)
            {
                if (_amount < 0)
                    throw new InvalidCouponException("A coupon amount must not be negative.");

                if (_amount > checkoutDto.Total + checkoutDto.ShippingCost)
                    throw new InvalidCouponException("Coupon amount must not exceed total cart amount.");
            }

            protected override double Calculate(CheckoutDto checkoutDto) => _amount;
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

            protected override void Validate(CheckoutDto checkoutDto)
            {
                if (_value < 0)
                    throw new InvalidCouponException("A coupon must not have a negative percentage.");

                if (_value > 100)
                    throw new InvalidCouponException("A coupon cannot discount more than 100 percent.");
            }

            protected override double Calculate(CheckoutDto checkoutDto) =>
                checkoutDto.Total * _value / 100.0;
        }

        /// <summary>
        /// Implementation for free shipping coupons
        /// </summary>
        private class FreeShippingCoupon : Coupon
        {
            protected override double Calculate(CheckoutDto checkoutDto) 
                => checkoutDto.ShippingCost;
        }

        #endregion
    }

}
