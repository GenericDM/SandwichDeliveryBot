using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot3.Precons;
using SandwichDeliveryBot.Databases;
using SandwichDeliveryBot3.CustomClasses;
using Microsoft.Extensions.DependencyInjection;
using System;
using SandwichDeliveryBot3.Services;

namespace SandwichDeliveryBot3.SandwichMod
{
    public class SandwichModule : ModuleBase
    {
        SandwichService _SS;
        SandwichDatabase _DB;
        ArtistDatabase _ADB;
        ListingDatabase _LDB;
        UserDatabase _UDB;
        StatService _stats;

        public SandwichModule(IServiceProvider provider)
        {
            _SS = provider.GetService<SandwichService>();
            _DB = provider.GetService<SandwichDatabase>();
            _ADB = provider.GetService<ArtistDatabase>();
            _LDB = provider.GetService<ListingDatabase>();
            _UDB = provider.GetService<UserDatabase>();
            _stats = provider.GetService<StatService>();
        }


        //[Command("respond")]
        //[Alias("r")]
        //[NotBlacklisted]
        //[RequireSandwichArtist]
        //public async Task Respond(int id, [Remainder]string response)
        //{
        //    if (SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value != null)
        //    {
        //        Sandwich order = SS.activeOrders.FirstOrDefault(s => s.Value.Id == id).Value;
        //        try
        //        {
        //            IGuild g = await Context.Client.GetGuildAsync(order.GuildId);
        //            ITextChannel t = await g.GetTextChannelAsync(order.ChannelId);
        //            Color c = new Color(255, 43, 43);
        //            if (t != null)
        //            {
        //                await t.SendMessageAsync($"<@{order.UserId}>, {Context.User.Username}#{Context.User.Discriminator} from The Kitchen™ as responded to your order! They said this:", embed: new EmbedBuilder()
        //        .AddField(builder =>
        //        {
        //            builder.Name = "Message:";
        //            builder.Value = "```" + response + "```";
        //            builder.IsInline = true;
        //        })
        //         .AddField(builder =>
        //         {
        //             builder.Name = "Your order:";
        //             builder.Value = order.Desc;
        //             builder.IsInline = true;
        //         })
        //        .AddField(builder =>
        //        {
        //            builder.Name = "Respond Back?";
        //            builder.Value = "If you wish to respond use the `;messagekitchen` command! (`;mk` for short). Ex `;mk Sorry about the typo! I want it with cheese!` or `;messagekitchen Hey thanks for the sandwich, I really enjoyed it!`.";
        //            builder.IsInline = true;
        //        })
        //        .WithUrl("https://discord.gg/XgeZfE2")
        //        .WithColor(c)
        //        .WithTitle("Message from The Kitchen™!")
        //        .WithTimestamp(DateTime.Now));
        //                await ReplyAsync($"{Context.User.Mention} Response successfully sent!");
        //                IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
        //                ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
        //                ITextChannel usrclog = await usr.GetTextChannelAsync(SS.usrlogcID);
        //                await usrclog.SendMessageAsync($"{Context.User.Mention} has responded to order `{order.Id}` with message: `{response}`.");
        //                await Context.Message.DeleteAsync();
        //                SS.LogCommand(Context, "Respond", new string[] { id.ToString(), response });
        //            }
        //        }
        //        catch (Exception e)//love me some 'defensive' programming
        //        {
        //            await ReplyAsync($"Contact Fires. ```{e}```");
        //            Console.WriteLine(e);
        //        }
        //    }
        //}

        //[Command("messagekitchen")]
        //[Alias("mk")]
        //[NotBlacklisted]
        //public async Task MessageKitchen([Remainder]string message)
        //{
        //    if (message.Length > 5)
        //    {
        //        IGuild usr = await Context.Client.GetGuildAsync(SS.usrID);
        //        ITextChannel usrc = await usr.GetTextChannelAsync(SS.usrcID);
        //        ITextChannel usrclog = await usr.GetTextChannelAsync(SS.usrlogcID);
        //        Color c = new Color(95, 62, 242);
        //        await usrc.SendMessageAsync($"New message from {Context.User.Mention}!", embed: new EmbedBuilder()
        //        .AddField(builder =>
        //        {
        //            builder.Name = "Message:";
        //            builder.Value = "```" + message + "```";
        //            builder.IsInline = true;
        //        })
        //         .AddField(builder =>
        //         {
        //             builder.Name = "Guild:";
        //             builder.Value = $"{Context.Guild.Name}({Context.Guild.Id})";
        //             builder.IsInline = true;
        //         })
        //          .AddField(builder =>
        //          {
        //              builder.Name = "Channel:";
        //              builder.Value = $"{Context.Channel.Name}({Context.Channel.Id})";
        //              builder.IsInline = true;
        //          })
        //        .WithColor(c)
        //        .WithTitle("Message from a customer!")
        //        .WithTimestamp(DateTime.Now));


