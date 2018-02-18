using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBot.Services
{
    public class TestService //MAKE SURE THE CLASS IS PUBLIC.
    {
        public string MOTD { get; private set; }
        public int UniqueUsersEncountered { get; set; }

        public void Serialize() //Save data to file
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(this); //Convert to easily readable text, uses https://www.newtonsoft.com/json
            System.IO.File.WriteAllText("testservice.json", json); //Write
        }

        public TestService()
        {
            var filetext = System.IO.File.ReadAllText("testservice.json"); //Read data from file
            TestService temporary = Newtonsoft.Json.JsonConvert.DeserializeObject<TestService>(filetext);
            //This is a bit unusual, but we want to deserialize our object from the json then set all the variables of the service to whatever it read from that file.
            //for your class, just repeat `this.VariableName = temporary.VariableName`
            this.MOTD = temporary.MOTD;
            this.UniqueUsersEncountered = temporary.UniqueUsersEncountered;
        }
    }
}
