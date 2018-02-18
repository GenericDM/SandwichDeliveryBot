using Discord.Commands;
using RestSharp;
using SandwichDeliveryBot.Databases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using SandwichDeliveryBot3.CustomClasses;
using Discord;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot3.Precons;

namespace SandwichDeliveryBot3.Modules
{
    public class CreditsModule : ModuleBase
    {

        UserDatabase udb;
        SandwichService ss;
        ArtistDatabase adb;

        public CreditsModule(IServiceProvider p)
        {
            udb = p.GetService<UserDatabase>();
            ss = p.GetService<SandwichService>();
        }

        [Command("exchange")]
        [Alias("convert")]
        [NotBlacklisted]
        public async Task MakeExchange(float amount, string towhom)
        {
            //double mult = Math.Pow(10.0, 2);
            //double result = Math.Truncate(mult * amount) / mult;
            //SandwichUser u = await udb.FindUser(Context.User.Id);
            //if ((u.Credits - (float)result) < 0)
            //{
            //    await ReplyAsync("You need more credits for this transaction!");
            //    return;
            //}

            //RestClient client = new RestClient($"https://discoin.disnodeteam.com/");
            //RestRequest request = new RestRequest($"/transaction/{Context.User.Id}/{(float)result}/{towhom.ToUpper()}", Method.GET);

            //request.AddHeader("Authorization", "A42D4911CACA52922BFAD61E44D71");
            //IRestResponse response = client.Execute(request);
            //if (response.Content.StartsWith("[ERROR]"))
            //{
            //    await ReplyAsync($"An error has occured: `{response.Content}`");
            //    return;
            //}
            //await ReplyAsync($"Submitted with response: `{response.Content}` Exchanges will take some time to process. So be patient!");
            //u.Credits -= (float)result;
            //await udb.SaveChangesAsync();
            await ReplyAsync("Cant.");
        }

        [Command("rates")]
        [NotBlacklisted]
        public async Task Rates()
        {
            await ReplyAsync("See this link for exchange rates: https://discoin.disnodeteam.com/rates");
        }

        [Command("daily")]
        [Alias("dailycredits")]
        [NotBlacklisted]
        public async Task Daily()
        {
            SandwichUser u= await udb.FindUser(Context.User.Id);
            if (u.lastDailyCredits.AddHours(12) < DateTime.Now)
            {
                
                var r = new Random();
                var rand = r.Next(2, 15);
                if (u.IsPatron)
                {
                    await udb.GiveUserCredits(u, rand * 3);
                    await udb.UpdateCreditDate(u);
                    await ReplyAsync($"You have earned {rand * 3} credits! Come back tomorrow for more, Patron. :wink:");
                }
                else
                {
                    await udb.GiveUserCredits(u, rand);
                    await udb.UpdateCreditDate(u);
                    await ReplyAsync($"You have earned {rand} credits! Come back tomorrow for more.");
                }
            }
            else
            {
                await ReplyAsync("Come back later for more!");
            }
        }

        [Command("tip")]
        [NotBlacklisted]
        public async Task Tip(IGuildUser us, float a)
        {
            SandwichUser u = await udb.FindUser(Context.User.Id);
            if (u != null)
            {
                    if ( a > 0.0f)
                    {
                        IUser userr = us as IUser;
                        IGuild USR = await Context.Client.GetGuildAsync(ss.USRGuildId);
                        ITextChannel log = await  USR.GetTextChannelAsync(ss.TipId);
                        SandwichUser tipper = await udb.FindUser(Context.User.Id);
                        if (tipper.Credits < a) { await ReplyAsync("You have no more credits left to tip!"); return; };
                        if (userr == Context.User) { await ReplyAsync("You can't tip yourself. That is cheating!"); await log.SendMessageAsync($"**{Context.User.Username}#{Context.User.Discriminator}** just tried to tip themselves..."); return; }
                        SandwichUser recsu = await udb.FindUser(us.Id);
                        recsu.Credits += a;
                        u.Credits -= a;
                        await udb.SaveChangesAsync();
                        await log.SendMessageAsync($"**{Context.User.Username}#{Context.User.Discriminator}** just tipped **{us.Username}#{us.Discriminator}** {a} credits..");
                        await ReplyAsync($"Thank you for tipping, you now have **{u.Credits}** credits left.");
                    }
                    else
                    {
                        await ReplyAsync("You can only tip a minimum of 1 credit.");
                    }
            }
            else
            {
                Console.WriteLine("cant find user in db");
            }
        }
    }
}
