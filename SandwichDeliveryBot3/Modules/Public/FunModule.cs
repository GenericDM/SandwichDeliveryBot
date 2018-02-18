using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SandwichDeliveryBot3.Services;
using SandwichDeliveryBot.Databases;
using System.Net;
using Newtonsoft.Json;
using RestSharp;

namespace SandwichDeliveryBot3.Databases
{
    public class FunModule : ModuleBase
    {
        FunService fs;
        UserDatabase udb;

        string[] answers = new string[] {
            "It is certain.",
            "It is decidedly so.",
            "Without a doubt",
            "lol im a sandwich bot why am i answering your questions???",
            "Yes, definitely.",
            "You may rely on it.",
            "As I see it, yes.",
            "Most likely",
            "Outlook good.",
            "Yes.",
            "Signs point to yes.",
            "Reply hazy, try again.",
            "Order a damn sandwich.",
            "Ask again later.",
            "Better not tell you now.",
            "Cannot predict now.",
            "Concentrate and ask again.",
            "Don't count on it.",
            "My reply is no.",
            "My sources say no.",
            "Nope.",
            "Outlook not so good.",
            "Very doubtful."
        };

        public FunModule(IServiceProvider serviceprovider)
        {
            fs = serviceprovider.GetService<FunService>();
            udb = serviceprovider.GetService<UserDatabase>();
        }

        [Command("gamble", RunMode = RunMode.Async)]
        [Alias("jackpot")]
        public async Task Gamble(float amount)
        {
            var user = await udb.FindUser(Context.User.Id);
            if (user.lastTip.AddMinutes(5) < DateTime.Now)
            {
                fs.LoadPot();

                if (amount < 10.0 || amount == 0.0) { await ReplyAsync("You have to bet more than 10 credits."); return; }
                if (user.Credits >= amount)
                {
                    await ReplyAsync($"You are gambling {amount} to win the pot of {fs.Pot}. You have a much higher chance of winning if you gamble large numbers. \r\b Gambling...");
                    await Task.Delay(3000);
                    var r = new Random();
                    var n = r.Next(1, 500);
                    if (n <= 10)
                    {
                        await ReplyAsync($"Congratulations! You won! :tada: :tada: {fs.Pot} credits have been added to your account!");
                        user.Credits = user.Credits + fs.Pot;
                        user.CreditsGambled = user.CreditsGambled + amount;
                        user.lastTip = DateTime.Now;
                        fs.Pot = 0;
                        fs.SavePot();
                    }
                    else
                    {
                        await ReplyAsync("Sorry! You lost. Your credits have been added to the pot. Try betting again to see if you can win them back!");
                        user.Credits = user.Credits - amount;
                        user.CreditsGambled = user.CreditsGambled + amount;
                        user.lastTip = DateTime.Now;
                        fs.Pot = fs.Pot + amount;
                        fs.SavePot();
                    }
                }
                else
                {
                    await ReplyAsync("You do not have enough credits.");
                }
            }
            else
            {
                await ReplyAsync("Sorry, due to some issues you can only gamble every 5 minutes. D:");
            }
        }

        [Command("pot")]
        public async Task Pot()
        {
            fs.LoadPot();
            await ReplyAsync(fs.Pot.ToString() + " credits in the pot!");
        }

        [Command("rroulette", RunMode = RunMode.Async)]
        [Alias("rr", "russianroulette", "roulette", "russian")]
        public async Task Roulette(int bullets)
        {
            var us = await udb.FindUser(Context.User.Id);
            if (us.lastTip.AddMinutes(5) < DateTime.Now)
            {
                if (bullets > 0 && bullets < 7)
                {
                    var mess = await ReplyAsync(":gun: Lets see...");
                    await Task.Delay(3000);
                    var r = new Random();
                    if (bullets == 1)
                    {
                        await mess.ModifyAsync(x =>
                      x.Content = ":boom: You died!"
                      );
                        us.lastTip = DateTime.Now;
                        return;
                    }
                    if (r.Next(0, bullets) == 1)
                    {
                        await mess.ModifyAsync(x =>
                        x.Content = ":boom: You died!"
                        );
                    }
                    else
                    {
                        await mess.ModifyAsync(x =>
                        x.Content = ":sweat_smile: You're okay."
                        );
                    }
                }
                else
                {
                    await ReplyAsync("You need to give it a number of 1 through 6");
                }
            }
            else
            {
                await ReplyAsync("Sorry but due to some issues you can only do this once every 5 minutes. I'm so sorry!!!");
            }
        }

