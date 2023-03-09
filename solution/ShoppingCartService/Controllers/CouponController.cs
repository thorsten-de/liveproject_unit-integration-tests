using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;

namespace ShoppingCartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly IMapper _mapper;
        private  readonly CouponRepository _couponRepository;

        public CouponController(CouponRepository couponRepository, IMapper mapper)
        {
            _mapper = mapper;
            _couponRepository = couponRepository;
        }

        public IActionResult Create([FromBody] CreateCouponDto createCoupon)
        {
            var coupon = _mapper.Map<Coupon>(createCoupon);
            _couponRepository.Create(coupon);

            var result = _mapper.Map<CouponDto>(coupon);
            return CreatedAtRoute("GetCoupon", new { id = result.Id }, result);
        }

        public ActionResult<CouponDto> FindById(string id)
        {
            var coupon = _couponRepository.FindById(id);
            if (coupon is null)
                return NotFound();

            return _mapper.Map<CouponDto>(coupon);
        }

        public IActionResult Delete(string id)
        {
            _couponRepository.Remove(id);
            return NoContent();
        }
    }
}
