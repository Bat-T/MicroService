using Mango.Web.Models;
using Mango.Web.Service.IService;
using Mango.Web.Utility;

namespace Mango.Web.Service
{
    public class CartService : ICartService
    {
        private readonly IBaseService baseService;

        public CartService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<ResponseDTO?> ApplyCoupon(CartDto coupon)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.POST,
                Data = coupon,
                Url = SD.ShoppingCartAPIBase + "/api/cart/ApplyCoupon"
            }, true);
        }

        public async Task<ResponseDTO?> EmailCouponRequest(CartDto coupon)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.POST,
                Data = coupon,
                Url = SD.ShoppingCartAPIBase + "/api/cart/EmailCartRequest"
            }, true);
        }

        public async Task<ResponseDTO?> CartUpsert(CartDto coupon)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.POST,
                Data = coupon,
                Url = SD.ShoppingCartAPIBase + "/api/cart/CartUpsert"
            }, true);
        }

        public async Task<ResponseDTO?> UpsertCartAsync(CartDto cartDto)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = SD.ApiType.POST,
                Data = cartDto,
                Url = SD.ShoppingCartAPIBase + "/api/cart/CartUpsert"
            });
        }
        public async Task<ResponseDTO?> GetCartbyUserId(string userId)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.GET,
                Url = SD.ShoppingCartAPIBase + $"/api/cart/GetCart/{userId}"
            }, true);
        }

        public async Task<ResponseDTO?> RemoveCart(int couponId)
        {
            return await baseService.SendAsync(new RequestDTO()
            {
                ApiType = Utility.SD.ApiType.GET,
                Data = couponId,
                Url = SD.ShoppingCartAPIBase + "/api/cart/RemoveCart"
            }, true);
        }
    }
}
