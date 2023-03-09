using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;
using ShoppingCartService.BusinessLogic;

namespace ShoppingCartService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly CouponManager _couponManager;

        public CouponController(CouponManager couponManager)
        {
            _couponManager = couponManager;
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateCouponDto createCoupon)
        {
            CouponDto result = _couponManager.Create(createCoupon);

            return CreatedAtRoute("GetCoupon", new { id = result.Id }, result);
        }

        public ActionResult<CouponDto> FindById(string id)
        {
            var coupon = _couponManager.FindById(id);
            if (coupon is null)
                return NotFound();

            return coupon;
        }

        [HttpDelete]
        public IActionResult Delete(string id)
        {
            _couponManager.DeleteById(id);
            return NoContent();
        }
    }
}
