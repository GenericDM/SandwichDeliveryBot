using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using SandwichDeliveryBot.Databases;
using SandwichDeliveryBot.SService;
using SandwichDeliveryBot.OrderStatusEnum;
using SandwichDeliveryBot3.Precons;
using SandwichDeliveryBot3.CustomClasses;
using SandwichDeliveryBot3.Enums;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Discord.WebSocket;
using SandwichDeliveryBot3.Services;

namespace SandwichDeliveryBot3.Modules.Public
{
    public class OrderModule : ModuleBase
    {
        SandwichService _SS;
        SandwichDatabase _DB;
        ArtistDatabase _ADB;
        ListingDatabase _LDB;
        UserDatabase _UDB;
        QueueService _QS;


        public OrderModule(IServiceProvider provider)
        {
            _SS = provider.GetService<SandwichService>();
            _DB = provider.GetService<SandwichDatabase>();
            _ADB = provider.GetService<ArtistDatabase>();
            _LDB = provider.GetService<ListingDatabase>();
            _UDB = provider.GetService<UserDatabase>();
            _QS = provider.GetService<QueueService>();
        }


        [Command("jump")]
        [Alias("j")]
        public async Task Jump()
        {
            using (Context.Channel.EnterTypingState())
            {
                IUserMessage msg = await ReplyAsync("Attempting to skip queue.");
                try
                {
                    var u = await  _UDB.FindUser(Context.User.Id);
                    if (u != null)
                    {
                        if (!u.IsPatron)
                        {
                            await ReplyAsync("You are not a Patron. Pledge to our(Fires) Patreon by using the `;patreon` command, then DMing Fires#1060."); return;
                        }
                    }
                    else
                    {
                        await ReplyAsync("You aren't registered."); return;
                    }
                    Sandwich order = await _DB.FindOrder(Context.User.Id);
                    if (order == null) { await ReplyAsync("Order doesn't exist."); return; }
                    _QS.JumpOrder(order);
                    IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                    ITextChannel usrc = await usr.GetTextChannelAsync(_SS.KitchenId);
                    await usrc.SendMessageAsync($"Order `{order.Id}` jumped to front of queue.");
                    await msg.ModifyAsync(x =>
                    {
                        x.Content = "Successfully jumped order! Thank you, Patron :wink:";
                    });
                }
                catch (Exception e)
                {
                    await msg.ModifyAsync(x =>
                    {
                        x.Content = "Failed to jump. Are you sure you have one? If this issue persists contact Fires#1060.";
                    });
                    Console.WriteLine(e);
                }
            }
        }


        [Command("nextorder")]
        [Alias("no")]
        public async Task NO()
        {
            var o = _QS.GetNextOrder();
            await ReplyAsync($"The next order in queue is `{o.Id}`");
        }

        [Command("getallorders")]
        [Alias("gao")]
        [RequireBlacklist]
        public async Task Gao()
        {
            var waiting = _DB.Sandwiches.Where(x => x.Status == OrderStatus.Waiting);
            var queue = _DB.Sandwiches.Where(x => x.Status == OrderStatus.ReadyToDeliver);
            string cashier = string.Join(",", queue.Select(x => string.Format("`{0}`", x.Id)).ToArray());
            string wait = string.Join(",", waiting.Select(x => string.Format("`{0}`", x.Id)).ToArray());


            await ReplyAsync($"**Orders:**\r\nWaiting:\r\n{wait}\r\nReady for Delivery\r\n{cashier}");
        }

