using NovaBOT.Models;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace NovaBOT.Commands
{
    [Hidden]
    internal class ModerationCommands : BaseCommandModule
    {
        #region MiniMod
        [Command("addminimod")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task addMini(CommandContext ctx, DiscordMember member)
        {
            DiscordRole role = ctx.Guild.GetRole(your guild role id);
            await member.GrantRoleAsync(role, "Promotion").ConfigureAwait(false);
            _ = await ctx.Channel.SendMessageAsync(member.Mention + " has been promoted to Mini-Moderator!").ConfigureAwait(false);
        }

        [Command("removeminimod")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task removeMini(CommandContext ctx, DiscordMember member)
        {
            DiscordRole role = ctx.Guild.GetRole(your guild role id);
            await member.RevokeRoleAsync(role, "Demotion.").ConfigureAwait(false);
            _ = await ctx.Channel.SendMessageAsync(member.Mention + " has been demoted from Mini-Moderator.").ConfigureAwait(false);
        }

        #endregion

        #region Mod
        [Command("addMod")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task addMod(CommandContext ctx, DiscordMember member)
        {
            DiscordRole role = ctx.Guild.GetRole(your guild role id);
            await member.GrantRoleAsync(role, "Promotion").ConfigureAwait(false);
            _ = await ctx.Channel.SendMessageAsync(member.Mention + " has been promoted to Moderator!").ConfigureAwait(false);
        }

        [Command("removeMod")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task removeMod(CommandContext ctx, DiscordMember member)
        {
            DiscordRole role = ctx.Guild.GetRole(your guild role id);
            await member.RevokeRoleAsync(role, "Demotion.").ConfigureAwait(false);
            _ = await ctx.Channel.SendMessageAsync(member.Mention + " has been demoted from Moderator.").ConfigureAwait(false);
        }

        #endregion

        #region Admin

        [Command("addadmin")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task addAdmin(CommandContext ctx, DiscordMember member)
        {
            _ = await ctx.Channel.SendMessageAsync(member.Mention + " has been promoted to Admin!").ConfigureAwait(false);
        }

        [Command("removeadmin")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task removeAdmin(CommandContext ctx, DiscordMember member)
        {
            _ = await ctx.Channel.SendMessageAsync(member.Mention + " has been demoted from Admin.").ConfigureAwait(false);
        }

        #endregion

        #region Hunter

        [Command("addHunter")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task addSupport(CommandContext ctx, DiscordMember member)
        {
            DiscordRole role = ctx.Guild.GetRole(your guild role id);
            await member.GrantRoleAsync(role, "Promotion").ConfigureAwait(false);
            _ = await ctx.Channel.SendMessageAsync(member.Mention + " just been givin the Noticed Hunter role.").ConfigureAwait(false);
        }

        [Command("removeHunter")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task removeSupport(CommandContext ctx, DiscordMember member)
        {
            DiscordRole role = ctx.Guild.GetRole(your guild role id);
            await member.RevokeRoleAsync(role, "Demotion.").ConfigureAwait(false);
        }

        #endregion

        #region Noticed

        [Command("notice")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task addNoticed(CommandContext ctx, DiscordMember member)
        {
            DiscordRole role = ctx.Guild.GetRole(your guild role id);
            await member.GrantRoleAsync(role, "Promotion").ConfigureAwait(false);
            _ = await ctx.Channel.SendMessageAsync(member.Mention + " is now noticed by staff!").ConfigureAwait(false);
        }

        [Command("removenotice")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task removeNoticed(CommandContext ctx, DiscordMember member)
        {
            DiscordRole role = ctx.Guild.GetRole(your guild role id);
            await member.RevokeRoleAsync(role, "Demotion.").ConfigureAwait(false);
        }

        #endregion

        #region ResetRoles
        [Command("resetRoles")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task demoteFully(CommandContext ctx, DiscordMember member)
        {
            DiscordRole noticed = ctx.Guild.GetRole(your guild role id);
            DiscordRole supporter = ctx.Guild.GetRole(your guild role id);
            DiscordRole admin = ctx.Guild.GetRole(your guild role id);
            DiscordRole mod = ctx.Guild.GetRole(your guild role id);
            DiscordRole mini = ctx.Guild.GetRole(your guild role id);

            await member.RevokeRoleAsync(mini, "Demotion.").ConfigureAwait(false);
            await member.RevokeRoleAsync(mod, "Demotion.").ConfigureAwait(false);
            await member.RevokeRoleAsync(admin, "Demotion.").ConfigureAwait(false);
            await member.RevokeRoleAsync(supporter, "Demotion.").ConfigureAwait(false);
            await member.RevokeRoleAsync(noticed, "Demotion.").ConfigureAwait(false);


            _ = await ctx.Channel.SendMessageAsync(member.Mention + " has been reset.").ConfigureAwait(false);
        }

        #endregion

        #region Warn
        [Command("warn")]
        [Aliases("scold")]
        [Description("Direct message user with a warning")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task Warn(CommandContext ctx,
             [Description("Server user to warn")] DiscordMember member,
             [Description("Warning message")][RemainingText] string reason = null)
        {
            DiscordEmbedBuilder output = new DiscordEmbedBuilder()
                .WithTitle("Warning received!")
                .WithDescription(Formatter.Bold(ctx.Guild.Name) + " has issued you a server warning!")
                .AddField("Sender:", ctx.Member.Username + "#" + ctx.Member.Discriminator, true)
                .AddField("Server Owner:", ctx.Guild.Owner.Username + "#" + ctx.Guild.Owner.Discriminator, true)
                .WithThumbnailUrl(ctx.Guild.IconUrl)
                .WithTimestamp(DateTime.Now)
                .WithColor(DiscordColor.Red);
            if (!string.IsNullOrWhiteSpace(reason))
            {
                _ = output.AddField("Warning message:", reason);
            }

            DiscordDmChannel dm = await member.CreateDmChannelAsync().ConfigureAwait(false);
            if (dm is null)
            {
                await BotServices.SendEmbedAsync(ctx, "Unable to direct message this user", EmbedType.Warning)
                    .ConfigureAwait(false);
            }
            else
            {
                _ = await dm.SendMessageAsync(embed: output.Build()).ConfigureAwait(false);
                await BotServices.SendEmbedAsync(ctx, "Successfully sent a warning to " + Formatter.Bold(member.Username), EmbedType.Good)
                    .ConfigureAwait(false);
            }
        }
        #endregion

        #region deafen/undeafen

        [Command("deafen")]
        [Aliases("deaf")]
        [Description("Deafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Deafen(CommandContext ctx,
            [Description("Server user to deafen")] DiscordMember member,
            [Description("Reason for the deafen")][RemainingText] string reason = null)
        {
            if (member.IsDeafened)
            {
                _ = await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **deafened**.")
                    .ConfigureAwait(false);
            }
            else
            {
                await member.SetDeafAsync(true, reason).ConfigureAwait(false);
                await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
                await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Deafen, member, reason ?? "No reason provided.");
            }
        }

        #region prune

        [Command("prune")]
        [Description("Prune inactive server members")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task PruneUsers(CommandContext ctx,
            [Description("Number of days the user had to be inactive to get pruned")] int days = 7)
        {
            if (days < 1 || days > 30)
            {
                await BotServices.SendEmbedAsync(ctx, "Number of days must be between 1 and 30", EmbedType.Warning)
                    .ConfigureAwait(false);
            }

            int count = await ctx.Guild.GetPruneCountAsync(days).ConfigureAwait(false);
            if (count == 0)
            {
                _ = await ctx.RespondAsync("No inactive members found to prune").ConfigureAwait(false);
                return;
            }

            DiscordMessage prompt = await ctx
                .RespondAsync($"Pruning will remove {Formatter.Bold(count.ToString())} member(s).\nRespond with **yes** to continue.")
                .ConfigureAwait(false);
            DSharpPlus.Interactivity.InteractivityResult<DiscordMessage> interactivity = await BotServices.GetUserInteractivity(ctx, "yes", 10).ConfigureAwait(false);
            if (interactivity.Result is null)
            {
                return;
            }

            await BotServices.RemoveMessage(interactivity.Result).ConfigureAwait(false);
            await BotServices.RemoveMessage(prompt).ConfigureAwait(false);
            _ = await ctx.Guild.PruneAsync(days).ConfigureAwait(false);
        }

        #endregion

        [Command("undeafen")]
        [Aliases("undeaf")]
        [Description("Undeafen server user")]
        [RequirePermissions(Permissions.DeafenMembers)]
        public async Task Undeafen(CommandContext ctx,
          [Description("Server user to undeafen")][RemainingText] DiscordMember member,
          [Description("Reason for the deafen")][RemainingText] string reason = null)
        {
            await member.SetDeafAsync(false, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Undeafen, member, reason ?? "No reason provided");
        }

        #endregion

        #region Mute/UnMute

        [Command("mute")]
        [Aliases("silence")]
        [Description("Mute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Mute(CommandContext ctx,
             [Description("Server user to mute")] DiscordMember member,
             [Description("Reason for the mute")][RemainingText] string reason = null)
        {
            if (member.IsMuted)
            {
                _ = await ctx.RespondAsync($"{member.DisplayName}#{member.Discriminator} is already **muted**.").ConfigureAwait(false);
            }
            else
            {
                await member.SetMuteAsync(true, reason).ConfigureAwait(false);
                await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
                await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Mute, member, reason ?? "No reason provided.");
            }
        }

        [Command("unmute")]
        [Description("Unmute server user")]
        [RequirePermissions(Permissions.MuteMembers)]
        public async Task Unmute(CommandContext ctx, [Description("Server user to unmute")][RemainingText] DiscordMember member, [Description("Reason for the deafen")][RemainingText] string reason = null)
        {
            await member.SetMuteAsync(false, reason).ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Unmute, member, reason ?? "No reason provided");
        }

        #endregion

        #region Ban
        [Command("ban")]
        [Description("Ban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, [Description("Server user to ban")] DiscordMember member, [Description("Reason for the ban")][RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
            {
                _ = await ctx.RespondAsync("You cannot ban yourself.").ConfigureAwait(false);
            }
            else
            {
                await ctx.Guild.BanMemberAsync(member, 7, reason).ConfigureAwait(false);
                await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
                await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Ban, member, reason ?? "No reason provided.");
            }
        }

        [Command("unban")]
        [Description("Unban server user")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Remove(CommandContext ctx,
           [Description("Discord user ID to unban from the server")] ulong userId,
           [Description("Reason for the deafen")][RemainingText] string reason = null)
        {
            DiscordUser member = await ctx.Client.GetUserAsync(userId).ConfigureAwait(false);
            await ctx.Guild.UnbanMemberAsync(member, reason ?? "No reason provided.").ConfigureAwait(false);
            await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
            _ = await ctx.RespondAsync($"Unbanned Discord User #{member} from the server.").ConfigureAwait(false);
        }
        #endregion

        #region Kick
        [Command("kick")]
        [Aliases("remove")]
        [Description("Kick server user")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx,
        [Description("Server user to kick")] DiscordMember member,
        [Description("Reason for the kick")][RemainingText] string reason = null)
        {
            if (ctx.Member.Id == member.Id)
            {
                _ = await ctx.RespondAsync("You cannot kick yourself.").ConfigureAwait(false);
            }
            else
            {
                await member.RemoveAsync(reason).ConfigureAwait(false);
                await BotServices.RemoveMessage(ctx.Message).ConfigureAwait(false);
                await BotServices.SendUserStateChangeAsync(ctx, UserStateChange.Kick, member, reason ?? "No reason provided.");
            }
        }
        #endregion
    }
}
