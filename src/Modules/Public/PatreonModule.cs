using Discord;
using Discord.Commands;
using SandwichDeliveryBot3.Precons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SandwichDeliveryBot.Databases;

namespace SandwichDeliveryBot3.Modules.Public
{
    public class PatreonModule : ModuleBase
    {

        UserDatabase udb;

        public PatreonModule(IServiceProvider prov)
        {
            udb = prov.GetService<UserDatabase>();
        }

        [Command("patreon")]
        public async Task Patreon()
        {
            await ReplyAsync("We now have a Patreon! https://www.patreon.com/sandwichdelivery");
        }

        [Command("donate")]
        public async Task PatreonCredits()
        {
            await ReplyAsync("If you donate, make sure to tell us! https://www.paypal.me/USRBots");
        }

        [Command("ispatron")]
        [RequireBlacklist]
        public async Task IsPatron(IGuildUser us)
        {
            var user = await udb.FindUser(us.Id);
            if (user.IsPatron)
            {
                await ReplyAsync("This user is already a Patron. We'll make it so they aint.");
                user.IsPatron = false;
                await udb.SaveChangesAsync();
            }
            else if (!user.IsPatron)
            {
                await ReplyAsync("Alright, I've made them a patron. They know have access to SPECIAL things");
                user.IsPatron = true;
                await udb.SaveChangesAsync();
            }
        }

        [Command("patroncommands")]
        public async Task PatronCommands()
        {
            await ReplyAsync("__**Current Custom Patron Commands**__\r\n- ;freemoney\r\n- ;ratemywaifu <waifuname>\r\nWant a custom command? Support us on `;patreon`!");
        }

        [Command("freemoney")]
        public async Task CustomPatronCommand()
        {
            await ReplyAsync("Do you really think you can just get free money?");
        }

        [Command("ratemywaifu")]
        [Alias("waifu")]
        public async Task RateMyWaifu([Remainder]string waifu)
        {
            var r = new Random();
            string[] possible = new string[] { "trash.", "alright.", "the best."};
            var pick = r.Next(0, 2);
            await ReplyAsync($"I will rate {waifu}, Your waifu is {possible[pick]}");
        }


    }
}
