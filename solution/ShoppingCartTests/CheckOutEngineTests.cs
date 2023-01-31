
using AutoMapper;
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Mapping;
using ShoppingCartService.Models;

namespace ShoppingCartTests
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
    
        private ICheckOutEngine CreateEngine(double shippingCosts = 0.00)
            => new CheckOutEngine(
                new MockShippingCalculator(shippingCosts),
                _mapper);


        [Fact]
        public void An_empty_cart_has_no_total_and_discount()
        {
            var engine = CreateEngine();
            var cart = new CartBuilder()
                .WithoutItems()
                .Build();

            var result = engine.CalculateTotals(cart);

            Assert.Equal(0.00, result.Total);
            Assert.Equal(0.0, result.CustomerDiscount);
        }

        [Fact]
        public void Standard_customers_get_no_discount()
        {
            var engine = CreateEngine();
            var cart = new CartBuilder()
                .WithManyItems()
                .Build();

            var result = engine.CalculateTotals(cart);

            Assert.Equal(123.00, result.Total);
            Assert.Equal(0.0, result.CustomerDiscount);
        }

        [Fact]
        public void Premium_customers_get_their_discount()
        {
            var engine = CreateEngine();
            var cart = new CartBuilder()
                .WithManyItems()
                .ForPremiumCustomer()
                .Build();

            var result = engine.CalculateTotals(cart);

            Assert.Equal(110.70, result.Total);
            Assert.Equal(10.0, result.CustomerDiscount);
        }

        [Fact]
        public void Checkout_considers_shipping_costs()
        {
            var engine = CreateEngine(shippingCosts: 4.0);
            var cart = new CartBuilder()
                .WithManyItems()
                .Build();

            var result = engine.CalculateTotals(cart);

            Assert.Equal(127.00, result.Total);
            Assert.Equal(4.0, result.ShippingCost);
        }
    }
}
