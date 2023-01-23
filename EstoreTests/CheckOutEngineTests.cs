
using AutoMapper;
using ShoppingCartService.BusinessLogic;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Mapping;
using ShoppingCartService.Models;

namespace EstoreTests
{
    public class CheckOutEngineTests
    {
        public IMapper _mapper =
            new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>())
            .CreateMapper();

        private class MockShippingCalculator : IShippingCalculator
        {
            public double CalculateShippingCost(Cart cart) => 0.0;
        }

       private static List<Item> GenerateCartItems() =>
            new List<Item> {
            new Item { Quantity = 1, Price = 100.0 },
            new Item { Quantity = 2, Price = 10.0},
            new Item { Quantity = 3, Price = 1.0 }
            };

        private static Cart DefaultCart() =>
            new Cart
            {                
                Items = GenerateCartItems(),
            };

        [Fact]
        public void Totals_of_empty_cart_are_zero()
        {
            var cart = new Cart
            { };

            var engine = new CheckOutEngine(
                new MockShippingCalculator(),
                _mapper);

            var result = engine.CalculateTotals(cart);

            Assert.Equal(0.00, result.Total);
            Assert.Equal(0.0, result.CustomerDiscount);
        }


    }
}
