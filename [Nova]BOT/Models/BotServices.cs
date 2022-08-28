using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace NovaBOT.Models
{
    public class BotServices
    {
        public static async Task SendEmbedAsync(CommandContext ctx, string message, EmbedType type = EmbedType.Default)
        {
            string prefix = string.Empty;
            DiscordColor color;
            switch (type)
            {
                case EmbedType.Good:
                    color = DiscordColor.Green;
                    break;

                case EmbedType.Warning:
                    prefix = ":warning: ";
                    color = DiscordColor.Yellow;
                    break;

                case EmbedType.Missing:
                    prefix = ":mag: ";
                    color = DiscordColor.Wheat;
                    break;

                case EmbedType.Error:
                    prefix = ":no_entry: ";
                    color = DiscordColor.Red;
                    break;

                default:
                    color = new DiscordColor("#00FF7F");
                    break;
            }

            DiscordEmbedBuilder output = new DiscordEmbedBuilder()
                .WithDescription(prefix + message)
                .WithColor(color);
            _ = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        public static async Task SendUserStateChangeAsync(CommandContext ctx, UserStateChange state, DiscordMember user, string reason)
        {
            DiscordEmbedBuilder output = new DiscordEmbedBuilder()
                .WithDescription($"{state}: {user.DisplayName}#{user.Discriminator}\nIdentifier: {user.Id}\nReason: {reason}\nIssued by: {ctx.Member.DisplayName}#{ctx.Member.Discriminator}")
                .WithColor(DiscordColor.Green);
            _ = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }

        public static bool CheckUserInput(string input)
        {
            return !string.IsNullOrWhiteSpace(input);
        }

        public static bool CheckChannelName(string input)
        {
            return !string.IsNullOrWhiteSpace(input) && input.Length <= 100;
        }

        public static async Task<InteractivityResult<DiscordMessage>> GetUserInteractivity(CommandContext ctx, string keyword, int seconds)
        {
            return await ctx.Client.GetInteractivity()
                .WaitForMessageAsync(
                    m => m.Channel.Id == ctx.Channel.Id && string.Equals(m.Content, keyword, StringComparison.InvariantCultureIgnoreCase),
                    TimeSpan.FromSeconds(seconds)).ConfigureAwait(false);
        }

        public static int LimitToRange(int value, int min = 1, int max = 100)
        {
            return value <= min ? min : value >= max ? max : value;
        }

        public static async Task RemoveMessage(DiscordMessage message)
        {
            await message.DeleteAsync().ConfigureAwait(false);
        }

        public static async Task<MemoryStream> CheckImageInput(CommandContext ctx, string input)
        {
            MemoryStream stream = new MemoryStream();
            if (!Uri.TryCreate(input, UriKind.Absolute, out _) &&
                (!input.EndsWith(".img") || !input.EndsWith(".png") || !input.EndsWith(".jpg")))
            {
                await SendEmbedAsync(ctx, "An image URL ending with .img, .png or .jpg is required!", EmbedType.Warning)
                    .ConfigureAwait(false);
            }
            else
            {
                WebClient client = new WebClient();
                byte[] results = client.DownloadData(input);
                stream.Write(results, 0, results.Length);
                stream.Position = 0;
            }

            return stream;
        }
    }
}
