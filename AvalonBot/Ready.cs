using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
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
		[Command("clear"), RequireOwner]
		public async Task ClearAsync()
		{
			ClearList();
			await ReplyAsync("Ready list cleared");
		}
		public static void ClearList()
		{
			readylist.Clear();
		}
	}
	public static class ReadyAnnounce
    {
		private static SocketGuild guild;
		internal static void CheckTime(SocketGuild sguild)
		{
			guild = sguild;
			//Time when method needs to be called
			var Today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 20, 55, 0);
			var NextThurs = Today.AddDays(((int)DayOfWeek.Thursday - (int)Today.DayOfWeek+7)%7);
			var NextSun = Today.AddDays(((int)DayOfWeek.Sunday - (int)Today.DayOfWeek+7)%7);

			if(NextThurs < DateTime.Now)
            {
				NextThurs=NextThurs.AddDays(7);
            }
			if (NextSun < DateTime.Now)
			{
				NextSun = NextSun.AddDays(7);
			}

			DateTime NextTime;
			if(NextThurs<NextSun)
            {
				NextTime = NextThurs;
            }
            else
            {
				NextTime = NextSun;
            }

			TimeSpan ts = NextTime - DateTime.Now;
			//waits certan time and run the code
			Timer ReadyTimer = new Timer(ts.TotalMilliseconds);
			ReadyTimer.AutoReset = false;
			ReadyTimer.Start();
			Console.WriteLine($"Timer started for {(DateTime.Now + ts).DayOfWeek}, {DateTime.Now + ts}");
			ReadyTimer.Elapsed += ReadyTimer_Elapsed;
		}
		private static async void ReadyTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			var channel = guild.TextChannels.FirstOrDefault(x => x.Name == "avalon");
			var role = guild.Roles.FirstOrDefault(x => x.Name == "Avalon");
			await channel.SendMessageAsync($"{role.Mention}, Avalon starts in 5 minutes. Ready up now.");
			Ready.ClearList();
			CheckTime(guild);
		}
	}
}

