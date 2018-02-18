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
using SandwichDeliveryBot3.CustomClasses;
using SandwichDeliveryBot3.Enums;

namespace SandwichDeliveryBot.UtilityMod
{
    public class UserModule : ModuleBase
    {
        UserDatabase _udb;

        public UserModule(IServiceProvider provider)
        {
            _udb = provider.GetService<UserDatabase>();
        }
        [Command("changeorders")]
        [RequireBlacklist]
        public async Task changeOrders(IGuildUser user, int diff)
        {
            await _udb.ChangeOrders(user.Id, diff);
        }
        [Command("changedenials")]
        [RequireBlacklist]
        public async Task changeDenials(IGuildUser user, int diff)
        {
            await _udb.ChangeDenials(user.Id, diff);
        }
        [Command("userinfo")]
        [Alias("profile")]
        public async Task Invite(IGuildUser user = null)
        {
            SandwichUser u;
            if (user == null)
                u = await _udb.FindUser(Context.User.Id);
            else
                u = await _udb.FindUser(user.Id);

            var random = new Random();

            Color c;
            c = new Color(random.Next(1,254), random.Next(1, 254), random.Next(1, 254));
       
            await ReplyAsync($"{Context.User.Mention} Here is your requested information!", embed: new EmbedBuilder()
            .AddField(builder =>
            {
                builder.Name = "**User**";
                builder.Value = u.Name+"#"+u.Distin;
                builder.IsInline = true;
            })
             .AddField(builder =>
             {
                 builder.Name = "Credits";
                 builder.Value = u.Credits;
                 builder.IsInline = true;
             })
            .AddField(builder =>
            {
                builder.Name = "Level";
                builder.Value = u.Level.ToString();
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "Orders";
                builder.Value = u.Orders;
                builder.IsInline = true;
            })
             .AddField(builder =>
             {
                 builder.Name = "Denied Orders";
                 builder.Value = u.Denials;
                 builder.IsInline = true;
             })
              .AddField(builder =>
              {
                  builder.Name = "DICK size";
                  if (u.DSize == 100)
                  {
                      builder.Value = "Type ;dick!";
                  }
                  else
                  {
                      builder.Value = u.DSize + " inches.";
                  }
                  builder.IsInline = true;
              })
               .AddField(builder =>
               {
                   builder.Name = "Credits gambled";
                   builder.Value = u.CreditsGambled;
                   builder.IsInline = true;
               })
            .WithUrl("https://discord.gg/DmGh9FT")
            .WithColor(c)
            .WithTitle("User information")
            .WithTimestamp(DateTime.Now));
        }
    }
}