        //        //Too lazy to create an embed and send them to both. so gonna run the shitty way. sorry D:


        //        await usrclog.SendMessageAsync($"New message from {Context.User.Mention}!", embed: new EmbedBuilder()
        //        .AddField(builder =>
        //        {
        //            builder.Name = "Message:";
        //            builder.Value = "```" + message + "```";
        //            builder.IsInline = true;
        //        })
        //         .AddField(builder =>
        //         {
        //             builder.Name = "Guild:";
        //             builder.Value = $"{Context.Guild.Name}({Context.Guild.Id})";
        //             builder.IsInline = true;
        //         })
        //          .AddField(builder =>
        //          {
        //              builder.Name = "Channel:";
        //              builder.Value = $"{Context.Channel.Name}({Context.Channel.Id})";
        //              builder.IsInline = true;
        //          })
        //        .WithColor(c)
        //        .WithTitle("Message from a customer!")
        //        .WithTimestamp(DateTime.Now));
        //        await Context.Message.DeleteAsync();
        //        await ReplyAsync(":thumbsup:");
        //        SS.LogCommand(Context, "Respond", new string[] { message });
        //    }
        //    else
        //    {
        //        await ReplyAsync("Your message must be longer then 5 characters.");
        //    }
        //}

        [Command("motd")]
        [NotBlacklisted]
        public async Task MOTD()
        {
            await ReplyAsync(_SS.motd);
        }

        [Command("blacklist")]
        [Alias("b")]
        [RequireBlacklist]
        public async Task Blacklist(ulong id, string name = "Undefined", [Remainder]string reason = "No reason given.")
        {
            Artist a = await _ADB.FindArtist(Context.User.Id);
            if (a != null)
            {
                await _LDB.NewListing(id, name, reason);
                IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
                await usrc.SendMessageAsync($"{Context.User.Mention} blacklisted <@{id}> for `{reason}`(id).");
                await ReplyAsync(":thumbsup:");
            }

        }


        [Command("blacklist")]
        [Alias("b")]
        [RequireBlacklist]
        public async Task Blacklist(IGuildUser user, [Remainder]string reason = "No reason given.")
        {
            Artist a = await _ADB.FindArtist(Context.User.Id);
            if (a != null)
            {
                await _LDB.NewListing(user.Id, user.Username, reason);
                IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
                await usrc.SendMessageAsync($"{Context.User.Mention} blacklisted <@{user.Id}> for `{reason}`(id).");
                await ReplyAsync(":thumbsup:");
            }
        }

        [Command("unblacklist")]
        [Alias("ub")]
        [RequireBlacklist]
        public async Task removeFromBlacklist(ulong id)
        {
            await _LDB.RemoveListing(id);
            IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
            await usrc.SendMessageAsync($"{Context.User.Mention} unblacklisted <@{id}>(id).");
            await ReplyAsync(":thumbsup:");
        }

        [Command("unblacklist")]
        [Alias("ub")]
        [RequireBlacklist]
        public async Task removeFromBlacklist(IGuildUser user)
        {
            await _LDB.RemoveListing(user.Id);
            IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
            await usrc.SendMessageAsync($"{Context.User.Mention} unblacklisted <@{user.Id}>(user).");
            await ReplyAsync(":thumbsup:");
        }

        [Command("listings")]
        public async Task showListings()
        {
            foreach (var o in _LDB.Listings)
            {
                await ReplyAsync($"{o.Name}, {o.Type}, {o.ID}, {o.Case}");
            }
        }

        [Command("editlisting")]
        [RequireBlacklist]
        public async Task editListings(ulong id, string name, string type, [Remainder]string reason)
        {
            await _LDB.EditListing(id, name, reason, type);
            IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
            await usrc.SendMessageAsync($"{Context.User.Mention} edited listing, `{id}`, `{type}`, `{reason}`.");
            await ReplyAsync(":thumbsup:");
        }

