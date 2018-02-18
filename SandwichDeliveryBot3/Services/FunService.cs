using Newtonsoft.Json;
using System;
using System.IO;

namespace SandwichDeliveryBot3.Services
{
    public class FunService
    {
        public float Pot { get; set; } = 0;

        public void SavePot()
        {
            try
            {

                using (var sw = new StreamWriter(@"data/jackpot.json", false))
                {
                    JsonSerializer.Create().Serialize(sw, Pot);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}");
            }
        }

        public void LoadPot()
        {
            try
            {
                using (var sr = new StreamReader(@"data/jackpot.json"))
                {
                    var myLovelyReader = new JsonTextReader(sr);
                    Pot = JsonSerializer.Create().Deserialize<float>(myLovelyReader);
                }
            }
            catch
            {
                Console.WriteLine("Failed to load pot.");
            }
        }
    }
}
