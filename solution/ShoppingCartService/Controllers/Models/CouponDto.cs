using System;
using System.ComponentModel.DataAnnotations;

namespace ShoppingCartService.Controllers.Models
{
    public record CouponDto
    {
        [Required]
        public string Id { get; init; }

        [Required] 
        public string Type { get; init; }

        public DateTime? ExpiresOn { get; init; }
    }

    public record CreateCouponDto
    {
        [Required]
        public string Type { get; init; }

        public double Amount { get; init; }

        public DateTime? ExpiresOn { get; init; }
    }
}
