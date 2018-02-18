using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Runtime.InteropServices;
using System.Diagnostics;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot3.Precons;
using Microsoft.Extensions.DependencyInjection;
using SandwichDeliveryBot3.Services;

namespace SandwichDeliveryBot.UtilityMod
{
    public class StatModule : ModuleBase
    {
        private StatService _SS;

        public StatModule(IServiceProvider provider)
        {
            _SS = provider.GetService<StatService>();
        }


        [Command("statistics")]
        public async Task Stats()
        {
            await ReplyAsync($@"  
Orders today: {_SS.OrdersToday.Count}
Orders this week: {_SS.OrdersThisWeek.Count}
Orders this month: {_SS.OrdersThisMonth.Count}
---
Reset today: {_SS.EndOfToday}
Reset week: {_SS.EndOfWeek}
Reset month: {_SS.EndOfMonth}");
        }

        [Command("savestats")]
        public async Task save()
        {
            _SS.OutputStats();
            await ReplyAsync("got it");
        }
    }
}