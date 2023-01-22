using ShoppingCartService.BusinessLogic.Validation;
using ShoppingCartService.Models;

namespace EstoreTests
{
    public class AddressValidatorTests
    {
        [Fact]
        public void IsValid_NullAddress_Fails()
        {
            var validator = new AddressValidator();
            bool result = validator.IsValid(null);
            Assert.False(result);
        }

        [Fact]
        public void IsValid_Empty_Address_Fails()
        {
            var address = new Address();
            var validator = new AddressValidator();

            var result = validator.IsValid(address);
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ProperAddress_Success()       
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