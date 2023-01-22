
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Models;

namespace EstoreTests
{
    public class ShippingCalculatorTests
    {
        private static readonly Address InSameCity = new Address
        {
            City = "Dallas",
            Country = "USA",
            Street = "At home"
        };

        private static List<Item> GenerateCartItems() =>
            new List<Item> {
            new Item { Quantity = 1 },
            new Item { Quantity = 2 },
            new Item { Quantity = 3 }
            };

        private static Cart DefaultCart() =>
            new Cart
            {
                ShippingAddress = InSameCity,
                Items = GenerateCartItems(),
            };

        [Fact]
        public void CalculateShippingCost_EmptyCart()
        {
            var cart = new Cart
            {
                ShippingAddress = InSameCity,
            };
            IShippingCalculator calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);
            Assert.Equal(0.00, costs);
        }

        [Fact]
        public void CalculateShippingCost_SingleItemCart()
        {
            var cart = new Cart
            {
                ShippingAddress = InSameCity,
                Items = new List<Item> { new Item { Quantity = 2 } }
            };
            IShippingCalculator calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);
            Assert.Equal(2.00, costs);
        }

       

        [Theory]
        [InlineData(ShippingMethod.Standard, 6.0)]
        [InlineData(ShippingMethod.Expedited, 7.2)]
        [InlineData(ShippingMethod.Priority, 12.0)]
        [InlineData(ShippingMethod.Express, 15.0)]
        public void CalculateShipingCosts_ToStandardCustomerInSameCity(ShippingMethod method, double expectedCosts) {            
            var cart = DefaultCart();
            cart.ShippingMethod = method;

            var calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);

            Assert.Equal(expectedCosts, costs, 6);
        }

        [Theory]
        [InlineData(ShippingMethod.Standard, 6.0)]
        [InlineData(ShippingMethod.Expedited, 6.0)]
        [InlineData(ShippingMethod.Priority, 6.0)]
        [InlineData(ShippingMethod.Express, 15.0)]
        public void CalculateShipingCosts_ToPremiumCustomerInSameCity(ShippingMethod method, double expectedCosts)
        {
            var cart = DefaultCart();
            cart.CustomerType = CustomerType.Premium;
            cart.ShippingMethod = method;

            var calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);

            Assert.Equal(expectedCosts, costs, 6);
        }

        [Fact]
        public void CalculateShippingCost_ToSameCountry()
        {
            var cart = new Cart
            {
                ShippingAddress = new Address
                {
                    City = "New York",
                    Country = "USA",
                    Street = "Wall Street 11"
                },
                Items = GenerateCartItems(),
                ShippingMethod = ShippingMethod.Standard,
            };
            IShippingCalculator calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);
            Assert.Equal(12.0, costs);
        }

        [Fact]
        public void CalculateShippingCost_TotherCountry()
        {
            var cart = new Cart
            {
                ShippingAddress = new Address
                {
                    City = "London",
                    Country = "GB",
                    Street = "Downing Street 10"
                },
                Items = GenerateCartItems(),
                ShippingMethod = ShippingMethod.Standard,
            };
            IShippingCalculator calculator = new ShippingCalculator();

            var costs = calculator.CalculateShippingCost(cart);
            Assert.Equal(90.0, costs);
        }



    }
}
