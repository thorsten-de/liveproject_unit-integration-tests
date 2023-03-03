namespace ShoppingCartService.BusinessLogic.Exceptions
{
    public class InvalidCouponException : System.Exception
    {
        public InvalidCouponException() { }

        public InvalidCouponException(string message) : base(message) { }

        public InvalidCouponException(string message, System.Exception inner)
            : base(message, inner) { }

        protected InvalidCouponException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}

