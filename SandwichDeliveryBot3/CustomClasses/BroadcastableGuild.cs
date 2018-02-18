using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandwichDeliveryBot3.CustomClasses
{
    public class BroadcastableGuild
    {
        [Key]
        public int Key { get; set; }
        public ulong GuildID { get; set; }
        public ulong BroadcastChannelID { get; set; }
        public DateTime date { get; set; }

        public BroadcastableGuild(ulong id, ulong channelid) {
            GuildID = id;
            BroadcastChannelID = channelid;
            date = DateTime.Now;
        }

        public BroadcastableGuild() { }
    }
}
