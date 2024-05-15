using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CouponService: ICouponService
    {
        private readonly IBaseService baseService;

        public CouponService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDTO?> CreateCouponAsync(CouponDTO coupon)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.POST,
                Data = coupon,
                Url = SD.CouponAPIBase+"/api/coupon"
            });
        }

        public async Task<ResponseDTO?> DeleteCouponByIdAsync(int id)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.DELETE,
                Url = SD.CouponAPIBase + $"/api/coupon/{id}"
            });
        }

        public async Task<ResponseDTO?> GetAllCouponAsync()
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.GET,
                Url = SD.CouponAPIBase + $"/api/coupon"
            });
        }

        public async Task<ResponseDTO?> GetCouponByCodeAsync(string code)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.GET,
                Url = SD.CouponAPIBase + $"/api/coupon/GetByCode/{code}"
            });
        }

        public async Task<ResponseDTO?> GetCouponByIdAsync(int id)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.GET,
                Url = SD.CouponAPIBase + $"/api/coupon/{id}"
            });
        }

        public async Task<ResponseDTO?> UpdateCouponAsync(CouponDTO coupon)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.PUT,
                Data = coupon,
                Url = SD.CouponAPIBase + $"/api/coupon"
            });
        }
    }
}
