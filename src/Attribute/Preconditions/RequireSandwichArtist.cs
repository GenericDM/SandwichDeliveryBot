﻿using System;
using System.Threading.Tasks;
using Discord.Commands;
using System.Linq;
using Discord.WebSocket;

namespace SandwichDeliveryBot3.Precons
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    class RequireSandwichArtist : PreconditionAttribute
    {
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider provider)
        {
            var user = context.User as SocketGuildUser;
            if (user == null)
                return Task.FromResult(PreconditionResult.FromError("The command was not used in a guild."));
            
             var matchingRoles = context.Guild.Roles.Where(role => role.Id == 358643732414988288);

            if (matchingRoles == null)
                return Task.FromResult(PreconditionResult.FromError("There are no matching roles on the server."));

            if (user.Roles.Any(role => matchingRoles.Contains(role)))
            {
                return Task.FromResult(PreconditionResult.FromSuccess());
            }

            return Task.FromResult(PreconditionResult.FromError("User does not have sandwich roles."));


        }
    }
}
