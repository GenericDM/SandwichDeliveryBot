using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
//using RequireBlacklistPrecon;
//using inUSRPrecon;
//using NotBlacklistedPreCon;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot.Databases;
using SandwichDeliveryBot3.Precons;
using SandwichDeliveryBot3.CustomClasses;
using Microsoft.Extensions.DependencyInjection;

namespace SandwichDeliveryBot.ArtistMod
{
    [Group("artist")]
    public class ArtistModule : ModuleBase
    {
        SandwichService _SS;
        SandwichDatabase _DB;
        ArtistDatabase _ADB;
        UserDatabase _UDB;
        ListingDatabase _LDB;

        public ArtistModule(IServiceProvider provider)
        {
            _SS = provider.GetService<SandwichService>();
            _DB = provider.GetService<SandwichDatabase>();
            _ADB = provider.GetService<ArtistDatabase>();
            _LDB = provider.GetService<ListingDatabase>();
            _UDB = provider.GetService<UserDatabase>();
        }

        [Command("new")]
        [Alias("a")]
        [NotBlacklisted]
        [RequireBlacklist]
        public async Task AddArtist(params IGuildUser[] artists)
        {
            int newartists = 0;
            foreach (var artist in artists)
            {
                Artist a = await _ADB.FindArtist(artist);
                if (a == null)
                {
                    Artist r = await _ADB.FindArtist(Context.User.Id);
                    string n = string.Format(artist.Username + "#" + artist.Discriminator);
                    await _ADB.NewArtist(artist, DateTime.Now.ToString("MMMM dd, yyyy"));
                    newartists++;
                }
                else
                {
                    await ReplyAsync($"{artist.Username} is already a Sandwich Artist.");
                }
            }
            await ReplyAsync($"{newartists} new Artists have been added.");
        }

        [Command("del")]
        [Alias("del")]
        [NotBlacklisted]
        [RequireBlacklist]
        public async Task DeleteArtist(params IGuildUser[] artists)
        {
            int deletedartist = 0;
            foreach (var artist in artists)
            {
                Artist a = await _ADB.FindArtist(artist);
                if (a != null)
                {
                    await _ADB.DelArtistAsync(a);
                    deletedartist++;
                }
                else
                    throw new CantFindInDatabaseException();
            }
            await ReplyAsync($":thumbsup:, {deletedartist} Artists have been removed.");
        }

        [Command("admin")]
        [NotBlacklisted]
        [Alias("a")]
        [RequireBlacklist]
        public async Task CanBlacklist(params IGuildUser[] user)
        {
            int updatedusers = 0;
            foreach (var artist in user)
            {
                var a = await _ADB.FindArtist(artist);
                a.canBlacklist = true;
                await _ADB.SaveChangesAsync();
                updatedusers++;
            }
            await ReplyAsync($":thumbsup:, {updatedusers} Artists have been given Administrator control over the bot.");
        }

        [Command("changerank")]
        [NotBlacklisted]
        [RequireBlacklist]
        public async Task PromoteArtist(string newrank, params IGuildUser[] chefs)
        {
            foreach (var chef in chefs)
            {
                Artist a = await _ADB.FindArtist(chef);
                var s = a.status;
                a.status = newrank;
                await ReplyAsync($"Promoted User {chef.Username}#{chef.Discriminator} from {s} to {a.status}");
                await _ADB.SaveChangesAsync();
            }
        }

      
        [Command("stats")]
        [Alias("info")]
        [NotBlacklisted]
        public async Task GetDeliveries(params IGuildUser[] artistss)
        {
            foreach (var chef in artistss)
            {
                Artist a = await _ADB.FindArtist(chef);
                Color c = new Color(54, 219, 148);
                await ReplyAsync($"{Context.User.Mention} Here is your requested information!", embed: new EmbedBuilder()
                .AddField(builder =>
                {
                    builder.Name = "**User**";
                    builder.Value = a.ArtistName+"#"+a.ArtistDistin;
                    builder.IsInline = true;
                })
                   .AddField(builder =>
                   {
                       builder.Name = "Rank";
                       builder.Value = a.status;
                       builder.IsInline = true;
                   })
                .AddField(builder =>
                {
                    builder.Name = "Orders Accepted";
                    builder.Value = a.ordersAccepted;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Most Recent Order";
                    builder.Value = a.lastOrder;
                    builder.IsInline = true;
                })
                 .AddField(builder =>
                 {
                     builder.Name = "Date Hired";
                     builder.Value = a.HiredDate;
                     builder.IsInline = true;
                 })
                .WithUrl("https://discord.gg/DmGh9FT")
                .WithColor(c)
                .WithTitle("User information")
                .WithTimestamp(DateTime.Now));
            }
        }

        
        [Command("list")]
        [NotBlacklisted]
        public async Task listImproved()
        {
            var result = string.Join(", \r\n", _ADB.Artists.Select(x => string.Format("{0}, {1}", x.ArtistName, x.status)).ToArray());
            await ReplyAsync(result);
        }
    }
}
