using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICouponService
    {
        public Task<ResponseDTO?> GetCouponByIdAsync(int id);
        public Task<ResponseDTO?> GetAllCouponAsync();
        public Task<ResponseDTO?> GetCouponByCodeAsync(string code);
        public Task<ResponseDTO?> CreateCouponAsync(CouponDTO coupon);
        public Task<ResponseDTO?> UpdateCouponAsync(CouponDTO coupon);
        public Task<ResponseDTO?> DeleteCouponByIdAsync(int id);

    }
}
