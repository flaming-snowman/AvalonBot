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
		private static readonly Random random = new Random();
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
				readyBuilder.WithDescription(message)
							.WithTitle("List of Everyone Ready");
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
		[Command("listready")]
		public async Task ListReadyAsync()
		{
			EmbedBuilder readyBuilder = new EmbedBuilder();
			string message = GetReadyMsg(readylist);
			readyBuilder.WithDescription(message)
						.WithTitle("List of Everyone Ready");
			await ReplyAsync("", false, readyBuilder.Build());
		}
		[Command("shuffle")]
		public async Task ShuffleAsync()
		{
			List<ulong> shuffledlist = readylist.OrderBy(i => Guid.NewGuid()).ToList(); //cheese shuffling algorithm
			EmbedBuilder readyBuilder = new EmbedBuilder();
			string message = GetReadyMsg(shuffledlist);
			readyBuilder.WithDescription(message)
						.WithTitle("Shuffled!")
						.WithColor(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
			await ReplyAsync("", false, readyBuilder.Build());
		}
	}
}

