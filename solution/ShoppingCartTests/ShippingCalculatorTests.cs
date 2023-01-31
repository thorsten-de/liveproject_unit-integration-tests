
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Models;

namespace ShoppingCartTests
{
    public class ShippingCalculatorTests
    {
 
        [Fact]
        public void An_empty_cart_has_no_costs()
        {
            var cart = new CartBuilder()
                .WithoutItems()
                .Build();
            IShippingCalculator calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);

            Assert.Equal(0.00, costs);
        }

        [Fact]
        public void Ship_a_single_item_cart_to_same_city_with_standards()
        {
            var cart = new CartBuilder()
                .WithItems(new List<Item> { new Item { Quantity = 2 } })
                .Build();

            IShippingCalculator calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);

            Assert.Equal(2.00, costs);
        }

        [Theory]
        [InlineData(ShippingMethod.Standard, 6.0)]
        [InlineData(ShippingMethod.Expedited, 7.2)]
        [InlineData(ShippingMethod.Priority, 12.0)]
        [InlineData(ShippingMethod.Express, 15.0)]
        public void It_applies_shipping_method_for_standard_customers(ShippingMethod method, double expectedCosts) {
            var cart = new CartBuilder()
                .WithManyItems()
                .WithShippingMethod(method)
                .Build();
            var calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);

            Assert.Equal(expectedCosts, costs, 6);
        }

        [Theory]
        [InlineData(ShippingMethod.Standard, 6.0)]
        [InlineData(ShippingMethod.Expedited, 6.0)]
        [InlineData(ShippingMethod.Priority, 6.0)]
        [InlineData(ShippingMethod.Express, 15.0)]
        public void Ita_applies_shipping_method_for_premium_customers(ShippingMethod method, double expectedCosts)
        {
            var cart = new CartBuilder()
                .WithManyItems()
                .WithShippingMethod(method)
                .ForPremiumCustomer()
                .Build();

            var calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);

            Assert.Equal(expectedCosts, costs, 6);
        }

        [Fact]
        public void Ship_to_different_city_in_same_country()
        {
            var cart = new CartBuilder()
                .WithManyItems()
                .WithShippingAddress(new AddressBuilder()
                    .InDifferentCity()
                    .Build())
                .Build();
            IShippingCalculator calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);

            Assert.Equal(12.0, costs);
        }

        [Fact]
        public void Ship_to_another_country()
        {
            var cart = new CartBuilder()
                .WithManyItems()
                .WithShippingAddress(new AddressBuilder()
                    .InDifferentCountry()
                    .Build())
                .Build();
            IShippingCalculator calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);

            Assert.Equal(90.0, costs);
        }
    }
}
