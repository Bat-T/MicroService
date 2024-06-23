using AutoMapper;
using Mango.Services.CouponAPI.Data;
using Mango.Services.CouponAPI.Models;
using Mango.Services.CouponAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    [Authorize]
    public class CouponsController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDTO _response;
        private readonly IMapper mapper;

        public CouponsController(AppDbContext db,IMapper _mapper)
        {
            _db = db;
            _response = new ResponseDTO();
            mapper = _mapper;

        }

        [HttpGet]
        public ResponseDTO Get()
        {
            try
            {
                IEnumerable<Coupon> coupons = _db.CouponDats.ToList();
                _response.Result = mapper.Map<IEnumerable<CouponDTO>>(coupons);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDTO Get(int id)
        {
            try
            {
                var coupon = _db.CouponDats.FirstOrDefault(c => c.CouponId == id);
                _response.Result = mapper.Map<CouponDTO>(coupon);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }


        [HttpGet("GetByCode/{code}")]
        public ResponseDTO GetByCode(string code)
        {
            try
            {
                var coupon = _db.CouponDats.FirstOrDefault(c => c.CouponCode.ToLower() == code.ToLower());
                _response.Result = mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPost]
        public ResponseDTO Post([FromBody] CouponDTO couponDTO)
        {
            try
            {
                var coupon = mapper.Map<Coupon>(couponDTO);
                _db.CouponDats.Add(coupon);
                _db.SaveChanges();
                _response.Result = mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpPut]
        public ResponseDTO Put([FromBody] CouponDTO couponDTO)
        {
            try
            {
                var coupon = mapper.Map<Coupon>(couponDTO);
                _db.CouponDats.Update(coupon);
                _db.SaveChanges();
                _response.Result = mapper.Map<CouponDTO>(coupon);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }

        [HttpDelete("{couponId:int}")]
        public ResponseDTO Delete(int couponId)
        {
            try
            {
                var coupon = _db.CouponDats.First(c => c.CouponId == couponId);
                _db.CouponDats.Remove(coupon);
                _db.SaveChanges();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }
            return _response;
        }
    }
}
