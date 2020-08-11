using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.Addons.Interactive;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace AvalonBot
{
    public class Admin : InteractiveBase<SocketCommandContext>
    {
        [Command("test")]
        public async Task Test()
        {
            EmbedBuilder test = new EmbedBuilder();
            test.WithDescription("Hello World");
            await ReplyAsync("", false, test.Build());
        }
        [Command("react")]
        public async Task React(ulong msgid, string emotestring)
        {
            Console.WriteLine(emotestring);
            IMessage msg = Context.Channel.GetMessageAsync(msgid).Result;
            if(msg != null)
            {
                if (Emote.TryParse(emotestring, out Emote emote))
                {
                    await msg.AddReactionAsync(emote);
                    await ReplyAndDeleteAsync("Done!");
                }
                else
                {
                    try
                    {
                        await msg.AddReactionAsync(new Emoji(emotestring));
                        await ReplyAndDeleteAsync("Done!");
                    }
                    catch (Exception)
                    {
                        await ReplyAndDeleteAsync("Failed to get emoji");
                    }
                }
            }
            else
            {
                await ReplyAndDeleteAsync("Failed to get message.");
            }
        }
    }
}
