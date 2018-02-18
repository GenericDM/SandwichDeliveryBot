using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using SandwichDeliveryBot3.CustomClasses;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace SandwichDeliveryBot.Databases
{
    public class BroadcastDatabase : DbContext
    {
        public DbSet<BroadcastableGuild> BroadcastableGuilds { get; set; }
        private IServiceProvider _provider;
        public BroadcastDatabase(IServiceProvider provider)
        {
            _provider = provider;
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "broadcast.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public async Task NewGuild(ulong id, ulong cid)
        {
            BroadcastableGuild bg = new BroadcastableGuild(id, cid);
            await BroadcastableGuilds.AddAsync(bg);
            await SaveChangesAsync();
        }

        public async Task RemoveGuild(ulong id)
        {
            BroadcastableGuild bg = await BroadcastableGuilds.FirstOrDefaultAsync(x => x.GuildID == id);
            if (bg != null)
            {
                BroadcastableGuilds.Remove(bg);
                return;
            }
            throw new Exception("Guild not located in database.");
        }

        public async Task ChangeChannel(ulong id, ulong newchannel)
        {
            BroadcastableGuild bg = await BroadcastableGuilds.FirstOrDefaultAsync(x => x.GuildID == id);
            if (bg != null)
            {
                bg.BroadcastChannelID = newchannel;
                await SaveChangesAsync();
                return;
            }
            throw new Exception("Guild not located in database.");
        }

        public BroadcastableGuild[] ToArray()
        {
            return BroadcastableGuilds.ToArray();
        }
    }
}