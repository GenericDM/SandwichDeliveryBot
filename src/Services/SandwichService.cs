using System;
using Newtonsoft.Json;
using System.IO;

namespace SandwichDeliveryBot.SService
{
    public class SandwichService
    {
        public int totalOrders = 0;
        public float Paychecks = 0;
        public string version = "4.2 TEST"; 
        public string date = "February 4th 2018";
        public string updatename = "TEST BUILD - Testing new queue system.";
        public string motd;
        public ulong USRGuildId = 358634652338225153; 
        public ulong KitchenId = 358637199916400640;
        public ulong LogId = 358664247510695936;
        public ulong TipId = 358664198907232256;


        public void Save()
        {
            try
            {

                using (var sw = new StreamWriter(@"data/ordercount.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, totalOrders);
                }
                using (var sw = new StreamWriter(@"data/pay.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, Paychecks);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }


        public void Load()
        {
            try
            {
                using (var sr = new StreamReader(@"data/ordercount.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    totalOrders = JsonSerializer.Create().Deserialize<int>(myLovelyReader);
                }
                using (var sr = new StreamReader(@"data/motd.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    motd = JsonSerializer.Create().Deserialize<string>(myLovelyReader);
                }
                using (var sr = new StreamReader(@"data/pay.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    Paychecks = JsonSerializer.Create().Deserialize<float>(myLovelyReader);
                }
            }
            catch
            {
                Console.WriteLine("Failed to save.");
            }
        }
    }
}