        [Command("editlistingname")]
        [RequireBlacklist]
        public async Task editListingName(ulong id, string name)
        {
            Listing[] array = await _LDB.GetArray();
            string cas = array.FirstOrDefault(x => x.ID == id).Name;
            await _LDB.EditListing(id, name);
            IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
            await usrc.SendMessageAsync($"{Context.User.Mention} edited name listing, `{id}`, from `{cas}` to `{name}`.");
            await ReplyAsync(":thumbsup:");
        }
        [Command("editlistingcase")]
        [RequireBlacklist]
        public async Task editListingCase(ulong id, int c)
        {
            Listing[] array = await _LDB.GetArray();
            string cas = array.FirstOrDefault(x => x.ID == id).Case.ToString();
            await _LDB.EditListing(id, c);
            IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
            ITextChannel usrc = await usr.GetTextChannelAsync(_SS.LogId);
            await usrc.SendMessageAsync($"{Context.User.Mention} edited case listing, `{id}`, from `{cas}` to `{c}`.");
            await ReplyAsync(":thumbsup:");
        }

        [Command("listinginfo")]
        [Alias("li")]
        public async Task listinginfo(ulong id)
        {
            Listing[] list = await _LDB.GetArray();
            Listing listing = list.FirstOrDefault(x => x.ID == id);
            await ReplyAsync($"{Context.User.Mention} Here is your requested information!", embed: new EmbedBuilder()
            .AddField(builder =>
            {
                builder.Name = "Name";
                builder.Value = listing.Name;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "Reason";
                builder.Value = listing.Reason;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "User ID";
                builder.Value = listing.ID;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "Type";
                builder.Value = listing.Type;
                builder.IsInline = true;
            })
            .AddField(builder =>
            {
                builder.Name = "Case number";
                builder.Value = listing.Case;
                builder.IsInline = true;
            })
             .AddField(builder =>
             {
                 builder.Name = "Date of listing";
                 builder.Value = listing.Date;
                 builder.IsInline = true;
             })
            .WithUrl("https://discord.gg/DmGh9FT")
            .WithTitle("Listing information")
            .WithTimestamp(DateTime.Now));

        }

        [Command("amiblacklisted")]
        public async Task amiblacklisted()
        {
            Console.WriteLine(await _LDB.CheckForBlacklist(Context.User.Id));
            string r = await _LDB.CheckForBlacklist(Context.User.Id);
            Console.WriteLine(r);
            await ReplyAsync(r ?? "lmao failed");
        }

        [Command("totalorders")]
        [Alias("to")]
        [NotBlacklisted]
        public async Task TotalOrders()
        {
            await ReplyAsync($"We have proudly served {_SS.totalOrders} sandwiches since our inception.");
        }

        [Command("credits")]
        [Alias("cred")]
        [NotBlacklisted]
        public async Task credits()
        {
            await ReplyAsync($"https://github.com/USRDiscordBots/Sandwich-Delivery-Bot-v2.0/wiki/Getting-Started-as-a-Sandwich-Artist#before-you-continue-a-quick-thank-you-to");
        }

