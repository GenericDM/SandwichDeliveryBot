using Discord;
using Newtonsoft.Json;
using System;

namespace SandwichDeliveryBot3.CustomClasses
{
    [JsonObject]
    public class Artist
    {
        public ulong ArtistId { get; set; } 
        public string ArtistName { get; set; } 
        public string ArtistDistin { get; set; }
        public int ordersAccepted { get; set; } 
        public int ordersDenied { get; set; } 
        public int ordersConfirmed { get; set; }
        public string status { get; set; } = "Trainee";
        public bool canBlacklist { get; set; } = false;
        public string HiredDate { get; set; }
        public DateTime lastOrder { get; set; }

        public Artist(IGuildUser newartist, string date) {
            this.ArtistId = newartist.Id;
            this.ArtistName = newartist.Username;
            this.ArtistDistin = newartist.Discriminator;
            this.HiredDate = date;
            this.ordersAccepted = 0;
            this.ordersDenied = 0;
            this.lastOrder = DateTime.Now;
        }

        public Artist() { }
    }
}
