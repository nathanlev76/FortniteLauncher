using System;
using System.Runtime.InteropServices;
using FortniteLauncher.Class;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Linq;
using RestSharp;
using System.Diagnostics;

namespace CandyLauncher
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Check();
            Selector("Bienvenue");
        }

        public static void Selector(string message="None")
        {
            int nb = 0;
            
            Config.ConfigData config = Config.Read();
            if (message != "None") 
            {
                Console.WriteLine(Color.Green + $"Bienvenue dans le CandyLauncher v{Globals.version} !\n" + Color.Clean);
            }
            
            Console.WriteLine($"{Color.Aqua}0{Color.Clean}: Ajouter un compte");
            foreach (string key in config.accounts.Keys)
            {
                nb++;
                Console.WriteLine($"{Color.Aqua}{nb}{Color.Clean}: {key}");
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
                    Console.WriteLine(Color.Red + "Erreur ! Entre un nombre correct !" + Color.Clean);
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

                    Console.WriteLine(Color.Aqua + "Vérification en cours... " + Color.Clean);
                    RestResponse tokenResponse = Requests.GetTokenByDeviceAuth(account_id, device_id, secret);
                    JObject jsonObject = JsonConvert.DeserializeObject<JObject>(tokenResponse.Content.ToString());
                    string access_token = (string)jsonObject["access_token"];
                    if (!tokenResponse.IsSuccessStatusCode)
                    {
                        if (Globals.debug == "false")
                        {
                            Console.Clear();
                        }
                        Console.WriteLine($"{Color.Red}Les identifiants pour le compte {accountname} sont invalides !{Color.Clean}");
                        Selector();
                    }
                    else
                    {
                        string response = Requests.GenerateExchangeCode(access_token);

                        string workingDirectory = @"C:\Program Files\Epic Games\Fortnite\FortniteGame\Binaries\Win64";
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

                        Console.WriteLine(Color.Aqua + "Lancement du compte " + Color.Pink + config.accounts.ElementAt(number - 1).Key + Color.Aqua + "..." + Color.Clean);
                    }
                   
                    


                }
            }

        }

    }
}