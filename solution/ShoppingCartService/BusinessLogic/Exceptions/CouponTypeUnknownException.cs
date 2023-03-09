using System;
using System.Runtime.Serialization;

namespace ShoppingCartService.BusinessLogic.Exceptions
{
    [Serializable]
    public class CouponTypeUnknownException : Exception
    {
        public CouponTypeUnknownException()
        {
        }

        public CouponTypeUnknownException(string message) : base(message)
        {
        }

        public CouponTypeUnknownException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CouponTypeUnknownException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}