        [Command("dick")]
        [Alias("penis")]
        public async Task Dick()
        {
            var us = await udb.FindUser(Context.User.Id);
            string dik = "";
            if (us.DSize != 999)
            {
                for (int i = 0; i < us.DSize; i++)
                {
                    dik = dik + "=";
                }
                await ReplyAsync($"{Context.User.Username}'s dick is **{us.DSize}** inches. Nice dick bro \r\n 8{dik}D");
                return;
            }

            var r = new Random();
            var size = r.Next(0, 90);
            var reroll = r.Next(0, 100);
            int finalsize = 0;

            if (reroll < size)
            {
                finalsize = r.Next(0, 90);
            }
            else
            {
                finalsize = size;
            }
            if (size == 0)
            {
                await ReplyAsync("Your dick is non existant!");
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    dik = dik + "=";
                }
            }
            us.DSize = size;

            await ReplyAsync($"{Context.User.Username}'s dick is **{size}** inches. Nice dick bro \r\n 8{dik}D");
        }

        class ApiResponse {
            public string file;
        }

        [Command("cat")]
        [Alias("dog", "bird", "birb")]
        public async Task Cat()
        {
            WebClient client = new WebClient();

            string s = client.DownloadString("http://random.cat/meow");

            ApiResponse response = JsonConvert.DeserializeObject<ApiResponse>(s);
            await ReplyAsync(response.file);
        }

        [Command("ratemydick")]
        [Alias("ratemypenis", "rmd", "rmp")]
        public async Task ratemyd()
        {
            var us = await udb.FindUser (Context.User.Id);
            if(us != null)
            {
                if (us.DSize == 999)
                {
                    await ReplyAsync("You haven't ran ;dick yet.");
                    return;
                }
                if (us.DSize == 0)
                {
                    await ReplyAsync("heya bud...you uh...don't have a dic....uh...");
                    return;
                }
                var r = new Random();
                await ReplyAsync($"You have a dick {us.DSize} inches long, I rate this {r.Next(0,10)}/10.");
            }
        }

        [Command("flipcoin")]
        [Alias("coin")]
        public async Task Coin()
        {
            await ReplyAsync("yes.");
        }

        class XkcdResponse
        {
            public string month;
            public int num;
            public string link;
            public string year;
            public string news;
            public string safe_title;
            public string transcript;
            public string alt;
            public string img;
            public string title;
            public string day;
        }

        class AdviceResponse
        {
            public Slip slip;
        }

        class Slip
            {
            public string advice;
            public string slip_id;
        }

        [Command("xkcd")]
        public async Task xkcd()
        {
            WebClient client = new WebClient();

            var random = new Random();

            string s = client.DownloadString($"https://xkcd.com/{random.Next(1,1850)}/info.0.json");

            XkcdResponse response = JsonConvert.DeserializeObject<XkcdResponse>(s);
            await ReplyAsync($"{response.title} - {response.img}");
        }

        [Command("advice")]
        public async Task Advice()
        {
            WebClient client = new WebClient();

            var random = new Random();

            string s = client.DownloadString("http://api.adviceslip.com/advice");

            AdviceResponse response = JsonConvert.DeserializeObject<AdviceResponse>(s);
            await ReplyAsync($"{response.slip.advice}");
        }


        [Command("ronswanson")]
        [Alias("ron", "swanson", "rs")]
        public async Task ronswanson()
        {
            WebClient client = new WebClient();

            var random = new Random();

            string s = client.DownloadString("http://ron-swanson-quotes.herokuapp.com/v2/quotes");

            await ReplyAsync($"{s.Replace("[", string.Empty).Replace("]", string.Empty)}");
        }

        [Command("8ball")]
        public async Task eightball([Remainder]string question)
        {
            var r = new Random();
            await ReplyAsync(answers[r.Next(0, answers.Length)]);
        }
    }
}