        [Command("order")]
        [Alias("o")]
        [NotBlacklisted]
        [RequireBotPermission(GuildPermission.CreateInstantInvite)]
        public async Task Order([Remainder]string order)
        {
            using (Context.Channel.EnterTypingState())
            {
                if (order.Length > 1)
                {
                    var outp = _DB.CheckForExistingOrders(Context.User.Id);
                    if (outp) { await ReplyAsync("You already have an order!"); return; }

                    SandwichUser u = await _UDB.FindUser(Context.User.Id);
                    u.Level = u.Orders / 10;

                    if (u == null)
                    {
                        await _UDB.CreateNewUser((SocketUser)Context.User);
                        await ReplyAsync("You've been registered!");
                        u = await _UDB.FindUser(Context.User.Id);
                    }

                    if (u.Credits < 1.0) { await ReplyAsync("You will need atleast 1 credit for this."); return; }

                    string orderid;

                    try
                    {
                        IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId); //Isn't there a better way to do these?
                        ITextChannel usrc = await usr.GetTextChannelAsync(_SS.KitchenId);
                        ITextChannel usrclog = await usr.GetTextChannelAsync(_SS.LogId);

                        orderid = _DB.GenerateId(3);
                        orderid = _DB.VerifyIdUniqueness(orderid);

                        var neworder = await _DB.NewOrder(order, orderid, DateTime.Now, OrderStatus.Waiting, Context);
                        _QS.AddOrder(neworder);
                        var builder = new EmbedBuilder();
                        builder.ThumbnailUrl = Context.User.GetAvatarUrl();
                        builder.Title = $" New order from {Context.Guild.Name} (`{Context.Guild.Id}`)";
                        var desc = $"Ordered by: **{Context.User.Username}#{Context.User.Discriminator}** (`{Context.User.Id}`)\n" +
                           $"Id: `{orderid}`\n" +
                           $"```{order}```";
                        builder.Description = desc;
                        builder.Color = new Color(71, 120, 198);
                        builder.WithFooter(x =>
                        {
                            x.Text = "Is this order abusive? Please ping @Artist Manager immediately!";
                        });
                        builder.Timestamp = DateTime.Now;
                        _SS.totalOrders += 1;

                        

                        var artist = usr.Roles.FirstOrDefault(x => x.Name.ToLower() == "sandwich artist"); //FIX
                        if (artist != null)
                        {
                            await usrc.SendMessageAsync($"{artist.Mention}", embed: builder);
                        }
                        else
                        {
                            await usrc.SendMessageAsync($" ", embed: builder);
                        }

                    }
                    catch (Exception e)
                    {
                        await ReplyAsync("This error should not happen! Contact Fires#1043 immediately!");
                        Console.WriteLine(e);
                        await ReplyAsync($"```{e}```");
                        return;
                    }

                    IDMChannel dm = await Context.User.GetOrCreateDMChannelAsync();
                    IUserMessage m = await ReplyAsync(":thumbsup:");
                    try
                    {
                        await dm.SendMessageAsync($"Thank you for ordering. Please wait while an artist accepts you. :slight_smile: - ID `{orderid}`");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        await m.ModifyAsync(msg =>
                        {
                            msg.Content = $":thumbsdown: {Context.User.Mention} We failed to dm you. You're order has been automatically deleted. Please enable DMs and re order. http://i.imgur.com/vY7tThf.png OR http://i.imgur.com/EtaA78Q.png";
                        });
                        IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                        ITextChannel usrc = await usr.GetTextChannelAsync(_SS.KitchenId);
                        await usrc.SendMessageAsync($"**IGNORE ORDER {orderid} AS IT HAS BEEN REMOVED**");
                    }
                }
                else { await ReplyAsync("Your order must be longer then 2 characters."); }
            }
        }

        [Command("delorder")]
        [Alias("deleteorder")]
        [NotBlacklisted]
        public async Task DelOrder()
        {
            using (Context.Channel.EnterTypingState())
            {
                IUserMessage msg = await ReplyAsync("Attempting to delete order...");
                try
                {
                    Sandwich order = await _DB.FindOrder(Context.User.Id);
                    if (order == null) { await ReplyAsync("That is not a valid order."); return; }
                    await _DB.DelOrder(order.Id.ToLower());
                    _QS.RemoveOrder(order);
                    IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                    ITextChannel usrc = await usr.GetTextChannelAsync(_SS.KitchenId);
                    await usrc.SendMessageAsync($"Order `{order.Id}`,`{order.Desc}` has been deleted.");
                    _SS.totalOrders -= 1;
                    await msg.ModifyAsync(x =>
                    {
                        x.Content = "Successfully deleted order!";
                    });
                }
                catch (Exception e)
                {
                    await msg.ModifyAsync(x =>
                    {
                        x.Content = "Failed to delete. Are you sure you have one? If this issue persists contact Fires#1043.";
                    });
                    Console.WriteLine(e);
                }
            }
        }


