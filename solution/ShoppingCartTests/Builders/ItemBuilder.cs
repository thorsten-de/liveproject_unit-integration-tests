using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartTests.Builders
{
    internal static class ItemBuilder
    {
        public static Item CreateItem(
            string productId = "product-1",
            string productName = "Superb Product",
            uint quantity = 1,
            double price = 1.0
            ) =>
            new Item {
                Price = price,
                ProductId = productId,
                ProductName = productName,
                Quantity = quantity
            };

        public static ItemDto CreateItemDto(
          string productId = "product-1",
          string productName = "Superb Product",
          double price = 1.0,
          uint quantity = 1
          ) =>
          new ItemDto(productId, productName, price, quantity);

    }
}
