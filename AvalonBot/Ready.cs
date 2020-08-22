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
		[Command("ready")]
		[Alias("add")]
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
		[Alias("rm")]
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
		[Command("list")]
		[Alias("ls")]
		public async Task ListReadyAsync()
		{
			EmbedBuilder readyBuilder = new EmbedBuilder();
			string message = GetReadyMsg(readylist);
			readyBuilder.WithDescription(message)
						.WithTitle("List of Everyone Ready");
			await ReplyAsync("", false, readyBuilder.Build());
		}
		[Command("shuffle")]
		[Alias("shuf")]
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
		[Command("clear"), RequireUserPermission(ChannelPermission.ManageMessages)]
		[Alias("clr")]
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
		private static int days;
		private static int hour;
		private static int min;

		static ReadyAnnounce()
		{
			if(File.Exists("datetimes.txt"))
            {
				StreamReader file = new StreamReader("datetimes.txt");
				days = int.Parse(file.ReadLine());
				hour = int.Parse(file.ReadLine());
				min = int.Parse(file.ReadLine());
            }
		}

		private static SocketGuild guild;
		internal static void CheckTime(SocketGuild sguild)
		{
			guild = sguild;
			if (days == 0)
            {
				Console.WriteLine("No timer started");
				return;
            }
			//Time when method needs to be called
			var Today = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, min, 0);
			DateTime next, NextTime=Today.AddDays(7);
			for(int i = 0; i<7; i++)
            {
				//bitwise flags for if timer should be called on that day of the week
				if ((days & (2 << i)) == 0) continue;
				next = Today.AddDays((i - (int)Today.DayOfWeek + 7) % 7);
				if(next<DateTime.Now)
                {
					next = next.AddDays(7);
                }
				if(next<NextTime)
                {
					NextTime = next;
                }
            }
			TimeSpan ts = NextTime - DateTime.Now;
			//waits until next time to run
			Timer ReadyTimer = new Timer(ts.TotalMilliseconds)
			{
				AutoReset = false
            };
			ReadyTimer.Elapsed += ReadyTimer_Elapsed;
            ReadyTimer.Start();
			Console.WriteLine($"Timer started for {(DateTime.Now + ts).DayOfWeek}, {DateTime.Now + ts}");
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

