using ShoppingCartService.BusinessLogic;
using ShoppingCartService.Controllers.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartTests.BusinessLogic
{
    public class CouponEngineTests
    {
        [Fact]
        public void An_empty_coupon_has_a_zero_discount()
        {
            CheckoutDto checkoutDto = new CheckoutDto(new ShoppingCartDto(), 0, 0, 0);
            CouponEngine engine = new CouponEngine();

            var discount = engine.CalculateDiscount(checkoutDto, null);

            Assert.Equal(0.00, discount);
        }
    }
}
