using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartTests
{
    internal class CartBuilder
    {
        private Address _shippingAddress = new AddressBuilder().Build();
        private List<Item> _items = new();
        private CustomerType _customerType = CustomerType.Standard;
        private ShippingMethod _shippingMethod = ShippingMethod.Standard;
        private string _id = "cart-1";
        private string _customerId = "customer-1";

        public CartBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public CartBuilder WithCustomerId(string customerId)
        {
            _customerId = customerId;
            return this;
        }

        public CartBuilder WithShippingAddress(Address address)
        {
            _shippingAddress = address;
            return this;
        }

        public CartBuilder WithItems(List<Item> items)
        {
            _items = items;
            return this;
        }

        public CartBuilder WithManyItems()
        {
            _items = new List<Item> {
                    new Item { Quantity = 1, Price = 100.0 },
                    new Item { Quantity = 2, Price = 10.0},
                    new Item { Quantity = 3, Price = 1.0 }
                };
            return this;
        }


        public CartBuilder WithoutItems()
        {
            _items = new List<Item> { };
            return this;
        }

        public CartBuilder WithShippingMethod(ShippingMethod shippingMethod)
        {
            _shippingMethod = shippingMethod;
            return this;
        }

        public CartBuilder ForPremiumCustomer()
        {
            _customerType = CustomerType.Premium;
            return this;
        }

        public Cart Build()
        {
            return new Cart
            {
                CustomerId= _customerId,
                ShippingAddress = _shippingAddress,
                ShippingMethod = _shippingMethod,
                CustomerType = _customerType,
                Items = _items
            };
        }

       
    }
}
