namespace ShoppingCartService.BusinessLogic.Exceptions
{
    [System.Serializable]
    public class CouponExdpiredException : System.Exception
    {
        public CouponExdpiredException() { }
        public CouponExdpiredException(string message) : base(message) { }
        public CouponExdpiredException(string message, System.Exception inner) : base(message, inner) { }
        protected CouponExdpiredException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