        [Command("help")]
        [Alias("h")]
        public async Task Help()
        {
            var dm = await Context.User.GetOrCreateDMChannelAsync();
            var auth = new EmbedAuthorBuilder() { IconUrl = Context.Client.CurrentUser.GetAvatarUrl(), Name = "Help" };

            await dm.SendMessageAsync("", embed: new EmbedBuilder()
            {
                Title = "Command List",
                Author = auth,
                Description = "**__Main__** \r\n " +
                "**;order** - Orders a sandwich. - ;order medium blt \r\n " +
                "**;feedback** - Gives feedback regarding the bot/delivery - ;feedback I really liked my sandwich artist. ok bot tho \r\n " +
                "**[;server](https://discord.gg/DmGh9FT)** - Returns our server invite \r\n " +
                "**;invite** - Returns bot invite link \r\n " +
                "**;delorder** - Deletes your order \r\n " +
                "**;tip** - Tip your sandwich deliverer! - ;tip @Fires#1043 15 \r\n " +
                "**;userinfo** - Returns your profile \r\n" +
                "**;daily** - Recieve your daily credits! \r\n "
            });

            await dm.SendMessageAsync("", embed: new EmbedBuilder()
            {
                Title = "Command List - Fun",
                Author = auth,
                Description = "**__Fun__** \r\n " +
                "**;gamble** - Try to win the jackpot. - ;gamble 15 \r\n " +
                "   **;pot** - Returns current jackpot \r\n" +
                "**;roulette** - Try it russian style - ;roulette 3 \r\n" +
                "**;dick** - Whats yo size man? \r\n" +
                "**;8ball** - [Consult the ball](https://youtu.be/KXlkmPXDvqU?t=23s) - ;8ball Will I ever find the perfect sandwich?\r\n"+
                "**;cat** - Random cat photo....meow\r\n" +
                "**;coin** - no.\r\n" +
                "**;xkcd** - Random xkcd comic.\r\n" +
                "**;ronswanson** - [Random Ron Swanson quote](https://www.youtube.com/watch?v=FA6ZcuQYbbQ&feature=youtu.be&t=16s)\r\n" +
                "**;advice** - Free advice..."
            });

            await dm.SendMessageAsync("", embed: new EmbedBuilder()
            {
                Title = "Command List - Quote",
                Author = auth,
                Description =
               "__**Quote**__ \r\n " +
               "**;quote** - Returns a random quote \r\n" +
               "**;quote suggest** - Suggust a quote! Feel free too, just don't abuse this command! - ;quote suggest http://i.imgur.com/RjoOmoa.png \r\n" +
               "**;quote count** - Amount of quotes. You can give it an id to get specific ids for that user"
            });

            var patreon = new EmbedAuthorBuilder() { IconUrl = "https://c5.patreon.com/external/logo/downloads_logomark_color_on_white@2x.png", Name = "Help" };

            await dm.SendMessageAsync("", embed: new EmbedBuilder()
            {
                Title = "Command List - Patreon",
                Author = patreon,
                Description =
                "__**Patreon**__ \r\n " +
                "**;patreon** - Our patreon. \r\n" +
                "**;donate** - Our paypal.(hosting is expensive) \r\n" +
                "**;freemoney** - 5ky's custom command."
            });


            await dm.SendMessageAsync("", embed: new EmbedBuilder()
            {
                Title = "Command List - Misc",
                Author = auth,
                Description = 
                "__**Misc**__ \r\n " +
                "**;broadcasthelp** - Information on the broadcast system \r\n" +
                "**;motd** - Join message! \r\n" +
                "**;info** - Bot stats/info \r\n" +
                "**;updateinfo** - Info on last bot update \r\n" +
                "**;credits** - List of people who I appreciate \r\n" +
                "**;totalorders** - Total orders! \r\n" +
                "**Broadcasts** \r\n" +
                "**;recievebroadcast** - Enable yourself to recieve announcements \r\n" +
                "**;changechannel** - Change announcement channel to the channel you send this command in \r\n" +
                "**;patroncommands - shhh\r\n" +
                "**;stopbroadcast** - Stops recieving broadcasts."
            });

            
            await dm.SendMessageAsync("", embed: new EmbedBuilder()
            {
                Title = "Command List - Admin",
                Author = auth,
                Color = new Color(237, 26, 26),
                Description =
                "__**Admin commands**__ \r\n " +
                "**;artist new** - Creates a new database entry for the user given - ;artist new @Fires#1043 \r\n" +
                "**;artist del** - Removes the database entry for the user given - ;artist del @Fires#1043 \r\n" +
                "**;artist promote** - Promotes the given user - ;artist promote @Fires#1043 \r\n" +
                "**;artist demote** - Demotes the given user - ;artist demote @Fires#1043 \r\n" +
                "**;artist admin** - Admin another artist - ;artist admin @Fires#1043 \r\n" +
                "**;ispatron** - Set a user to get patron specific rewards.\r\n"+
                "**__Fires Only__** \r\n" +
                "**;quote new** - Creates a new quote - ;quote new http://i.imgur.com/RjoOmoa.png 264222431172886529 \r\n" +
                "**;quote del** - Deletes quote that matches given id - ;quote del 2"
            });


            await dm.SendMessageAsync("", embed: new EmbedBuilder()
            {
                Title = "Contact",
                Author = auth,
                Color = new Color(237, 26, 26),
                Description =
               "__**Contact**__ \r\n " +
               "Fires#1043 - Bot owner/developer. \r\n " +
               "ShadowR3con#8876 - Sandwich Artist(and delivery) manager. \r\n" +
               "Ruhe#0653 - Server and bot admin. \r\n" +
               "https://discord.gg/DmGh9FT - Our server"
            });
        }
    }
}