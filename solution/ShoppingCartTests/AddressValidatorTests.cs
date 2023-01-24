using ShoppingCartService.BusinessLogic.Validation;
using ShoppingCartService.Models;

namespace ShoppingCartTests
{
    public class AddressValidatorTests
    {
        [Fact]
        public void Null_is_not_a_valid_address()
        {
            var validator = new AddressValidator();
            bool result = validator.IsValid(null);
            Assert.False(result);
        }

        [Fact]
        public void Address_without_street_is_invalid()
        {
            var address = new Address
            {
                City = "London",
                Country = "England"
            };
            var validator = new AddressValidator();

            var result = validator.IsValid(address);
            Assert.False(result);
        }

        [Fact]
        public void Address_without_city_is_invalid()
        {
            var address = new Address
            {
                Street = "Baker Street 221B",
                Country = "England"
            };
            var validator = new AddressValidator();

            var result = validator.IsValid(address);
            Assert.False(result);
        }


        [Fact]
        public void Address_without_country_is_invalid()
        {
            var address = new Address
            {
                City = "London",
                Street = "Baker Street 221B",
            };
            var validator = new AddressValidator();

            var result = validator.IsValid(address);
            Assert.False(result);
        }

        [Fact]
        public void A_complete_address_is_valid()       
        {
            var address = new Address
            {
                City = "London",
                Street = "Baker Street 221B",
                Country = "England"
            };
            var validator = new AddressValidator();

            var result = validator.IsValid(address);
            Assert.True(result);
        }
    }
}