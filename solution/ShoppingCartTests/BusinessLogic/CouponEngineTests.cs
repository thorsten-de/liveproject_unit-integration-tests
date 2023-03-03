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
            Coupon coupon = new() { Amount = amount };

            var discount = engine.CalculateDiscount(checkoutDto, coupon);
            
            Assert.Equal(amount, discount);
        }

        [Fact]
        public void A_coupon_cannot_discount_more_than_total_cart_amount()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto(total: 10);
            CouponEngine engine = new CouponEngine();
            Coupon coupon = new() { Amount = 100 };

            Assert.Throws<InvalidCouponException>(() => { 
                var discount = engine.CalculateDiscount(checkoutDto, coupon);
            });
        }

        [Fact]
        public void A_coupon_must_not_have_a_negative_amount()
        {
            CheckoutDto checkoutDto = CreateCheckoutDto();
            CouponEngine engine = new CouponEngine();
            Coupon coupon = new() { Amount = -10 };

            Assert.Throws<InvalidCouponException>(() => {
                var discount = engine.CalculateDiscount(checkoutDto, coupon);
            });
        }
    }
}
