﻿namespace Mango.Web.Utility
{
    public static class SD
    {
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string RoleAdmin = "ADMIN";
        public static string RoleCustomer = "CUSTOMER";
        public static string TokenCookie = "JwtToken";
        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE
        }
    }
}
