using Mango.Web.Models;

namespace Mango.Web.Service.IService
{
    public interface ICartService
    {
        public Task<ResponseDTO?> GetCartbyUserId(string userId);
        public Task<ResponseDTO?> CartUpsert(CartDto coupon);
        public Task<ResponseDTO?> RemoveCart(int couponCode);
        public Task<ResponseDTO?> ApplyCoupon(CartDto coupon);
        public Task<ResponseDTO?> EmailCartRequest(CartDto coupon);

    }
}
