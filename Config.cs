using System;
using System.Runtime.InteropServices;
using FortniteLauncher.Class;
using System.IO;
using Newtonsoft.Json;
using System.Xml.Linq;
using static CandyLauncher.Config;
using System.Net.Sockets;

namespace CandyLauncher
{
    class Config
    {
        public static void Check()
        {
            string fileName = "config.json";
            if (!File.Exists(fileName))
            {
                Console.WriteLine(Color.Green + "Le fichier config.json n'existe pas, création du fichier !\n" + Color.Clean);
                string jsonContent = "{\"fortnite_path\": \"C:\\Program Files\\Epic Games\\Fortnite\\FortniteGame\\Binaries\\Win64\", \"debug\": \"false\", \"accounts\": {}}";
                File.WriteAllText(fileName, jsonContent);
            }
        }

        public static ConfigData Read()
        {
            string fileName = "config.json";
            string jsonString = File.ReadAllText(fileName);
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Populate
            };
            ConfigData data = JsonConvert.DeserializeObject<ConfigData>(jsonString);
            Globals.debug = data.debug;
            return data;

        }

        public static void AddAccount(string name, string account_id, string device_id, string secret)
        {
            string fileName = "config.json";
            string jsonString = File.ReadAllText(fileName);
            ConfigData data = JsonConvert.DeserializeObject<ConfigData>(jsonString);
            Accounts newAccount = new Accounts
            {
                account_id = account_id,
                device_id = device_id,
                secret = secret
            };
            data.accounts[name] = newAccount;
            string updatedJson = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText(fileName, updatedJson);
        }

        public class Accounts
        {
            public string account_id { get; set; }
            public string device_id { get; set; }
            public string secret { get; set; }
        }

        public class ConfigData
        {
            public string fortnite_path { get; set; }
            public string debug { get; set; }
            public Dictionary<string, Accounts> accounts { get; set; }
        }

    }
}