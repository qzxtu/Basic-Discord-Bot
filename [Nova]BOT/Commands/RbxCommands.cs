using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Leaf.xNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace NovaBOT.Commands
{
    internal class RbxCommands : BaseCommandModule
    {
        #region secure webclient
        protected WebClient SecureWebClient()
        {
            WebClient w = new WebClient
            { Proxy = null }; return w;
        }
        #endregion

        #region rbxinfo
        private object endresult;
        [Command("rbxinfo")]
        [Description("Discord To Roblox")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task BotCreator(CommandContext ctx, string args)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            try
            {
                WebClient wc = SecureWebClient();
                embed.Color = DiscordColor.Black;
                _ = embed.WithTitle("Discord ID To Roblox");
                string thing = "https://verify.eryn.io/api/user/" + args;
                _ = embed.WithFooter($"{ctx.User.Username}" + " | " + DateTime.Now.ToString("MM/dd/yyyy") + " | ");
                endresult = JsonConvert.DeserializeObject(wc.DownloadString(thing));
                JObject ee = JObject.Parse(endresult.ToString());
                string username = ee["robloxUsername"].ToString();
                string followers = JObject.Parse(wc.DownloadString("https://friends.roblox.com/v1/users/" + ee["robloxId"].ToString() + "/followers/count"))["count"].ToString();
                string onlinestatus = JObject.Parse(wc.DownloadString("https://api.roblox.com/users/" + ee["robloxId"].ToString() + "/onlinestatus"))["IsOnline"].ToString();
                string friends = JObject.Parse(wc.DownloadString("https://friends.roblox.com/v1/users/" + ee["robloxId"].ToString() + "/friends/count"))["count"].ToString();
                if (onlinestatus == "False")
                {
                    onlinestatus = "not online";
                }
                else if (onlinestatus == "True")
                {
                    onlinestatus = "is online";
                }
                _ = embed.WithThumbnailUrl("http://www.roblox.com/Thumbs/Avatar.ashx?x=150&y=150&Format=Png&username=" + username);
                _ = embed.WithDescription(
                "**Username: **" + string.Format("{0:n0}", username) + Environment.NewLine +
                "**Online status: **" + string.Format("{0:n0}", onlinestatus) + Environment.NewLine +
                "**Followers: **" + string.Format("{0:n0}", followers) + Environment.NewLine +
                "**Friends: **" + string.Format("{0:n0}", friends)
                );
                if (onlinestatus == "True")
                {
                    _ = embed.WithDescription("status: " + JObject.Parse(wc.DownloadString("https://api.roblox.com/users/" + ee["robloxId"].ToString() + "/onlinestatus"))["LastLocation"].ToString() + "\n");
                }
                _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                _ = await ctx.Channel.SendMessageAsync("Failed To Find User!").ConfigureAwait(false);
            }
        }
        #endregion

        #region loadstring
        [Command("loadstring")]
        [Description("loadstring")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task Loadstring(CommandContext ctx, string lua, string args)
        {
            try
            {
                int x = int.Parse(args);
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                _ = embed.WithTitle("Here is your custom loadstring:");
                _ = embed.WithDescription($"`loadstring(game:GetObjects('rbxassetid://" + x + "')[1].Source)()`");
                _ = embed.WithColor(DiscordColor.Black);
                _ = embed.WithThumbnailUrl("https://cdn.discordapp.com/attachments/689717323543609386/757668292172185660/Lua-Logo.png");
                _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
            }
            catch
            {
                _ = await ctx.Channel.SendMessageAsync("Input your Roblox Catalog ID of your LocalScript").ConfigureAwait(false);
            }
        }
        #endregion

        #region bypass rbx name
        [Command("rbxbypassednames")]
        [Description("generate roblox bypassed names")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task websitesource(CommandContext ctx, int args)
        {
            _ = Convert.ToInt32(args);
            if (args >= 11)
            {
                _ = await ctx.Channel.SendMessageAsync("La cantidad maxima es de 10");
            }
            else
            {
                try
                {
                    int a = 0;
                    while (a != args)
                    {
                        List<string> nouns = new List<string> { "Nljrgeir", "Kumbucket", "Fayygot", "Arse", "CIi_t", "HltI1_er", "Retar_1d", "Iesbi4en", "C4ock" };
                        int index = new Random().Next(nouns.Count);
                        string rnoun = nouns[index];
                        nouns.RemoveAt(index);
                        List<string> adjectives = new List<string> { "Sxxexy", "Throbblng", "Stanky", "Wet", "Quaking", "Mois1t", "Juiclng", "Sweaty", "Ejacu1latlng" };
                        int index2 = new Random().Next(adjectives.Count);
                        string radjective = adjectives[index2];
                        adjectives.RemoveAt(index2);
                        int n = new Random().Next(11, 99);
                        DiscordMessage Message = await ctx.Channel.SendMessageAsync("Please wait...").ConfigureAwait(false);
                        _ = await Message.ModifyAsync("**Your bypassed names:** \n" + radjective + rnoun + n).ConfigureAwait(false);
                        a++;
                    }
                }
                catch
                {
                    _ = await ctx.Channel.SendMessageAsync($"An error occured...").ConfigureAwait(false);
                }
            }
        }
        #endregion

        #region rbxcheckusername
        [Command("rbxcheckusername")]
        [Description("Check Roblox Username")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task CHECKUSERNAME(CommandContext ctx, string args)
        {
            string url = "https://api.roblox.com//users/get-by-username?username=";
            try
            {
                HttpRequest req = new HttpRequest();
                string resp = req.Get(url + args).ToString();

                if (resp.Contains("User not found"))
                {
                    _ = await ctx.Channel.SendMessageAsync(args + "is invalid");
                }
                else
                {
                    string appendText = args;
                    _ = await ctx.Channel.SendMessageAsync("ValidUserName:" + appendText).ConfigureAwait(false);
                }
            }
            catch (Exception resp)
            {
                if (resp.Message.Contains("User not found"))
                {
                    _ = await ctx.Channel.SendMessageAsync(args + "IS INVALID").ConfigureAwait(false);
                }
            }
        }
        #endregion
    }
}