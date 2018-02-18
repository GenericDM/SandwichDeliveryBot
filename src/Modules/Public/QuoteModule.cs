using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SandwichDeliveryBot.Databases;
using Discord;
using SandwichDeliveryBot3.CustomClasses;

namespace SandwichDeliveryBot3.Modules
{
    [Group("quote")]
    [Alias("quotes")]
    public class QuoteModule : ModuleBase
    {
        private readonly QuoteDatabase _qdb;
        public QuoteModule(IServiceProvider serviceprovider)
        {
            _qdb = serviceprovider.GetService<QuoteDatabase>();
        }
        [Command, Priority(0)]
        public async Task GetRandomQuote()
        {
            var quotes = _qdb.Quotes.ToArray();
            if (quotes == null)
            {
                await ReplyAsync("No quotes?");
                return;
            }
            var r = new Random();
            var quote = quotes[r.Next(0, quotes.Length)];
            await ReplyAsync($"{quote.QuoteContent} \r\n **Id:** {quote.QuoteNum} - {quote.QuoteDate} - {quote.Creator}");
        }
        [Command, Priority(5)]
        public async Task GetNumberedQuote(int num)
        {
            var quotes = await _qdb.GetArray();
            if (quotes == null)
            {
                await ReplyAsync("No quotes?");
                return;
            }
            var quote = quotes[num - 1];
            if (quote != null)
            {
                await ReplyAsync($"{quote.QuoteContent} \r\n **Id:** {quote.QuoteNum} - {quote.QuoteDate} - {quote.Creator}");
            }
            else
            {
                await ReplyAsync("Cannot find quote!");
            }
        }

        [Command("suggest"), Priority(10)]
        public async Task SuggestQuote(string link)
        {
            IGuild usr = await Context.Client.GetGuildAsync(264222431172886529);
            IGuildUser fires = await usr.GetUserAsync(131182268021604352);
            var dm = await fires.GetOrCreateDMChannelAsync();
            await dm.SendMessageAsync("", embed: new EmbedBuilder() {
                Title = "New quote suggestion",
                Description = $"{Context.User.Username}#{Context.User.Discriminator} - {Context.User.Id} \r\n {link} \r\n {DateTime.Now}",
                Color = new Color(79, 255, 231),
                Timestamp = DateTime.Now
            });
            await ReplyAsync(":thumbsup:");
        }

        [Command("new"), Priority(10)]
        public async Task NewQuote(string link, ulong id)
        {
            if (Context.User.Id == 131182268021604352)
            {
                Quote q = new Quote();
                q.Creator = id;
                q.QuoteContent = link;
                q.QuoteDate = DateTime.Now;
                q.QuoteNum = _qdb.Quotes.Count()  +1;

                await _qdb.NewQuote(q);
                await ReplyAsync(":thumbsup:");
            }
            else
            {
                await ReplyAsync("You are not Fires#1043.");
            }
        }

        [Command("new"), Priority(10)]
        public async Task NewQuote(string link, ulong id, DateTime date)
        {
            if (Context.User.Id == 131182268021604352)
            {
                Quote q = new Quote();
                q.Creator = id;
                q.QuoteContent = link;
                q.QuoteDate = date;
                q.QuoteNum = _qdb.Quotes.Count();
                await _qdb.NewQuote(q);
                await ReplyAsync(":thumbsup:");
            }
            else
            {
                await ReplyAsync("You are not Fires#1043.");
            }
        }

        [Command("del"), Priority(10), Alias("delete")]
        public async Task DeleteQuote(int num)
        {
            await _qdb.RemoveQuote(num);
            await ReplyAsync(":thumbsup:");
        }

        [Command("count"), Priority(10)]
        public async Task QuoteCount(ulong id = 00)
        {
            if (id == 00)
            {
                await ReplyAsync($"Currently we are keeping track of {_qdb.Quotes.Count()} quotes!");
            }
            else
            {
                var a = await _qdb.GetArray();
                var qa = a.Where(x => x.Creator == id);
                if (qa.Count() == 0)
                {
                    await ReplyAsync("This user has no quotes.");
                    return;
                }
                var s = string.Join(", ", qa.Select(x => string.Format("{0}", x.QuoteNum)));
                await ReplyAsync($"Quotes that creator matches with given id: \r\n {s}");
            }
        }
    }
}
