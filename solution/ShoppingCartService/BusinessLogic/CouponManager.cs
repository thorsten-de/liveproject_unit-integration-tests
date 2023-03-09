using AutoMapper;
using ShoppingCartService.Controllers.Models;
using ShoppingCartService.DataAccess;
using ShoppingCartService.DataAccess.Entities;

namespace ShoppingCartService.BusinessLogic
{
    public class CouponManager
    {
        private readonly IMapper _mapper;
        private readonly ICouponRepository _couponRepository;

        public CouponManager(ICouponRepository couponRepository, IMapper mapper)
        {
            _mapper = mapper;
            _couponRepository = couponRepository;
        }

        public CouponDto Create(CreateCouponDto createCouponDto) { 
            var coupon = _mapper.Map<Coupon>(createCouponDto);
            _couponRepository.Create(coupon);
            return _mapper.Map<CouponDto>(coupon);
        }

        public CouponDto FindById(string id)
        {
            var coupon = _couponRepository.FindById(id);
            return _mapper.Map<CouponDto>(coupon);
        }

        public void DeleteById(string id) { 
            _couponRepository.Remove(id);
        }



    }
}
