using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using SandwichDeliveryBot3.CustomClasses;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Discord.WebSocket;
using Discord;
using System.Linq;
using SandwichDeliveryBot3.Enums;

namespace SandwichDeliveryBot.Databases
{
    public class UserDatabase : DbContext
    {
        public DbSet<SandwichUser> Users { get; set; }
        private IServiceProvider _provider;
        private ArtistDatabase _ADB;
        public UserDatabase(IServiceProvider provider)
        {
            _provider = provider;
            _ADB = _provider.GetService<ArtistDatabase>();
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string baseDir = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(baseDir))
                Directory.CreateDirectory(baseDir);

            string datadir = Path.Combine(baseDir, "users.sqlite.db");
            optionsBuilder.UseSqlite($"Filename={datadir}");
        }

        public async Task<SandwichUser> FindUser(ulong id)
        {
            try
            {
                var a = await Users.FirstOrDefaultAsync(x => x.Id == id);
                if (a != null) { return a; } else { return null; }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public async Task CreateNewUser(SocketUser us)
        {
            Console.WriteLine($"Creating a new user profile for {us.Username}.");
            SandwichUser u = new SandwichUser(us);
            await Users.AddAsync(u);
            await SaveChangesAsync();
        }
        public async Task CreateNewUser(IGuildUser us)
        {
            Console.WriteLine($"Creating a new user profile for {us.Username}.");
            SandwichUser u = new SandwichUser(us);
            await Users.AddAsync(u);
            await SaveChangesAsync();
        }
  

        public async Task UpOrders(ulong id)
        {
            var a = await Users.FirstOrDefaultAsync(x => x.Id == id);
            a.Orders += 1;
            await SaveChangesAsync();
        }
        public async Task UpDenials(ulong id)
        {
            var a = await Users.FirstOrDefaultAsync(x => x.Id == id);
            a.Denials += 1;
            await SaveChangesAsync();
        }
       
        public async Task ChangeOrders(ulong id, int c)
        {
            SandwichUser us = await FindUser(id);
            if (us != null)
            {
                us.Orders = c;
                await SaveChangesAsync();
            }
        }
        public async Task ChangeDenials(ulong id, int c)
        {
            SandwichUser us = await FindUser(id);
            if (us != null)
            {
                us.Denials = c;
                await SaveChangesAsync();
            }
        }

        public async Task GiveUserCredits(SandwichUser u, float credits)
        {
            u.Credits += credits;
            await SaveChangesAsync();
        }
        public async Task RemoveUserCredits(SandwichUser u, float credits)
        {
            u.Credits -= credits;
            await SaveChangesAsync();
        }
        public async Task GiveUserCredits(IGuildUser us, float credits)
        {
            SandwichUser u = await FindUser(us.Id);
            u.Credits += credits;
            await SaveChangesAsync();
        }
        public async Task RemoveUserCredits(IGuildUser us, float credits)
        {
            SandwichUser u = await FindUser(us.Id);
            u.Credits -= credits;
            await SaveChangesAsync();
        }
        public async Task UpdateCreditDate(SandwichUser us)
        {
            SandwichUser u = await FindUser(us.Id);
            u.lastDailyCredits= DateTime.Now;
            await SaveChangesAsync();
        }

        //public async Task AddTips(SandwichUser user, int amount)
        //{
        //    SandwichUser u = await Users.FirstOrDefaultAsync(x => x.Id == user.Id);
        //    u.Tips += amount;
        //    await SaveChangesAsync();
        //}
        //public async Task ChangeTips(SandwichUser send, SandwichUser rec)
        //{
        //    var s = await Users.FirstOrDefaultAsync(x => x.Id == send.Id);
        //    var a = await _ADB.FindArtist(rec.Id);
        //    if (a != null)
        //    {
        //        s.Tips -= 1;
        //        a.tipsRecieved += 1;
        //        await _ADB.SaveChangesAsync();
        //        await SaveChangesAsync();
        //    }
        //}
    }
}
