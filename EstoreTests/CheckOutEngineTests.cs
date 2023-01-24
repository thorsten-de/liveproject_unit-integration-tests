
using AutoMapper;
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Mapping;
using ShoppingCartService.Models;

namespace EstoreTests
{
    public class CheckOutEngineTests
    {
        private IMapper _mapper =
            new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
            .CreateMapper();

        private class MockShippingCalculator : IShippingCalculator
        {
            private double costs;
            public MockShippingCalculator(double costs)
            {
                this.costs = costs;
            }

            public double CalculateShippingCost(Cart cart) => costs;
        }

        private static Cart DefaultCart() =>
            new Cart
            {
                Items = new List<Item> {
                    new Item { Quantity = 1, Price = 100.0 },
                    new Item { Quantity = 2, Price = 10.0},
                    new Item { Quantity = 3, Price = 1.0 }
                }
            };

        private ICheckOutEngine CreateEngine() => new CheckOutEngine(
                new MockShippingCalculator(0.0),
                _mapper);


        [Fact]
        public void An_empty_cart_has_no_total_and_discount()
        {
            var engine = CreateEngine();
            var cart = new Cart { };

            var result = engine.CalculateTotals(cart);

            Assert.Equal(0.00, result.Total);
            Assert.Equal(0.0, result.CustomerDiscount);
        }

        [Fact]
        public void Standard_customers_get_no_discount()
        {
            var engine = CreateEngine();
            var cart = DefaultCart();

            var result = engine.CalculateTotals(cart);

            Assert.Equal(123.00, result.Total);
            Assert.Equal(0.0, result.CustomerDiscount);
        }

        [Fact]
        public void Premium_customers_get_their_discount()
        {
            var engine = CreateEngine();
            var cart = DefaultCart();
            cart.CustomerType = CustomerType.Premium;

            var result = engine.CalculateTotals(cart);

            Assert.Equal(110.70, result.Total);
            Assert.Equal(10.0, result.CustomerDiscount);
        }

        [Fact]
        public void Checkout_considers_shipping_costs()
        {
            var engine = new CheckOutEngine(new MockShippingCalculator(4.0), _mapper);
            var cart = DefaultCart();

            var result = engine.CalculateTotals(cart);

            Assert.Equal(127.00, result.Total);
            Assert.Equal(4.0, result.ShippingCost);
        }
    }
}
