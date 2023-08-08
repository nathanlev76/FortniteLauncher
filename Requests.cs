using System;
using RestSharp;
using System.Runtime.InteropServices;
using FortniteLauncher.Class;
using System.ComponentModel;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Windows;

namespace CandyLauncher
{
    class Requests
    {
        public static Loggers logger = new Loggers();
        private static void sendRequestLog(string url, RestRequest request, RestResponse response)
        {
            if(Globals.debug == "true")
            {
                Console.WriteLine($"[{logger.Green((request.Method.ToString()))}] {logger.Aqua(response.StatusDescription.ToString())} ({logger.Pink(response.StatusCode.ToString())}): {logger.Aqua(url)}");
            }
            
        }
        public static string GetTokenByExchange(string exchange_code, string EpicClient=Globals.IOS_CLIENT)
        {
            string url = Endpoint.Token;
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.Post);
            request.AddHeader("Authorization", EpicClient);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "exchange_code");
            request.AddParameter("exchange_code", exchange_code);

            RestResponse response = client.Execute(request);
            sendRequestLog(url, request, response);
            if((int)response.StatusCode != 200) 
            {
                if (Globals.debug == "false")
                {
                    Console.Clear();
                }
                Console.WriteLine(logger.Red("Erreur ! Exchange Code invalide ou expiré !"));
                return "Error";

            }
            else
            {
                return response.Content.ToString();
            }
            
        }

        public static RestResponse GetTokenByDeviceAuth(string account_id, string device_id, string secret)
        {
            string url = Endpoint.Token;
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.Post);
            request.AddHeader("Authorization", Globals.IOS_CLIENT);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request.AddParameter("grant_type", "device_auth");
            request.AddParameter("account_id", account_id);
            request.AddParameter("device_id", device_id);
            request.AddParameter("secret", secret);


            RestResponse response = client.Execute(request);
            sendRequestLog(url, request, response);
            return response;
        }

        public static string GenerateExchangeCode(string access_token)
        {
            string url = Endpoint.ExchangeCode;
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.Get);
            string token = $"bearer {access_token}";
            request.AddHeader("Authorization", token);
            request.AddHeader("Content-Type", "application/json");
            RestResponse response = client.Execute(request);
            sendRequestLog(url, request, response);

            JObject jsonObject2 = JsonConvert.DeserializeObject<JObject>(response.Content.ToString()) ;
            string exchange = (string)jsonObject2["code"];

            string url2 = Endpoint.Token;
            RestClient client2 = new RestClient(url2);
            RestRequest request2 = new RestRequest("", Method.Post);
            request2.AddHeader("Authorization", Globals.LAUNCHER_CLIENT);
            request2.AddHeader("Content-Type", "application/x-www-form-urlencoded");

            request2.AddParameter("grant_type", "exchange_code");
            request2.AddParameter("exchange_code", exchange);

            RestResponse response2 = client2.Execute(request2);
            sendRequestLog(url2, request2, response2);
            JObject jsonObject = JsonConvert.DeserializeObject<JObject>(response2.Content.ToString());
            string accessToken = (string)jsonObject2["access_token"];

            string url3 = Endpoint.ExchangeCode;
            RestClient client3 = new RestClient(url3);
            RestRequest request3 = new RestRequest("", Method.Get);
            string token3 = $"bearer {access_token}";
            request3.AddHeader("Authorization", token3);
            request3.AddHeader("Content-Type", "application/json");
            RestResponse response3 = client3.Execute(request3);
            sendRequestLog(url3, request3, response3);

            JObject jsonObject3 = JsonConvert.DeserializeObject<JObject>(response3.Content.ToString());
            string exchange3 = (string)jsonObject3["code"];
            return exchange3;
        }

        public static void GenerateDeviceAuth(string access_token, string account_id, string name)
        {
            string url = Endpoint.DeviceAuth.Replace("accountID", account_id);
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.Post);

            string token = $"bearer {access_token}";
            request.AddHeader("Authorization", token);
            request.AddHeader("Content-Type", "application/json");

            RestResponse response = client.Execute(request);
            sendRequestLog(url, request, response);
            if((int)response.StatusCode == 200)
            {
                JObject jsonObject = JsonConvert.DeserializeObject<JObject>(response.Content.ToString());
                string device_id = (string)jsonObject["deviceId"];
                string secret = (string)jsonObject["secret"];
                Config.AddAccount(name, account_id, device_id, secret);
                if (Globals.debug == "false")
                {
                    Console.Clear();
                }
                Program.Selector();
            }
        }
    }
}