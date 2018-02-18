using Discord;
using Discord.WebSocket;
using System;

namespace SandwichDeliveryBot3.CustomClasses
{
    public class SandwichUser
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public string Distin { get; set; }
        public int Orders { get; set; }
        public int Denials { get; set; }
        public float Credits { get; set; }
        public int Level { get; set; }
        public DateTime lastTip { get; set; }
        public DateTime lastDailyCredits { get; set; }
        public int DSize { get; set; } = 999;
        public float CreditsGambled { get; set; } = 0;
        public bool IsPatron { get; set; } = false;

        public SandwichUser(SocketUser u)
        {
            Id = u.Id;
            Name = u.Username;
            Distin = u.Discriminator;
            Orders = 1;
            Denials = 0;
            Credits = 5.0f;
            Level = 1;
        }
        public SandwichUser(IGuildUser u)
        {
            Id = u.Id;
            Name = u.Username;
            Distin = u.Discriminator;
            Orders = 1;
            Denials = 0;
            Credits = 5.0f;
            Level = 1;
        }

        public SandwichUser() { }
    }
}
