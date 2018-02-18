using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using SandwichDeliveryBot3.CustomClasses;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;

namespace SandwichDeliveryBot.Databases
{
    public class QuoteDatabase : DbContext
    {
        public DbSet<Quote> Quotes { get; set; }
        private IServiceProvider _provider;
        public QuoteDatabase(IServiceProvider provider)
        {
            _provider = provider;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "quotes.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public async Task NewQuote(Quote q)
        {
            try
            {
                await Quotes.AddAsync(q);
                await SaveChangesAsync();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task RemoveQuote(int id)
        {
            Quote l = await Quotes.FirstOrDefaultAsync(x => x.QuoteNum == id);
            if (l != null)
            {
                Quotes.Remove(l);
                await SaveChangesAsync();
            }
            else
            {
                throw new CantFindInDatabaseException();
            }
        }

        public async Task<Quote[]> GetArray()
        {
            return await Quotes.ToArrayAsync();
        }
    }
}