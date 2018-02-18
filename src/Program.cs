using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Linq;
using SandwichDeliveryBot.Handler;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot.Databases;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Newtonsoft.Json;
using SandwichDeliveryBot3.CustomClasses;
using SandwichDeliveryBot3.Services;

namespace SandwichDeliveryBot
{
    public class Program
    {
        // Convert our sync main to an async main.
        public static void Main(string[] args) =>
            new Program().Start().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        private CommandHandler handler;

        private SandwichService ss;
        private BroadcastDatabase bdb;
        private UserDatabase udb;

                    

        public async Task Start()
        {
            // Define the DiscordSocketClient
            client = new DiscordSocketClient();


            var token = "nope. Bad idea that I stored it in plaintext tho lol.";

            ss = new SandwichService();

            client.Log += async (message) =>
            {
                await Log(message);
            };

            client.JoinedGuild += async (m) =>
            {
                IGuild usr =  client.GetGuild(ss.USRGuildId);
                ITextChannel usrc = await usr.GetTextChannelAsync(ss.LogId);
                var e = new EmbedBuilder();
                e.Color = new Color(94, 180, 255);
                e.ThumbnailUrl = m.IconUrl;
                e.Timestamp = DateTime.Now;
                e.Description = $"I have joined **{m.Name}**(`{m.Id}`) \r\n" +
                $"That is **{m.Users.Count}** new users. \r\n" +
                $"I am now in **{client.Guilds.Count}** servers!";
                await usrc.SendMessageAsync("", embed: e);

                var textchannel = m.GetTextChannel(m.Channels.Where(c => m.CurrentUser.GetPermissions(c).SendMessages)
                .OrderBy(c => c.Position)
                .FirstOrDefault().Id);
                ss.Load();
                await textchannel.SendMessageAsync(ss.motd);
                await bdb.NewGuild(m.Id, textchannel.Id);
                await bdb.SaveChangesAsync();
                
                if (!m.CurrentUser.GuildPermissions.CreateInstantInvite)
                {
                    await m.DefaultChannel.SendMessageAsync(":warning: The bot cannot create instant invites! Please give the bot permission or else it will not work! :warning: ");
                }
                

            };

            client.LeftGuild += async (m) =>
            {
                await bdb.RemoveGuild(m.Id);
                IGuild usr = client.GetGuild(ss.USRGuildId);
                ITextChannel usrc = await usr.GetTextChannelAsync(ss.LogId);
                var e = new EmbedBuilder();
                e.Color = new Color(232, 34, 34);
                e.ThumbnailUrl = m.IconUrl;
                e.Timestamp = DateTime.Now;
                e.Description = $"I have left **{m.Name}**(`{m.Id}`)! \r\n" +
                $"That is **{m.Users.Count}** less users. \r\n" +
                $"I am now in **{client.Guilds.Count}** servers...";
                await usrc.SendMessageAsync("", embed: e);

            };

            client.MessageReceived += async (m) =>
            {
                if (m.Author.IsBot) return;
                if (m.Channel.Id == client.CurrentUser.Id) return;
                if (m.MentionedUsers.Any(u => u.Id == client.CurrentUser.Id))
                {
                    if (m.Content.Contains("help"))
                    {
                        await m.Channel.SendMessageAsync("Type `;help`. `;server` if you have any problems.");
                    }
                }
                var rnd = new Random();
                var r = rnd.Next(1, 25);
                if (r == 2)
                {
                    var totalChannels = 0;
                    var totalUsers = 0;
                    foreach (var obj in client.Guilds)
                    {
                        totalChannels = totalChannels + obj.Channels.Count;
                        totalUsers = totalUsers + obj.Users.Count;
                    }
                    await client.SetGameAsync($"{totalChannels} channels with a total of {totalUsers} users. | Type ;motd for help");
                }
                if (r == 6)
                {
                    await client.SetGameAsync($" {client.Guilds.Count()} servers. | Type ;motd for help");
                }

            };
      

            var serviceProvider = ConfigureServices();


            //var timer = new System.Threading.Timer(async (e) =>
            //{
            //    await VerifyTransactionsAsync();
            //}, null, 0, (int)TimeSpan.FromMinutes(0.5).TotalMilliseconds);


            // Login and connect to Discord.

            handler = new CommandHandler();
            await handler.Install(serviceProvider);
            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            // Block this program until it is closed.
            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Critical:
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogSeverity.Verbose:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            Console.WriteLine($@"[{msg.Severity}] {msg.ToString()}");
            Console.ResetColor();
            return Task.CompletedTask;
        }


        //I'm so sorry about this function. It was late when I made it and  I didnt care.
        private IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                .AddSingleton(client)
                .AddSingleton(new CommandService())
                .AddSingleton<SandwichService>()
                .AddSingleton<SandwichDatabase>()
                .AddSingleton<StatService>()
                .AddSingleton<ListingDatabase>()
                .AddSingleton<UserDatabase>()
                .AddSingleton<QueueService>()
                .AddDbContext<QuoteDatabase>()
                .AddSingleton<FunService>()
                //.AddSingleton<TipDatabase>()
                .AddSingleton<BroadcastDatabase>()
                .AddSingleton<ArtistDatabase>();
            var provider = new DefaultServiceProviderFactory().CreateServiceProvider(services);
            provider.GetService<SandwichService>().Load();
            provider.GetService<FunService>().LoadPot();
            bdb = provider.GetService<BroadcastDatabase>();
            udb = provider.GetService<UserDatabase>();
            SandwichDatabase _DB;
            ArtistDatabase _ADB;
            ListingDatabase _LDB;
            UserDatabase _UDB;
            //TipDatabase _TDB;
            BroadcastDatabase _BDB;
            QueueService q = provider.GetService<QueueService>();
            _DB = provider.GetService<SandwichDatabase>();
            _ADB = provider.GetService<ArtistDatabase>();
            _LDB = provider.GetService<ListingDatabase>();
            _UDB = provider.GetService<UserDatabase>();
            //_TDB = provider.GetService<TipDatabase>();
            _BDB = provider.GetService<BroadcastDatabase>();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Loaded {_DB.Sandwiches.ToArray().Count()} orders.");
            Console.WriteLine($"Loaded {_ADB.Artists.ToArray().Count()} artists.");
            Console.WriteLine($"Loaded {_LDB.Listings.ToArray().Count()} listings.");
            Console.WriteLine($"Loaded {_UDB.Users.ToArray().Count()} users.");
            //Console.WriteLine($"Loaded {_TDB.Tips.ToArray().Count()} tips.");
            Console.WriteLine($"Loaded {_BDB.BroadcastableGuilds.ToArray().Count()} broadcastable guilds.");
            return provider;
        }

        //public async Task VerifyTransactionsAsync()
        //{
        //    RestClient client = new RestClient(" https://discoin.disnodeteam.com");
        //    RestRequest request = new RestRequest("/transaction", Method.GET);

        //    request.AddHeader("Authorization", "A42D4911CACA52922BFAD61E44D71");
        //    IRestResponse response = client.Execute(request);

        //    try
        //    {
        //        Transaction[] transactions = JsonConvert.DeserializeObject<Transaction[]>(response.Content);

        //        if (transactions.Count() != 0)
        //        {
        //            foreach (Transaction t in transactions)
        //            {
        //                SandwichUser u = await udb.FindUser(t.UserId);
        //                if (u == null)
        //                    return;

        //                u.Credits += t.Amount;
        //                await udb.SaveChangesAsync();
        //                Console.WriteLine($"Deposited {t.Amount} credits into {u.Name}'s account.");
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine($"Failed to verify transactions. ");

        //    }
        //}
    }
}
