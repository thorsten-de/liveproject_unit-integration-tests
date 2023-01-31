using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingCartService.Models;

namespace ShoppingCartTests
{
    internal class AddressBuilder
    {
        private string _street = "At home";
        private string _city = "Dallas";
        private string _country = "USA";


        public AddressBuilder()
        {
        }

        public AddressBuilder WithoutStreet() {
            _street = string.Empty;
            return this; 
        }

        public AddressBuilder WithoutCity() {
            _city = string.Empty;
            return this; 
        }

        public AddressBuilder WithoutCountry()
        {
            _country = string.Empty;
            return this;
        }

        public AddressBuilder InDifferentCity()
        {
            _city = "New York";
            return this;
        }

        public AddressBuilder InDifferentCountry()
        {
            _country = "England";
            _city = "London";
            return this;
        }


        public Address Build() => new Address
        {
            Street = _street,
            City = _city,
            Country = _country
        };
    }
}
