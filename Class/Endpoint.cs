using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FortniteLauncher.Class
{
    public static class Endpoint
    {
        public static string Token { get; set; } = "https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token";
        public static string DeviceAuth { get; set; } = "https://account-public-service-prod.ol.epicgames.com/account/api/public/account/accountID/deviceAuth";

        public static string ExchangeCode { get; set; } = "https://account-public-service-prod.ol.epicgames.com/account/api/oauth/exchange";
    }
}
