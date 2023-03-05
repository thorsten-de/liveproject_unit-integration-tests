using ShoppingCartService.BusinessLogic;
using ShoppingCartService.BusinessLogic.Exceptions;
using ShoppingCartService.BusinessLogic.Models;
using ShoppingCartService.Controllers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace ShoppingCartTests.BusinessLogic
{
    public class CouponEngineTests
    {
        private CheckoutDto CreateCheckoutDto(ShoppingCartDto? cart = null,
            double total = 0.0, double shipping = 0.0, double discount = 0.0) =>
            new CheckoutDto(cart ?? new ShoppingCartDto(), shipping, discount, total);

        [Fact]
        public void An_empty_coupon_has_a_zero_discount()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto();
            CouponEngine engine = new CouponEngine();

            var discount = engine.CalculateDiscount(checkoutDto, null);

            Assert.Equal(0.00, discount);
        }

        [Theory]
        [InlineData(10.0)]
        [InlineData(50.0)]
        public void A_coupon_discounts_the_specified_amount(double amount)
        {
            CheckoutDto checkoutDto = CreateCheckoutDto(total: 100);
            CouponEngine engine = new CouponEngine();
            Coupon coupon = Coupon.WithAmount(amount);

            var discount = engine.CalculateDiscount(checkoutDto, coupon);
            
            Assert.Equal(amount, discount);
        }

        [Fact]
        public void A_coupon_cannot_discount_more_than_total_cart_amount()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto(total: 10);
            CouponEngine engine = new CouponEngine();
            Coupon coupon = Coupon.WithAmount(100);

            Assert.Throws<InvalidCouponException>(() => { 
                var discount = engine.CalculateDiscount(checkoutDto, coupon);
            });
        }

        [Fact]
        public void A_coupon_must_not_have_a_negative_amount()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto();
            CouponEngine engine = new CouponEngine();
            Coupon coupon = Coupon.WithAmount(-100);

            Assert.Throws<InvalidCouponException>(() => {
                var discount = engine.CalculateDiscount(checkoutDto, coupon);
            });
        }

        [Theory]
        [InlineData(10.0)]
        [InlineData(15.0)]
        public void Coupons_can_be_percentage_of_total_cost(double percentage)
        {
            CheckoutDto checkoutDto = CreateCheckoutDto(total: 100.0);
            CouponEngine engine = new();
            Coupon coupon = Coupon.WithPercentage(percentage);

            var discount = engine.CalculateDiscount(checkoutDto, coupon);
            Assert.Equal(percentage, discount);
        }

        [Fact]
        public void Coupons_cannot_discount_more_than_100_percent()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto();
            CouponEngine engine = new CouponEngine();
            Coupon coupon = Coupon.WithPercentage(105);

            Assert.Throws<InvalidCouponException>(() => {
                var discount = engine.CalculateDiscount(checkoutDto, coupon);
            });
        }

        [Fact]
        public void Coupons_must_not_have_a_negative_percentage()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto();
            CouponEngine engine = new CouponEngine();
            Coupon coupon = Coupon.WithPercentage(-10);

            Assert.Throws<InvalidCouponException>(() => {
                var discount = engine.CalculateDiscount(checkoutDto, coupon);
            });
        }

        // Shouldn't be a problem with my implementation...
        [Fact]
        public void CalculateDiscount_CouponOfTypePercentageAndHigherThanAmount_DoNotThrowInvalidCouponException()
        {
            var target = new CouponEngine();

            var actual = target.CalculateDiscount(
                CreateCheckoutDto(total: 10),
                Coupon.WithPercentage(50));

            Assert.Equal(5, actual);
        }

        [Fact]
        public void Free_shipping_couponts_discount_shipping_cost()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto(shipping: 10.0);
            CouponEngine engine = new();
            Coupon coupon = Coupon.WithFreeShipping();

            var discount = engine.CalculateDiscount(checkoutDto, coupon);
            Assert.Equal(10.0, discount);
        }

        [Fact]
        public void An_expired_coupon_throws_exception()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto(total: 100);
            CouponEngine engine = new();
            Coupon coupon = Coupon
                .WithAmount(10)
                .ExpiresOn(new DateTime(2023, 1, 1));


            Assert.Throws<CouponExdpiredException>(() =>
            {
                var discount = engine.CalculateDiscount(checkoutDto, coupon, 
                    onDate: new DateTime(2023, 3, 1));
            });
        }

        [Fact]
        public void Coupons_are_valid_before_expiry_date()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto(total: 100);
            CouponEngine engine = new();
            Coupon coupon = Coupon
                .WithAmount(10)
                .ExpiresOn(new DateTime(2024, 1, 1));

            var discount = engine.CalculateDiscount(checkoutDto, coupon,
                    onDate: new DateTime(2023, 3, 1));

            Assert.Equal(10, discount);
        }
    }
}
