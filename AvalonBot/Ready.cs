using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Discord.Addons.Interactive;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace AvalonBot
{
	public class Ready : ModuleBase<SocketCommandContext>
    {
		private static List<ulong> readylist = new List<ulong>();
		[Command ("test"), RequireOwner]
		public async Task Test()
        {
			EmbedBuilder test = new EmbedBuilder();
			test.WithDescription("Hello World");
			await ReplyAsync("", false, test.Build());
        }
		[Command("ready")]
		public async Task ReadyAsync()
		{
			ulong id = Context.User.Id;
			if (!readylist.Contains(id))
			{
				await ReplyAsync($"{Context.User.Mention} has readied up.");
				readylist.Add(id);
			}
			else
			{
				string message = "";
				EmbedBuilder readyBuilder = new EmbedBuilder();
				readylist = readylist.OrderBy(i => Guid.NewGuid()).ToList();
				int i = 1;
				foreach (ulong name in readylist)
				{
					message += $"{i}. <@{name}> \n";
					i++;
				}
				readyBuilder.WithDescription(message);
				await ReplyAsync("", false, readyBuilder.Build());
			}
		}
		[Command("unready")]
		public async Task UnreadyAsync()
		{
			ulong id = Context.User.Id;
			if (readylist.Contains(id))
            {
				readylist.Remove(id);
            }
			await ReplyAsync($"{Context.User.Mention} has unreadied.");
        }
    }
}

