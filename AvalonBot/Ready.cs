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
		private string GetReadyMsg(List<ulong> ready)
        {
			string message = "";
			int i = 1;
			foreach (ulong name in ready)
			{
				message += $"{i}. <@{name}> \n";
				i++;
			}
			return message;
		}
		[Command ("test"), RequireOwner]
		public async Task Test()
        {
			EmbedBuilder test = new EmbedBuilder();
			test.WithDescription("Hello World");
			await ReplyAsync("", false, test.Build());
        }
		[Command("ready")]
		public async Task ReadyAsync(IUser user=null)
		{
			if(user==null)
            {
				user = Context.User;
            }
			ulong id = user.Id;
			if (!readylist.Contains(id))
			{
				await ReplyAsync($"{user.Mention} has readied up.");
				readylist.Add(id);
			}
			else
			{
				EmbedBuilder readyBuilder = new EmbedBuilder();
				string message = GetReadyMsg(readylist);
				readyBuilder.WithDescription(message);
				await ReplyAsync("", false, readyBuilder.Build());
			}
		}
		[Command("unready")]
		public async Task UnreadyAsync(IUser user = null)
		{
			if (user == null)
			{
				user = Context.User;
			}
			ulong id = user.Id;
			if (readylist.Contains(id))
            {
				readylist.Remove(id);
				await ReplyAsync($"{user.Mention} has unreadied.");
			}
			else
            {
				await ReplyAsync($"{user.Mention} is already unreadied.");
			}
        }
		[Command("shuffle")]
		public async Task ShuffleAsync()
		{
			List<ulong> shuffledlist = readylist.OrderBy(i => Guid.NewGuid()).ToList();
			EmbedBuilder readyBuilder = new EmbedBuilder();
			string message = GetReadyMsg(shuffledlist);
			readyBuilder.WithDescription(message);
			await ReplyAsync("", false, readyBuilder.Build());
		}
	}
}

