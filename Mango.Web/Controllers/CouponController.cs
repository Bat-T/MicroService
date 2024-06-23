using Mango.Web.Models;
using Mango.Web.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Mango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponService _couponService;

        public CouponController(ICouponService couponService)
        {
            this._couponService = couponService;
        }

        public async Task<IActionResult> CouponIndex()
        {
            List<CouponDTO?> couponDTOs = [];

            var response = await _couponService.GetAllCouponAsync();

            if(response!= null && response.IsSuccess)
            {
                couponDTOs = JsonConvert.DeserializeObject<List<CouponDTO>>(Convert.ToString(response.Result));
                return View(couponDTOs);
            }
            TempData["error"] = response?.Message;
            return View();
        }

        public async Task<IActionResult> CreateCoupon()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles ="ADMIN")]
        public async Task<IActionResult> CreateCoupon(CouponDTO model)
        {
            

            if (ModelState.IsValid)
            {
                var response = await _couponService.CreateCouponAsync(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon Created";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return RedirectToAction("CreateCoupon");
        }

        public async Task<IActionResult> DeleteCoupon(int couponId)
        {
            var response = await _couponService.GetCouponByIdAsync(couponId);

            if (response != null && response.IsSuccess)
            {
                var model = JsonConvert.DeserializeObject<CouponDTO>(Convert.ToString(response.Result));
                return View(model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCoupon(CouponDTO model)
        {
            var response = await _couponService.DeleteCouponByIdAsync(model.CouponId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon Deleted";
                return RedirectToAction("CouponIndex");
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
    }
}
