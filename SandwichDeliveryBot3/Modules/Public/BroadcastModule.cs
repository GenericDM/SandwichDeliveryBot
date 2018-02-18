using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot3.Precons;
using Microsoft.Extensions.DependencyInjection;
using SandwichDeliveryBot.Databases;

namespace SandwichDeliveryBot.BroadcastMod
{
    public class BroadcastModule : ModuleBase
    {
        SandwichService _SS;
        SandwichDatabase _DB;
        ArtistDatabase _ADB;
        ListingDatabase _LDB;
        UserDatabase _UDB;
        //TipDatabase _TDB;
        BroadcastDatabase _BDB;

        public BroadcastModule(IServiceProvider provider)
        {
            _SS = provider.GetService<SandwichService>();
            _DB = provider.GetService<SandwichDatabase>();
            _ADB = provider.GetService<ArtistDatabase>();
            _LDB = provider.GetService<ListingDatabase>();
            _UDB = provider.GetService<UserDatabase>();
           // _TDB = provider.GetService<TipDatabase>();
            _BDB = provider.GetService<BroadcastDatabase>();
        }

        [Command("broadcasthelp")]
        public async Task broadcasthelp()
        {
            await ReplyAsync("Commands: \r\n" +
                "Only users with administrator can use these commands. \r\n" +
                ";recievebroadcast - Enables recieving of broadcasts \r\n" +
                ";changechannel - Changes broadcast channel to the channel you send the command in \r\n" +
                ";stopbroadcast - Stops recieving of broadcasts.");
        }
        [Command("recievebroadcast")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task recievebroadcast()
        {
            await _BDB.NewGuild(Context.Guild.Id, Context.Channel.Id);
            IGuild s = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel ch = await s.GetTextChannelAsync(_SS.LogId);
            await ReplyAsync(":thumbsup:, `;broadcasthelp` for further info.");
            await ch.SendMessageAsync($"**{Context.Guild.Name}** has registered their guild to recieve broadcasts.");
        }
        [Command("changechannel")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task changechannel()
        {
            await _BDB.ChangeChannel(Context.Guild.Id, Context.Channel.Id);
            await ReplyAsync(":thumbsup:");
        }
        [Command("stoprecievingbroadcast")]
        [Alias("srb", "stopbroadcast")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task stoprecievingbroadcast()
        {
            await _BDB.RemoveGuild(Context.Guild.Id);
            await ReplyAsync(":thumbsup:");
            IGuild s = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel ch = await s.GetTextChannelAsync(_SS.LogId);
            await ch.SendMessageAsync($"**{Context.Guild.Name}** has removed their guild from recieving broadcasts.");
        }
        [Command("broadcast")]
        public async Task Broadcast([Remainder]string b)
        {
            if (Context.User.Id != 131182268021604352)
                return;

            var guilds = _BDB.ToArray();
            foreach(var guild in guilds)
            {
                var g = await Context.Client.GetGuildAsync(guild.GuildID);
                if(g != null)
                {
                    var c = await g.GetTextChannelAsync(guild.BroadcastChannelID);
                    if (c == null)
                    {
                        c = await g.GetTextChannelAsync(g.DefaultChannelId);
                    }
                    await c.SendMessageAsync($"`;broadcasthelp` for further info.", embed: new EmbedBuilder()
            .WithDescription(b)
            .WithThumbnailUrl(Context.User.GetAvatarUrl())
            .WithUrl("https://discord.gg/DmGh9FT")
            .WithColor(new Color(156, 183, 226))
            .WithTitle("New broadcasted message!")
            .WithTimestamp(DateTime.Now));
                }
            }
        }
    }
}
