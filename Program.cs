using System;
using System.Runtime.InteropServices;
using FortniteLauncher.Class;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using RestSharp;
using System.Diagnostics;
using Serilog;
using Serilog.Core;

namespace CandyLauncher
{
    class Program
    {
        public static Loggers logger = new Loggers();
        static void Main(string[] args)
        {
            Config.Check();

            Console.Title = "Candy Launcher";

            Selector("Bienvenue");
        }

        public static void Selector(string message="None")
        {
            int nb = 0;
            
            Config.ConfigData config = Config.Read();
            if (message != "None") 
            {
                Console.WriteLine($"{logger.Green($"Bienvenue dans le CandyLauncher v{Globals.version} !")}\n");
            }
            
            Console.WriteLine($"{logger.Aqua("0")}: Ajouter un compte");
            foreach (string key in config.accounts.Keys)
            {
                nb++;
                Console.WriteLine($"{logger.Aqua(nb.ToString())}: {key}");
            }
            Console.Write("\nNuméro du compte à lancer: ");
            string name = Console.ReadLine();
            if (name == "0")
            {

                Console.Write("Exchange Code: ");
                string exchange_code = Console.ReadLine();
                string req = Requests.GetTokenByExchange(exchange_code);
                if(req != "Error")
                {
                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(req);
                    string access_token = (string)jsonObject["access_token"];
                    string account_id = (string)jsonObject["account_id"];
                    string displayname = (string)jsonObject["displayName"];

                    Requests.GenerateDeviceAuth(access_token, account_id, displayname);
                    Console.Clear();
                }
                else
                {
                    Selector();
                }
            }
            else
            {
                int isRightNumber;
                int number = 100000;
                bool isNumber = int.TryParse(name, out isRightNumber);
                if (isNumber)
                {
                    number = int.Parse(name);
                }
                if(number > nb)
                {
                    if (Globals.debug == "false")
                    {
                        Console.Clear();
                    }
                    Console.WriteLine($"{logger.Red("Erreur ! Entre un nombre correct !")}");
                    Selector();
                }
                else
                {
                    if (Globals.debug == "false")
                    {
                        Console.Clear();
                    }

                    string device_id = config.accounts.ElementAt(number - 1).Value.device_id;
                    string account_id = config.accounts.ElementAt(number - 1).Value.account_id;
                    string secret = config.accounts.ElementAt(number - 1).Value.secret;
                    string accountname = config.accounts.ElementAt(number - 1).Key;

                    Console.WriteLine($"{logger.Aqua("Vérification en cours...")}");
                    RestResponse tokenResponse = Requests.GetTokenByDeviceAuth(account_id, device_id, secret);
                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(tokenResponse.Content.ToString());
                    string access_token = (string)jsonObject["access_token"];
                    if (!tokenResponse.IsSuccessStatusCode)
                    {
                        if (Globals.debug == "false")
                        {
                            Console.Clear();
                        }
                        Console.WriteLine($"{logger.Red($"Les identifiants pour le compte {accountname} sont invalides !")}");
                        Selector();
                    }
                    else
                    {
                        string response = Requests.GenerateExchangeCode(access_token);

                        string workingDirectory = Globals.path;
                        string command = $"start \"\" FortniteLauncher.exe -AUTH_LOGIN=unused -AUTH_PASSWORD={response} AUTH_TYPE=exchangecode -epicapp=Fortnite -epicenv=Prod -epicportal";

                        ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe")
                        {
                            WorkingDirectory = workingDirectory,
                            RedirectStandardInput = true,
                            RedirectStandardOutput = true,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                        };

                        Process process = new Process { StartInfo = processStartInfo };
                        process.Start();

                        process.StandardInput.WriteLine(command);
                        process.StandardInput.Close();

                        Console.WriteLine($"{ logger.Aqua($"Lancement du compte {logger.Pink(config.accounts.ElementAt(number - 1).Key)} ...")}");
                    }
                   
                    


                }
            }

        }

    }
}