        [Command("acceptorder")]
        [Alias("ao")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task AcceptOrder(string id)
        {
            using (Context.Channel.EnterTypingState())
            {
                try
                {
                    Artist a = await _ADB.FindArtist(Context.User.Id);
                    Sandwich o = await _DB.FindOrder(id);
                    SandwichUser user = await _UDB.FindUser(o.UserId);
                    if (o == null) { await ReplyAsync("That id isn't correct bud."); return; };
                    if (o.Status != OrderStatus.Waiting) { await ReplyAsync("This order is not available. :angry: "); return; }

                    IGuild s = await Context.Client.GetGuildAsync(o.GuildId);
                    ITextChannel ch = await s.GetTextChannelAsync(o.ChannelId);
                    IGuildUser u = await s.GetUserAsync(o.UserId);

                    IDMChannel dm = await u.GetOrCreateDMChannelAsync();
                    _QS.AcceptOrder();
                    o.Status = OrderStatus.ReadyToDeliver;
                    o.ArtistId = Context.User.Id;
                    await _ADB.ChangeAcceptCount(a, ArtistStatChange.Increase);

                    await ch.SendMessageAsync($"{u.Mention}, a Sandwich Artist has accepted your sandwich. It will be delivered soon.");
                    await ReplyAsync($":thumbsup:");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

            }
        }

        [Command("deliver")]
        [Alias("d")]
        [NotBlacklisted]
        [inUSR]
        [RequireSandwichArtist]
        public async Task Deliver(string id)
        {
            _SS.Save();
            using (Context.Channel.EnterTypingState())
            {
                Sandwich o = await _DB.FindOrder(id);
                if (o.ArtistId == Context.User.Id)
                {
                    if (o.Status == OrderStatus.ReadyToDeliver)
                    {
                        Artist a = await _ADB.FindArtist(Context.User.Id);
                        //Collect variables
                        await ReplyAsync($"{Context.User.Mention} DMing you an invite! Go deliver it! Remember to be nice and ask for `;feedback`.");
                        IGuild s = await Context.Client.GetGuildAsync(o.GuildId);
                        SandwichUser user = await _UDB.FindUser(o.UserId);
                        await _UDB.UpOrders(user.Id);
                        ITextChannel ch = await s.GetTextChannelAsync(o.ChannelId);
                        IGuildUser u = await s.GetUserAsync(o.UserId);
                        IDMChannel dm = await u.GetOrCreateDMChannelAsync();
                        //Create Invite
                        IInvite inv = await ch.CreateInviteAsync(1800, 1, false, true);
                        IDMChannel artistdm = await Context.User.GetOrCreateDMChannelAsync();
                        //Build embed
                        var builder = new EmbedBuilder();
                        builder.ThumbnailUrl = o.AvatarUrl;
                        builder.Title = $"Your order is being delivered by {Context.User.Username}#{Context.User.Discriminator}!";
                        var desc = $"```{o.Desc}```\n" +
                                   $"**Incoming sandwich! Watch {o.GuildName}!**";
                        builder.Description = desc;
                        builder.Color = new Color(255, 181, 10);
                        builder.WithFooter(x =>
                        {
                            x.IconUrl = u.GetAvatarUrl();
                            x.Text = $"Ordered at: {o.date}.";
                        });
                        builder.Timestamp = DateTime.UtcNow;
                        await dm.SendMessageAsync($"Your sandwich is being delivered soon! Watch out!", embed: builder);
                        //Finish up
                        await artistdm.SendMessageAsync("Invite: " + inv.ToString() +" \r\n Name: "+o.UserName);
                        o.Status = OrderStatus.Delivered;
                        await _ADB.UpdateMostRecentOrder(a);
                        await _UDB.GiveUserCredits(Context.User as IGuildUser, 5.0f);
                        await _DB.DelOrder(id);

                    }
                    else
                    {
                        await ReplyAsync("This order is not ready to be delivered yet.");
                    }
                }
                else
                {
                    await ReplyAsync("You have not claimed this order!");
                }
            }
        }

        [Command("denyorder")]
        [Alias("do")]
        [NotBlacklisted]
        [inUSR]
        public async Task DenyOrder(string id, [Remainder] string reason)
        {
            try
            {
                Sandwich order = await _DB.FindOrder(id);
                if (order == null) { await ReplyAsync("No order has that id."); return; }
                order.Status = OrderStatus.Delivered;
                await _DB.DelOrder(id);
                _QS.Skip();
                await ReplyAsync($"{Context.User.Mention} has deleted order {order.Id}.");
                IGuild s = await Context.Client.GetGuildAsync(order.GuildId);
                ITextChannel ch = await s.GetTextChannelAsync(order.ChannelId);
                IGuildUser u = await s.GetUserAsync(order.UserId);
                IDMChannel dm = await u.GetOrCreateDMChannelAsync();
                Artist a = await _ADB.FindArtist(Context.User.Id);
                await _ADB.ChangeAcceptCount(a, ArtistStatChange.Decrease);
                await _ADB.ChangeDenyCount(a);
                SandwichUser user = await _UDB.FindUser(order.UserId);
                await _UDB.UpDenials(user.Id);
                await dm.SendMessageAsync($"Your sandwich order has been deleted! ", embed: new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl())
                    .WithUrl("https://discord.gg/DmGh9FT")
                    .AddField(builder =>
                    {
                        builder.Name = "Order:";
                        builder.Value = order.Desc;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Denied By:";
                        builder.Value = string.Join("#", Context.User.Username, Context.User.Discriminator);
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Denied because:";
                        builder.Value = reason;
                        builder.IsInline = true;
                    })
                    .AddField(builder =>
                    {
                        builder.Name = "Order Id:";
                        builder.Value = order.Id;
                        builder.IsInline = true;
                    })
                    .WithCurrentTimestamp()
                    .WithTitle("Denied order:"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        [Command("orderinfo")]
        [Alias("oi")]
        [NotBlacklisted]
        [RequireSandwichArtist]
        public async Task OrderInfo(string id)
        {
            Sandwich order = await _DB.FindOrder(id);
            Artist art = await _ADB.FindArtist(order.ArtistId);
                Color c = new Color(42, 249, 153);
                await ReplyAsync($"{Context.User.Mention} Here is the info:", embed: new EmbedBuilder()
                .AddField(builder =>
                {
                    builder.Name = "Order";
                    builder.Value = order.Desc;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Order Id";
                    builder.Value = order.Id;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Order Server";
                    builder.Value = order.GuildName;
                    builder.IsInline = true;
                })
                .AddField(builder =>
                {
                    builder.Name = "Customer";
                    builder.Value = order.UserName + "#" + order.Discriminator;
                    builder.IsInline = true;
                })
                 .AddField(builder =>
                 {
                     builder.Name = "Order Status";
                     builder.Value = order.Status;
                     builder.IsInline = true;
                 })
                .WithUrl("https://discord.gg/DmGh9FT")
                .WithColor(c)
                .WithThumbnailUrl(order.AvatarUrl)
                .WithTitle("Order information")
                .WithTimestamp(DateTime.Now));
            
        }

        [Command("feedback")]
        [Alias("f")]
        [NotBlacklisted]
        public async Task Feedback([Remainder]string f)
        {
            if (f != null)
            {
                try
                {
                    IGuild usr = await Context.Client.GetGuildAsync(_SS.USRGuildId);
                    ITextChannel usrc = await usr.GetTextChannelAsync(358637402576912385);

                    var builder = new EmbedBuilder();
                    builder.ThumbnailUrl =Context.User.GetAvatarUrl();
                    builder.Title = $"New feedback from {Context.User.Username}#{Context.User.Discriminator}(`{Context.User.Id}`)";
                    var desc = $"{f}";
                    builder.Description = desc;
                    builder.Color = new Color(244, 155, 66);
                    builder.WithFooter(x =>
                    {
                        x.Text = "Is this feedback abusive? Please tell Fires immediately!";
                    });
                    builder.Timestamp = DateTime.Now;

                    await usrc.SendMessageAsync(" ", embed: builder);
                    await ReplyAsync("Thank you!");
                }
                catch (Exception e)
                {
                    await ReplyAsync($"Error! {e}");
                }
            }
            else
            {
                await ReplyAsync("Please enter something!");
            }
        }
    }
}