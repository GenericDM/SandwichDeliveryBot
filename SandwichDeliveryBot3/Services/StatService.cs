using Newtonsoft.Json;
using SandwichDeliveryBot3.CustomClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandwichDeliveryBot3.Services
{
    public class StatService
    {
        public List<string> OrdersToday = new List<string>();
        public List<string> OrdersThisWeek = new List<string>();
        public List<string> OrdersThisMonth = new List<string>();

        public DateTime EndOfToday;
        public DateTime EndOfWeek;
        public DateTime EndOfMonth;

        public void OutputStats()
        {
           
        }

        public void NewOrder()
        {
            
        }

        public void SetTimes()
        {
            
        }

    }
}
