using NovaBOT.Models;
using DevComponents.DotNetBar;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Microsoft.VisualBasic.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Web.Security;
using System.Windows.Controls;
using Microsoft.VisualBasic.ApplicationServices;

namespace NovaBOT.Commands
{
    public class MainCommands : BaseCommandModule
    {
        private static int adapternum;

        #region secure webclient
        protected WebClient SecureWebClient()
        {
            WebClient w = new WebClient
            { Proxy = null }; return w;
        }
        #endregion

        #region Ping
        [Command("ping")]
        [Description("Returns Pong and Latency.")]
        public async Task PingPong(CommandContext ctx)
        {
            Stopwatch time = new Stopwatch();
            time.Start();
            await Task.Delay(1);
            time.Stop();
            DiscordEmbedBuilder pingImbed = new DiscordEmbedBuilder
            {
                Description = "Pong! |" + ":stopwatch:" + time.ElapsedMilliseconds.ToString() + "ms" + " | " + ":heartbeat:" + ctx.Client.Ping + "ms | " + $"{ctx.User.Mention}",
                Color = new DiscordColor(3, 252, 94)
            };
            const int delay = 100;
            await Task.Delay(delay);
            _ = await ctx.Channel.SendMessageAsync(embed: pingImbed).ConfigureAwait(false);
        }
        #endregion

        #region F
        [Command("respects"), Aliases("F")]
        [RequireBotPermissions(Permissions.AddReactions)]
        public async Task Respects(CommandContext ctx, [RemainingText] string args)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                ImageUrl = "https://media.discordapp.net/attachments/689717323543609386/755980623826387014/1600310218312.png?width=180&height=180",
                Description = $"Press F to pay respects to " + args + ":",
                Color = new DiscordColor(37, 159, 207)
            };
            DiscordMessage Message = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
            DiscordEmoji thumbsUpEmoji = DiscordEmoji.FromName(ctx.Client, ":regional_indicator_f:");
            await Message.CreateReactionAsync(thumbsUpEmoji).ConfigureAwait(false);
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            InteractivityResult<DSharpPlus.EventArgs.MessageReactionAddEventArgs> reactionResult = await interactivity.WaitForReactionAsync(
                x => x.Message == Message &&
                x.User == ctx.User &&
               (x.Emoji == thumbsUpEmoji)).ConfigureAwait(false);
        }
        #endregion

        #region random SCP
        [Command("randomscp")]
        [Description("SCP Foundation Secure, Contain, Protect")]
        public async Task randomscp(CommandContext ctx)
        {
            int scp;
            string scpr;
            Random r = new Random();
            scp = r.Next(1, 5999);
            scpr = scp <= 99 ? scp <= 9 ? 0 + 0 + scp.ToString() : 0 + scp.ToString() : scp.ToString();
            _ = await ctx.Channel.SendMessageAsync("http://www.scpwiki.com/scp-" + scpr);
        }
        #endregion

        #region geoip
        [Command("geoip")]
        public async Task ip(CommandContext ctx, [RemainingText] string args)
        {
            try
            {
                WebClient wc = SecureWebClient();
                {
                    DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                    string text = wc.DownloadString("https://api.apithis.net/geoip.php?ip=" + args);
                    _ = embed.WithDescription(text.Replace("<br />", Environment.NewLine));
                    _ = embed.WithTitle("GeoIP");
                    _ = embed.WithColor(DiscordColor.Black);
                    _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                }
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region Translator
        public string TranslateText(string input, string inputlanguage, string outputlanguage)
        {
            string url = string.Format("https://translate.googleapis.com/translate_a/single?client=gtx&sl={0}&tl={1}&dt=t&q={2}", inputlanguage, outputlanguage, Uri.EscapeUriString(input));
            HttpClient httpClient = new HttpClient();
            string result = httpClient.GetStringAsync(url).Result;
            List<dynamic> jsonData = new JavaScriptSerializer().Deserialize<List<dynamic>>(result);
            dynamic translationItems = jsonData[0];
            string translation = "";
            foreach (object item in translationItems)
            {
                IEnumerable translationLineObject = item as IEnumerable;
                IEnumerator translationLineString = translationLineObject.GetEnumerator();
                _ = translationLineString.MoveNext();
                translation += string.Format(" {0}", Convert.ToString(translationLineString.Current));
            }
            if (translation.Length > 1) { translation = translation.Substring(1); };
            return translation;
        }

        [Command("translate")]
        public async Task translator(CommandContext ctx, string translate, [RemainingText] string args)
        {
            if (translate.Contains("en"))
            {
                string translatedText = TranslateText(args, "en", "es");
                _ = await ctx.Channel.SendMessageAsync(translatedText).ConfigureAwait(false);
            }
            else if (translate.Contains("es"))
            {
                string translatedText = TranslateText(args, "es", "en");
                _ = await ctx.Channel.SendMessageAsync(translatedText).ConfigureAwait(false);
            }
        }
        #endregion

        #region role info
        [Command("roleinfo")]
        [Description("Retrieve role information")]
        public async Task GetRole(CommandContext ctx,
            [Description("Server role information to retrieve")][RemainingText] DiscordRole role = null)
        {
            if (role is null)
            {
                _ = ctx.Channel.SendMessageAsync("ERROR: Data unavailable").ConfigureAwait(false);
            }
            else
            {
                DiscordEmbedBuilder output = new DiscordEmbedBuilder()
                    .WithTitle(role.Name)
                    .WithDescription("ID: " + role.Id)
                    .AddField("Creation Date", role.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                    .AddField("Hoisted", role.IsHoisted ? "Yes" : "No", true)
                    .AddField("Mentionable", role.IsMentionable ? "Yes" : "No", true)
                    .AddField("Permissions", role.Permissions.ToPermissionString())
                    .WithThumbnailUrl(ctx.Guild.IconUrl)
                    .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                    .WithColor(role.Color);
                _ = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
        }
        #endregion

        #region generateqr
        [Command("generateqr")]
        [Description("generate QR code")]
        public async Task generateqr(CommandContext ctx, [RemainingText] string args)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            _ = embed.WithDescription($"**[QR Code Generator](https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={args})**");
            _ = embed.WithImageUrl($"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={args}");
            _ = embed.WithColor(new DiscordColor(139, 0, 0));
            _ = await ctx.Channel.SendMessageAsync("", false, embed).ConfigureAwait(false);
        }
        #endregion

        #region LinkExtractor
        [Command("linkextractor")]
        public async Task linkEX(CommandContext ctx, string args)
        {
            string path = "./linkextractor/linkextractor.txt";
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.hackertarget.com/pagelinks/?q=" + args);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    using (Stream stream = response.GetResponseStream())
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        args = reader.ReadToEnd();
                    }
                }
                File.WriteAllText(path, args);
                _ = await ctx.Channel.SendFileAsync(path).ConfigureAwait(false);
            }
            catch
            {
                return;
            }
        }
        #endregion

        #region w95key
        private static readonly Random rd = new Random(); internal static string CreateInterger(int intLength)
        {
            const string allowedChars = "1234567890"; char[] chars = new char[intLength]; for (int i = 0; i < intLength; i++)
            { chars[i] = allowedChars[rd.Next(0, allowedChars.Length)]; }
            return new string(chars);
        }

        private int[] GetIntArray(int num)
        {
            List<int> listOfInts = new List<int>(); while (num > 0)
            { listOfInts.Add(num % 10); num /= 10; }
            listOfInts.Reverse(); return listOfInts.ToArray();
        }

        private readonly string a = "95";
        private readonly string b2 = "96";
        private readonly string c = "97";
        private readonly string d2 = "98";
        private readonly string el = "99";
        private readonly string f = "01";
        private readonly string g = "02";
        private readonly string h = "03"; [Command("w95key")]
        public async Task w95key(CommandContext ctx)
        {
            bool running = true; DiscordMessage Message = await ctx.Channel.SendMessageAsync("Running Key Generation Loop...").ConfigureAwait(false); while (running == true)
            {
                int l2 = Convert.ToInt32(CreateInterger(5)); int imdoingepicgamerint = new Random().Next(001, 366); string rand = rd.Next(1, 8).ToString(); if (rand == "1")
                { rand = a; }
                else if (rand == "2")
                { rand = b2; }
                else if (rand == "3")
                { rand = c; }
                else if (rand == "4")
                { rand = d2; }
                else if (rand == "5")
                { rand = el; }
                else if (rand == "6")
                { rand = f; }
                else if (rand == "7")
                { rand = g; }
                else if (rand == "8")
                { rand = h; }
                string d = "-OEM-0"; int l = rd.Next(1, 1000000000); l = Convert.ToInt32(l.ToString().Remove(6, l.ToString().Length - 6)); string b = "0" + l.ToString(); int sum = GetIntArray(l).Aggregate((temp, x) => temp + x); if (sum % 7 == 0)
                {
                    if (!l.ToString().EndsWith("0"))
                    {
                        if (!l.ToString().EndsWith("8"))
                        {
                            if (!l.ToString().EndsWith("9"))
                            {
                                bool flag = Convert.ToInt32(rand) % 4 == 0; if (!flag && !Convert.ToBoolean(imdoingepicgamerint == 366))
                                {
                                    string flag2 = imdoingepicgamerint + rand; if (flag2.Length == 5)
                                    { _ = await Message.ModifyAsync(imdoingepicgamerint + rand + d + l + "-" + l2).ConfigureAwait(false); running = false; }
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region link
        [Command("linkcut")]
        private async Task link(CommandContext ctx, string message)
        {
            WebClient wc = SecureWebClient();
            string output = wc.DownloadString("https://shrinkearn.com/api?api=f89d1750bdad3cdc0215a2aaa1ec6dcf534972b0&url=" + message.ToString() + "&format=text");
            IReadOnlyList<DiscordMessage> items = await ctx.Channel.GetMessagesAsync(1).ConfigureAwait(false);
            await ctx.Channel.DeleteMessagesAsync(items);
            _ = await ctx.Channel.SendMessageAsync(output).ConfigureAwait(false);
        }
        #endregion

        #region GetHTML
        public static string GetHTML(string Url)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(Url);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
            string HTML = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();
            return HTML;
        }
        #endregion

        #region Instagram check
        [Command("Instagramcheck")]
        private async Task Instagramcheck(CommandContext ctx, [RemainingText] string args)
        {
            try
            {
                _ = GetHTML("https://instagram.com/" + args).Contains(args);
                _ = await ctx.Channel.SendMessageAsync("The name " + args + " is already in use").ConfigureAwait(false);
            }
            catch
            {
                _ = await ctx.Channel.SendMessageAsync("The name " + args + " is available").ConfigureAwait(false);
            }
        }
        #endregion

        #region Twitter check
        [Command("Twittercheck")]
        private async Task Twittercheck(CommandContext ctx, [RemainingText] string args)
        {
            try
            {
                _ = GetHTML("https://twitter.com/" + args).Contains("Sorry, that page doesn’t exist!");
                _ = await ctx.Channel.SendMessageAsync("The name " + args + " is already in use").ConfigureAwait(false);
            }
            catch
            {
                _ = await ctx.Channel.SendMessageAsync("The name " + args + " is available").ConfigureAwait(false);
            }
        }
        #endregion

        #region linkvertise
        [Command("linkvertisebypass")]
        [RequirePermissions(Permissions.Administrator)]
        private async Task linkvertise(CommandContext ctx, string getLink = null)
        {
            StringBuilder sb = new StringBuilder();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(3, 252, 94)
            };
            _ = embed.WithFooter($"{ctx.User.Username}" + " | " + DateTime.Now.ToString("MM/dd/yyyy") + " | ");
            _ = sb.AppendLine();
            string uriString = "https://online-coding.eu/api/LinkvertiseBypass.php?url=" + getLink;
            WebClient wc = SecureWebClient();
            _ = getLink == null
                ? sb.AppendLine("**" + "Necesitas poner un enlace!" + "**")
                : sb.AppendLine("**" + "Your bypassed linkvertise link: " + "**" + wc.DownloadString(new Uri(uriString))); ;
            wc.Dispose();
            embed.Description = sb.ToString();
            _ = await ctx.Channel.SendMessageAsync(null, false, embed.Build()).ConfigureAwait(false);
        }
        #endregion

        #region say
        [Command("say")]
        private async Task say(CommandContext ctx, [RemainingText] string @args = null)
        {
            StringBuilder sb = new StringBuilder();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder
            {
                Color = new DiscordColor(3, 252, 94)
            };
            _ = embed.WithFooter($"{ctx.User.Username}" + " | " + DateTime.Now.ToString("MM/dd/yyyy") + " | ");
            _ = sb.AppendLine();
            _ = args == null ? sb.AppendLine("**" + "Necesitas poner un texto!" + "**") : sb.AppendLine($"**" + @args.ToString() + "**");

            {
            };
            embed.Description = sb.ToString();
            _ = await ctx.Channel.SendMessageAsync(null, false, embed.Build()).ConfigureAwait(false);
        }
        #endregion

        #region emoji
        [Command("emoji")]
        [Aliases("emojiinfo")]
        [Description("Retrieve server emoji information.")]
        public async Task GetEmoji(CommandContext ctx,
            [Description("Server emoji information to retrieve.")] DiscordEmoji query)
        {
            try
            {
                DiscordGuildEmoji emoji = await ctx.Guild.GetEmojiAsync(query.Id).ConfigureAwait(false);
                DiscordEmbedBuilder output = new DiscordEmbedBuilder()
                    .WithTitle(emoji.Name)
                    .WithDescription("Created By " + (emoji.User is null ? "<unknown>" : emoji.User.Username))
                    .AddField("Server", emoji.Guild.Name, true)
                    .AddField("Creation Date", emoji.CreationTimestamp.ToString(), true)
                    .WithColor(DiscordColor.Black)
                    .WithUrl(emoji.Url)
                    .WithThumbnailUrl(emoji.Url);
                _ = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
            catch { return; }
        }
        #endregion 

        #region emojilist
        [Command("emojilist")]
        [Aliases("print", "l", "ls", "all")]
        [Description("Retrieve list of server emojis.")]
        public async Task GetEmojiList(CommandContext ctx)
        {
            StringBuilder emojiList = new StringBuilder();
            foreach (DiscordEmoji emoji in ctx.Guild.Emojis.Values.OrderBy(e => e.Name))
            {
                _ = emojiList.Append(emoji.Name).Append(!emoji.Equals(ctx.Guild.Emojis.Last().Value) ? ", " : string.Empty);
            }

            DiscordEmbedBuilder output = new DiscordEmbedBuilder()
                .WithTitle("Emojis available for " + ctx.Guild.Name)
                .WithDescription(emojiList.ToString())
                .WithThumbnailUrl(ctx.Guild.IconUrl)
                .WithColor(DiscordColor.Black);
            _ = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }
        #endregion

        #region whois
        [Command("whois")]
        [Description("Get information about someone.")]
        public async Task GetUser(CommandContext ctx, [RemainingText] DiscordMember member)
        {
            member = member ?? ctx.Member;
            StringBuilder roles = new StringBuilder();
            Permissions permsobj = member.PermissionsIn(ctx.Channel);
            string perms = permsobj.ToPermissionString();
            DiscordEmbedBuilder output = new DiscordEmbedBuilder()
                .WithTitle($"@{member.Username}#{member.Discriminator}")
                .WithDescription("ID: " + member.Id)
                .AddField("Registered on", member.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Joined on", member.JoinedAt.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Nickname", member.Nickname ?? "None", true)
                .AddField("Muted?", member.IsMuted ? "Yes" : "No", true)
                .AddField("Deafened?", member.IsDeafened ? "Yes" : "No", true)
                .WithThumbnailUrl(member.AvatarUrl)
                .WithFooter($"{ctx.Guild.Name} / #{ctx.Channel.Name} / {DateTime.Now}")
                .WithColor(member.Color);
            if (member.IsBot)
            {
                output.Title += " __[BOT]__ ";
            }

            if (member.IsOwner)
            {
                output.Title += " __[OWNER]__ ";
            }

            _ = output.AddField("Verified?", member.Verified == true ? "Yes" : "No", true);
            foreach (DiscordRole role in member.Roles)
            {
                _ = roles.Append($"[`{role.Name}`] ");
            }

            if (roles.Length > 0)
            {
                _ = output.AddField("Roles", roles.ToString(), true);
            }

            if (((permsobj & Permissions.Administrator) | (permsobj & Permissions.AccessChannels)) == 0)
            {
                perms = $"**This user cannot see this channel!**\n{perms}";
            }

            _ = output.AddField("Permissions", perms ?? "*None*");
            _ = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }
        #endregion 

        #region nickname
        [Command("nick")]
        [Description("Change a user's nickname")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task NicknameSet(CommandContext ctx, [Description("Nickname to change")] DiscordMember user, [Description("New name")][RemainingText] string nickname)
        {
            DiscordMember discordMember = user;
            string oldNick = discordMember.Nickname;
            await ctx.TriggerTypingAsync();
            try
            {
                await discordMember.ModifyAsync(x => x.Nickname = nickname);
                _ = await ctx.Channel.SendMessageAsync($"{ctx.Member.Username} changed {oldNick}'s nickname to: {discordMember.Nickname}.").ConfigureAwait(false);

            }
            catch (Exception e)
            {
                _ = await ctx.Channel.SendMessageAsync($"An error occured...").ConfigureAwait(false);
                Console.WriteLine($"{e}");
            }
        }
        #endregion

        #region squads
        [Command("Squads")]
        [Description("El maximo es de 7 personas")]
        public async Task CreateSquads(CommandContext ctx, [RemainingText] params string[] usersArray)
        {
            string[] squadOne;
            string[] squadTwo;
            int totalUsers = usersArray.Length;
            Random squadSelector = new Random();
            StringBuilder sb = new StringBuilder();
            string squadsMessage;

            if (totalUsers < 5)
            {
                squadsMessage = "I am sensing 4 or less names, just play squads, morons.";
            }
            else if (totalUsers >= 5 && totalUsers <= 7)
            {
                usersArray = usersArray.OrderBy(i => squadSelector.Next()).ToArray();
                squadOne = usersArray.Take(4).ToArray();
                squadTwo = usersArray.Skip(4).Take(totalUsers - 4).ToArray();
                _ = sb.Append("__**Squad One:**__");
                _ = sb.Append(Environment.NewLine);
                for (int i = 0; i < squadOne.Length; i++)
                {
                    _ = sb.Append(squadOne[i]);
                    _ = sb.Append(Environment.NewLine);
                }
                _ = sb.Append(Environment.NewLine);

                _ = totalUsers == 5 ? sb.Append("__**LOL, Loser:**__") : sb.Append("__**Squad Two:**__");

                _ = sb.Append(Environment.NewLine);

                for (int i = 0; i < squadTwo.Length; i++)
                {
                    _ = sb.Append(squadTwo[i]);
                    _ = sb.Append(Environment.NewLine);
                }

                squadsMessage = sb.ToString();
            }
            else
            {
                squadsMessage = "We have this many friends? Wut.";
            }
            _ = await ctx.Channel.SendMessageAsync(squadsMessage).ConfigureAwait(false);
        }
        #endregion

        #region fights
        [Command("fight")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task Queue(CommandContext ctx)
        {
            _ = ctx.Message.Author.Id;
            DiscordGuild server = ctx.Guild;
            string name = ctx.Member.Username;
            int? userLimit = 2;
            DiscordMember member = ctx.Member;

            DiscordChannel fightCategory = await server.CreateChannelAsync($"{name} :vs: ", ChannelType.Category);
            _ = await server.CreateChannelAsync("Bets and Feed", ChannelType.Text, fightCategory);
            DiscordChannel fightVoiceChannel = await server.CreateChannelAsync($"For", ChannelType.Voice, fightCategory, default, null, userLimit);
            {
                _ = await ctx.Channel.SendMessageAsync($"{member} has being intiated into the fight and the channel {name} :vs: has been made to do your betting").ConfigureAwait(false);
                _ = await ctx.Channel.SendMessageAsync($"{fightVoiceChannel} has been created and you will be placed shortly").ConfigureAwait(false);
                await member.PlaceInAsync(fightVoiceChannel);
                await fightVoiceChannel.PlaceMemberAsync(member);
            }
        }
        #endregion

        #region image
        [Command("image")]
        public async Task SendPic(CommandContext ctx)
        {
            string[] RandomPic = { "https://images.unsplash.com/photo-1536746803623-cef87080bfc8?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1000&q=80",
            "https://s3.amazonaws.com/artallnight/static/files/2018/08/14154730/back-1024x532.jpg",
            "https://www.abc.net.au/news/image/9776766-3x2-700x467.jpg",
            "https://static1.squarespace.com/static/58d2d1d003596ef5151dd694/t/5911277b893fc011d4e8543d/1494296445782/stars2.jpg?format=1500w"
            };
            Random random = new Random();
            int randomNumber = random.Next(0, 3);
            DiscordEmbedBuilder d = new DiscordEmbedBuilder();
            _ = d.WithColor(new DiscordColor(120, 40, 23));

            _ = randomNumber == 1
                ? await ctx.Channel.SendMessageAsync(RandomPic[1]).ConfigureAwait(false)
                : randomNumber == 2
                    ? await ctx.Channel.SendMessageAsync(RandomPic[2]).ConfigureAwait(false)
                    : randomNumber == 3
                                    ? await ctx.Channel.SendMessageAsync(RandomPic[3]).ConfigureAwait(false)
                                    : await ctx.Channel.SendMessageAsync(RandomPic[0]).ConfigureAwait(false);
        }
        #endregion

        #region news
        public class ItemNews
        {
            public string title { get; set; }
            public string link { get; set; }
            public string PubDate { get; set; }
        }

        [Command("news")]
        public async Task news(CommandContext ctx, [RemainingText] string args)
        {
            try
            {
                StringBuilder sb = new StringBuilder(); DiscordEmbedBuilder embed = new DiscordEmbedBuilder(); HttpWebRequest request = (HttpWebRequest)WebRequest.Create
                     ("http://news.google.com/news?q=" + args + "&output=rss"); request.Method = "GET"; HttpWebResponse response = (HttpWebResponse)request.GetResponse(); if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream(); StreamReader readStream = response.CharacterSet == ""
                        ? new StreamReader(receiveStream)
                        : new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    string data = readStream.ReadToEnd(); DataSet ds = new DataSet(); StringReader reader = new StringReader(data); _ = ds.ReadXml(reader); DataTable dtGetNews = new DataTable(); if (ds.Tables.Count > 3)
                    {
                        dtGetNews = ds.Tables["item"]; foreach (DataRow dtRow in dtGetNews.Rows)
                        { string title = dtRow["title"].ToString(); string link = dtRow["link"].ToString(); string pubDate = dtRow["pubDate"].ToString(); embed.Color = DiscordColor.Black; _ = embed.WithTitle(":newspaper: " + title); _ = embed.WithFooter($"[" + "Source: Google News" + "]"); _ = embed.WithDescription(string.Format("{0:n0}", link) + Environment.NewLine + "**Publication Date: **" + string.Format("{0:n0}", pubDate)); _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false); break; }
                    }
                }
            }
            catch (NullReferenceException)
            { _ = ctx.Channel.SendMessageAsync("ERROR: Data unavailable").ConfigureAwait(false); }
        }
        #endregion

        #region delete
        [Command("delete")]
        [Description("Deletes the specified amount of messages.")]
        [RequireUserPermissions(Permissions.Administrator)]
        [RequireBotPermissions(Permissions.ManageMessages)]
        public async Task PurgeChat(CommandContext ctx, [RemainingText] int amount)
        {
            IReadOnlyList<DiscordMessage> items = await ctx.Channel.GetMessagesAsync(amount + 1).ConfigureAwait(false);
            await ctx.Channel.DeleteMessagesAsync(items).ConfigureAwait(false);
            const int delay = 3000;
            DiscordMessage m = await ctx.Channel.SendMessageAsync($"I have deleted {amount} messages").ConfigureAwait(false);
            await Task.Delay(delay);
            await m.DeleteAsync();
        }
        #endregion

        #region ServerInfo
        [Command("serverinfo")]
        [Description("Get information about the current guild/server.")]
        public async Task ServerInfo(CommandContext ctx)
        {
            DiscordEmbedBuilder output = new DiscordEmbedBuilder()
                .WithAuthor($"Owner: {ctx.Guild.Owner.Username}#{ctx.Guild.Owner.Discriminator}", iconUrl: ctx.Guild.Owner.AvatarUrl ?? string.Empty)
                .WithTitle(ctx.Guild.Name)
                .WithDescription("ID: " + ctx.Guild.Id)
                .AddField("Created on", ctx.Guild.CreationTimestamp.DateTime.ToString(CultureInfo.InvariantCulture), true)
                .AddField("Member Count", ctx.Guild.MemberCount.ToString(), true)
                .AddField("Region", ctx.Guild.VoiceRegion.Name.ToUpperInvariant(), true)
                .AddField("Authentication", ctx.Guild.MfaLevel.ToString(), true)
                .AddField("Content Filter", ctx.Guild.ExplicitContentFilter.ToString(), true)
                .AddField("Verification", ctx.Guild.VerificationLevel.ToString(), true)
                .WithFooter(ctx.Guild.Name + " / #" + ctx.Channel.Name + " / " + DateTime.Now)
                .WithColor(DiscordColor.Rose);
            if (!string.IsNullOrEmpty(ctx.Guild.IconHash))
            {
                _ = output.WithThumbnailUrl(ctx.Guild.IconUrl);
            }

            StringBuilder roles = new StringBuilder();
            foreach (KeyValuePair<ulong, DiscordRole> role in ctx.Guild.Roles)
            {
                _ = roles.Append($"[`{role.Value.Name}`]");
            }

            if (roles.Length == 0)
            {
                _ = roles.Append("None");
            }

            _ = output.AddField("Roles", roles.ToString());

            StringBuilder emojis = new StringBuilder();
            foreach (KeyValuePair<ulong, DiscordEmoji> emoji in ctx.Guild.Emojis)
            {
                _ = emojis.Append(emoji.Value.Name + (!emoji.Equals(ctx.Guild.Emojis.Last()) ? ", " : string.Empty));
            }

            if (emojis.Length != 0)
            {
                _ = output.AddField("Emojis", emojis.ToString(), true);
            }

            _ = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }
        #endregion

        #region invite
        [Command("createinvite")]
        [Description("Retrieve an instant invite link to the server")]
        public async Task createserverinvite(CommandContext ctx)
        {
            _ = await ctx.RespondAsync("Instant Invite to " + Formatter.Bold(ctx.Guild.Name) + ":https://discord.gg/" + ctx.Channel.CreateInviteAsync().Result.Code).ConfigureAwait(false);
        }
        #endregion

        [Hidden]
        [Command("netcut"), RequireUserPermissions(Permissions.ManageRoles)]
        public async Task Netcut(CommandContext ctx)
        {
            if (ctx.User.Id.ToString().Equals(your user id))
            {
                _ = await ctx.Channel.SendMessageAsync("Only the owner can use this command!").ConfigureAwait(false);
            }
            else
            {
                ProcessStartInfo proc = new ProcessStartInfo("ipconfig", "/release")
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true,
                    UseShellExecute = false
                };
                _ = Process.Start(proc);
            };
        }
        #endregion

        #region generatenewmac
        [Hidden]
        [Command("changemac"), RequireUserPermissions(Permissions.ManageRoles)]
        public async Task changemac(CommandContext ctx)
        {
            if (ctx.User.Id.ToString().Equals(689711811335160048))
            { _ = await ctx.Channel.SendMessageAsync("Only the owner can use this command!").ConfigureAwait(false); }
            else
            {
                try
                {
                    _ = await ctx.Channel.SendMessageAsync("Determining Network Adapters...").ConfigureAwait(false); ComboBox netBox = new ComboBox(); foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces().Where(a => Adapter.IsValidMac(a.GetPhysicalAddress().GetAddressBytes(), true)).OrderByDescending(a => a.Speed))
                    { adapternum++; Console.WriteLine(new Adapter(adapter)); _ = netBox.Items.Add(new Adapter(adapter)); }
                    _ = await ctx.Channel.SendMessageAsync("Generating and Changing to New MAC Address...").ConfigureAwait(false); for (int i = 0; i < adapternum; i++)
                    {
                        netBox.SelectedIndex = i; Adapter netBoxSelectedItem = netBox.SelectedItem as Adapter; string ss = Adapter.GetNewMac(); if (netBoxSelectedItem.SetRegistryMac(ss))
                        {
                            Console.WriteLine("[Network " + (i + 1) + " Changed] " + ss);
                        }
                        else
                        {
                            Console.WriteLine("[Network " + (i + 1) + " Not Changed] " + netBoxSelectedItem.Mac);
                        }

                        Thread.Sleep(217);
                    }
                }
                catch
                {
                    IntPtr hnd = WinAPI.GetStdHandle(-11); bool flag = hnd != WinAPI.INVALID_HANDLE_VALUE; if (flag)
                    { WinAPI.CONSOLE_FONT_INFO_EX info = default; info.cbSize = (uint)Marshal.SizeOf<WinAPI.CONSOLE_FONT_INFO_EX>(info); WinAPI.CONSOLE_FONT_INFO_EX newInfo = default; newInfo.cbSize = (uint)Marshal.SizeOf<WinAPI.CONSOLE_FONT_INFO_EX>(newInfo); newInfo.FontFamily = 4; newInfo.dwFontSize = new WinAPI.COORD(info.dwFontSize.X, info.dwFontSize.Y); newInfo.FontWeight = info.FontWeight; _ = WinAPI.SetCurrentConsoleFontEx(hnd, false, ref newInfo); }
                }
            };
        }
        #endregion

        #region shutdown
        [Hidden]
        [Command("shutdown"), RequireUserPermissions(Permissions.ManageRoles)]
        public async Task shutdown(CommandContext ctx)
        {
            if (ctx.User.Id.ToString().Equals(your user id))
            {
                _ = await ctx.Channel.SendMessageAsync("Only the owner can use this command!").ConfigureAwait(false);
            }
            else
            {
                ProcessStartInfo psi = new ProcessStartInfo("shutdown", "/s /t")
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                _ = Process.Start(psi);
            };
        }
        #endregion

        #region discordping

        private readonly string[] pingServer ={
            "https://youtu.be/qZBCMH5WJbo",
            "https://youtu.be/Bol6N1WeoRM",
            "https://youtu.be/u9XDgAfbPFA",
            "https://cdn.discordapp.com/attachments/138522037181349888/438774275546152960/Ping_Discordapp_GIF-downsized_large.gif"
            };

        [Command("discordping")]
        public async Task discordpinging(CommandContext ctx)
        {
            Random random = new Random();
            _ = await ctx.Channel.SendMessageAsync(pingServer[random.Next(pingServer.Length)]).ConfigureAwait(false);
        }
        #endregion

        #region Invite
        [Command("invite")]
        [Description("Invite the bot to a server.")]
        public async Task inviteBot(CommandContext ctx)
        {
            DiscordApplication application = await ctx.Client.GetCurrentApplicationAsync();
            DiscordEmbedBuilder aboutImbed = new DiscordEmbedBuilder
            {
                Title = "Can invite me to your server here " + ctx.User.Username + ":",
                ThumbnailUrl = SharedData.Logo,
                Description = $"<https://discordapp.com/oauth2/authorize?client_id={application.Id}&permissions=8&scope=bot>",
                Color = DiscordColor.Black
            };

            _ = await ctx.Channel.SendMessageAsync(embed: aboutImbed).ConfigureAwait(false);
        }
        #endregion

        #region avatar
        [Command("avatar")]
        [Description("Avatar of someone")]
        public async Task avatar(CommandContext ctx, DiscordMember member)
        {
            {
                DiscordEmbedBuilder aboutImbed = new DiscordEmbedBuilder
                {
                    Title = ":frame_photo: Avatar of " + member.DisplayName + ":",
                    ImageUrl = member.AvatarUrl,
                    Color = new DiscordColor(3, 252, 94)
                };
                _ = await ctx.Channel.SendMessageAsync(embed: aboutImbed).ConfigureAwait(false);
            }
        }
        #endregion

        #region Timer
        [Command("timer")]
        [Description("**%timer** + time in seconds and then it will tell you when it's done!")]
        private async Task test(CommandContext ctx, [RemainingText] int input)
        {
            int time = Convert.ToInt32(input);
            time *= 1000;
            if (input >= 1000)
            {
                _ = await ctx.Channel.SendMessageAsync("El tiempo máximo es de 1000 miliegundos");
            }
            else
            {
                _ = await ctx.Channel.SendMessageAsync(":stopwatch: **" + ctx.User.Mention + "** has put a timer on **" + input + "** seconds! :stopwatch:").ConfigureAwait(false);
                await Task.Delay(time);
                _ = await ctx.Channel.SendMessageAsync(":stopwatch: **" + ctx.User.Mention + "'s** timer on **" + input + "** seconds is done! :stopwatch:").ConfigureAwait(false);
            }
        }
        #endregion

        #region About Bot
        [Command("about")]
        [Description("About [Nova]BOT")]
        public async Task BotCreator(CommandContext ctx)
        {
            TimeSpan uptime = DateTime.Now - SharedData.ProcessStarted;
            DiscordEmbedBuilder output = new DiscordEmbedBuilder()
                .WithTitle(SharedData.Name)
                .WithDescription("A multipurpose Discord bot written in C# with [DSharpPlus](https://github.com/DSharpPlus/DSharpPlus/).")
                .AddField(":tools: [Nova]BOT developed", "by Nova355#7369")
                .AddField(":clock1: Uptime", $"{(int)uptime.TotalDays:00} days {uptime.Hours:00}:{uptime.Minutes:00}:{uptime.Seconds:00}", true)
                .AddField(":link: Links", $"** [Invite]({SharedData.InviteLink}) **| [GitHub]({SharedData.GitHubLink})", true)
                .WithFooter("Thank you for using " + SharedData.Name + $" (v{SharedData.Version})")
                .WithUrl(SharedData.GitHubLink)
                .WithThumbnailUrl(SharedData.Logo)
                .WithColor(DiscordColor.Black);
            _ = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
        }
        #endregion

        #region donate
        [Command("donate")]
        public async Task donate(CommandContext ctx)
        {
            DiscordEmbedBuilder aboutImbed = new DiscordEmbedBuilder
            {
                Title = "Donate",
                Description = SharedData.PayPal,
                Color = new DiscordColor(3, 252, 94)
            };

            _ = await ctx.Channel.SendMessageAsync(embed: aboutImbed).ConfigureAwait(false);
        }
        #endregion

        #region covid 19 info
        [Command("covid")]
        [Description("es or en and country")]
        public async Task covid19(CommandContext ctx, string idiome, [RemainingText] string args)
        {
            switch (idiome)
            {
                case "esp":
                case "español":
                case "es":
                case "spanish":
                    {
                        string translatedText = TranslateText(args, "es", "en"); IRestClient restClient = new RestClient("https://coronavirus-19-api.herokuapp.com/countries/" + translatedText); IRestRequest restRequest = new RestRequest(0); IRestResponse restResponse = restClient.Execute(restRequest);
                        _ = new StringBuilder(); DiscordEmbedBuilder embed = new DiscordEmbedBuilder(); try
                        {
                            embed.Color = DiscordColor.Black; _ = embed.WithTitle("Covid-19-Info-es" + "[" + args + "]"); _ = embed.WithFooter($"[" + "Source: worldometers.info" + "]"); JObject jobject = (JObject)JsonConvert.DeserializeObject(restResponse.Content); JToken jtoken = jobject["cases"]; JToken jtoken2 = jobject["deaths"]; JToken jtoken3 = jobject["recovered"]; JToken jtoken4 = jobject["active"]; JToken jtoken5 = jobject["critical"]; JToken jtoken6 = jobject["todayCases"]; JToken jtoken7 = jobject["todayDeaths"]; bool flag = string.IsNullOrEmpty((string)jtoken); if (flag)
                            { jtoken = 0; }
                            bool flag2 = string.IsNullOrEmpty((string)jtoken2); if (flag2)
                            { jtoken2 = 0; }
                            bool flag3 = string.IsNullOrEmpty((string)jtoken3); if (flag3)
                            { jtoken3 = 0; }
                            bool flag4 = string.IsNullOrEmpty((string)jtoken4); if (flag4)
                            { jtoken4 = 0; }
                            bool flag5 = string.IsNullOrEmpty((string)jtoken5); if (flag5)
                            { jtoken5 = 0; }
                            bool flag6 = string.IsNullOrEmpty((string)jtoken6); if (flag6)
                            { jtoken6 = 0; }
                            bool flag7 = string.IsNullOrEmpty((string)jtoken7); if (flag7)
                            { jtoken7 = 0; }
                            int num = int.Parse((string)jtoken4) + int.Parse((string)jtoken5); _ = embed.WithDescription("**Cases: **" + string.Format("{0:n0}", jtoken) + Environment.NewLine + "**NEW (24H): **" + string.Format("{0:n0}", jtoken6) + Environment.NewLine + "**Active Cases: **" + string.Format("{0:n0}", jtoken4) + Environment.NewLine + "**Deaths: **" + string.Format("{0:n0}", jtoken2) + Environment.NewLine + "**NEW (24H): **" + string.Format("{0:n0}", jtoken7) + Environment.NewLine + "**Critical: **" + "" + string.Format("{0:n0}", jtoken5) + Environment.NewLine + "**Recovered: **" + string.Format("{0:n0}", jtoken3)); _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                        }
                        catch (NullReferenceException)
                        { _ = ctx.Channel.SendMessageAsync("ERROR: Data unavailable").ConfigureAwait(false); }
                    }
                    break;
                case "en":
                case "ingles":
                case "english":
                case "inglés":
                    {
                        IRestClient restClient = new RestClient("https://coronavirus-19-api.herokuapp.com/countries/" + args); IRestRequest restRequest = new RestRequest(0); IRestResponse restResponse = restClient.Execute(restRequest);
                        _ = new StringBuilder(); DiscordEmbedBuilder embed = new DiscordEmbedBuilder(); try
                        {
                            embed.Color = DiscordColor.Black; _ = embed.WithTitle("Covid-19 Info" + "[" + args + "]"); _ = embed.WithFooter($"[" + "Source: worldometers.info" + "]"); JObject jobject = (JObject)JsonConvert.DeserializeObject(restResponse.Content); JToken jtoken = jobject["cases"]; JToken jtoken2 = jobject["deaths"]; JToken jtoken3 = jobject["recovered"]; JToken jtoken4 = jobject["active"]; JToken jtoken5 = jobject["critical"]; JToken jtoken6 = jobject["todayCases"]; JToken jtoken7 = jobject["todayDeaths"]; bool flag = string.IsNullOrEmpty((string)jtoken); if (flag)
                            { jtoken = 0; }
                            bool flag2 = string.IsNullOrEmpty((string)jtoken2); if (flag2)
                            { jtoken2 = 0; }
                            bool flag3 = string.IsNullOrEmpty((string)jtoken3); if (flag3)
                            { jtoken3 = 0; }
                            bool flag4 = string.IsNullOrEmpty((string)jtoken4); if (flag4)
                            { jtoken4 = 0; }
                            bool flag5 = string.IsNullOrEmpty((string)jtoken5); if (flag5)
                            { jtoken5 = 0; }
                            bool flag6 = string.IsNullOrEmpty((string)jtoken6); if (flag6)
                            { jtoken6 = 0; }
                            bool flag7 = string.IsNullOrEmpty((string)jtoken7); if (flag7)
                            { jtoken7 = 0; }
                            int num = int.Parse((string)jtoken4) + int.Parse((string)jtoken5); _ = embed.WithDescription("**Cases: **" + string.Format("{0:n0}", jtoken) + Environment.NewLine + "**NEW (24H): **" + string.Format("{0:n0}", jtoken6) + Environment.NewLine + "**Active Cases: **" + string.Format("{0:n0}", jtoken4) + Environment.NewLine + "**Deaths: **" + string.Format("{0:n0}", jtoken2) + Environment.NewLine + "**NEW (24H): **" + string.Format("{0:n0}", jtoken7) + Environment.NewLine + "**Critical: **" + "" + string.Format("{0:n0}", jtoken5) + Environment.NewLine + "**Recovered: **" + string.Format("{0:n0}", jtoken3)); _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                        }
                        catch (NullReferenceException)
                        { _ = ctx.Channel.SendMessageAsync("ERROR: Data unavailable").ConfigureAwait(false); }
                        break;
                    }
            }
        }
        #endregion

        #region websitesource
        [Command("websitesource")]
        [Description("?websitesource + link")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task websitesource(CommandContext ctx, string args)
        {
            try
            {
                string path = "./source.txt";
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(args);
                WebResponse webResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                StreamReader streamReader = new StreamReader(webResponse.GetResponseStream());
                File.WriteAllText(path, streamReader.ReadToEnd());
                streamReader.Close();
                _ = await ctx.Channel.SendFileAsync($"{path}", "Your source file:");
            }
            catch
            {
                _ = await ctx.Channel.SendMessageAsync($"An error occured...").ConfigureAwait(false);
            }
        }
        #endregion

        #region regionalindicator
        private string output = "";
        [Command("regionalindicator")]
        [Description("Discord Regional Indicator Text Generator")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task regionalindicator(CommandContext ctx, [RemainingText] string args)
        {
            try
            {
                string s = args; foreach (char c in s)
                {
                    if (c.ToString() == "a" || c.ToString() == "A" || c.ToString() == "b" || c.ToString() == "B" || c.ToString() == "c" || c.ToString() == "C" || c.ToString() == "d" || c.ToString() == "D" || c.ToString() == "e" || c.ToString() == "E" || c.ToString() == "f" || c.ToString() == "F" || c.ToString() == "g" || c.ToString() == "G" || c.ToString() == "h" || c.ToString() == "H" || c.ToString() == "i" || c.ToString() == "I" || c.ToString() == "j" || c.ToString() == "J" || c.ToString() == "k" || c.ToString() == "K" || c.ToString() == "l" || c.ToString() == "L" || c.ToString() == "m" || c.ToString() == "M" || c.ToString() == "n" || c.ToString() == "N" || c.ToString() == "o" || c.ToString() == "O" || c.ToString() == "p" || c.ToString() == "P" || c.ToString() == "q" || c.ToString() == "Q" || c.ToString() == "r" || c.ToString() == "R" || c.ToString() == "s" || c.ToString() == "S" || c.ToString() == "t" || c.ToString() == "T" || c.ToString() == "u" || c.ToString() == "U" || c.ToString() == "v" || c.ToString() == "V" || c.ToString() == "w" || c.ToString() == "W" || c.ToString() == "x" || c.ToString() == "X" || c.ToString() == "y" || c.ToString() == "Y" || c.ToString() == "z" || c.ToString() == "Z")
                    { output = output + ":regional_indicator_" + c.ToString().ToLower() + ": "; }
                    if (c.ToString() == "0")
                    { output += ":zero: "; }
                    if (c.ToString() == "1")
                    { output += ":one: "; }
                    if (c.ToString() == "2")
                    { output += ":two: "; }
                    if (c.ToString() == "3")
                    { output += ":three:"; }
                    if (c.ToString() == "4")
                    { output += ":four: "; }
                    if (c.ToString() == "5")
                    { output += ":five: "; }
                    if (c.ToString() == "6")
                    { output += ":six: "; }
                    if (c.ToString() == "7")
                    { output += ":seven: "; }
                    if (c.ToString() == "8")
                    { output += ":eight: "; }
                    if (c.ToString() == "9")
                    { output += ":nine: "; }
                    if (c.ToString() == "#")
                    { output += ":hash: "; }
                    if (c.ToString() == "*")
                    { output += ":asterisk: "; }
                    if (c.ToString() == "!")
                    { output += ":grey_exclamation: "; }
                    if (c.ToString() == "?")
                    { output += ":grey_question: "; }
                    if (c.ToString() == "<")
                    { output += ":arrow_backward: "; }
                    if (c.ToString() == ">")
                    { output += ":arrow_forward: "; }
                    if (c.ToString() == " ")
                    { output += "     "; }
                }
                await Task.Delay(100); _ = await ctx.Channel.SendMessageAsync(output).ConfigureAwait(false); await Task.Delay(100); output = "";
            }
            catch
            { _ = await ctx.Channel.SendMessageAsync($"An error occured...").ConfigureAwait(false); }
        }
        #endregion

        #region 8ball
        [Command("8ball")]
        [Aliases("ask")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task AskEightBall(CommandContext ctx, [RemainingText] string args = null)
        {
            StringBuilder sb = new StringBuilder(); DiscordEmbedBuilder embed = new DiscordEmbedBuilder(); List<string> replies = new List<string> { "si", "no", "tal vez", "deslumbrante...." }; embed.Color = new DiscordColor(0, 255, 0); embed.Title = "🎱 ¡Bienvenido a 8-ball!"; _ = embed.WithFooter($"8-ball creado por: {ctx.User.Username}"); _ = sb.AppendLine($","); _ = sb.AppendLine(); if (args == null)
            { _ = sb.AppendLine("Lo sentimos, no puedo responder una pregunta que no hiciste."); }
            else
            {
                string answer = replies[new Random().Next(replies.Count - 1)]; _ = sb.AppendLine($"Preguntaste:  [" + args + "]..."); _ = sb.AppendLine(); _ = sb.AppendLine($"...tu respuesta es:  [" + answer + "]"); switch (answer)
                {
                    case "si": { _ = embed.WithColor(new DiscordColor(0, 255, 0)); break; }
                    case "no": { _ = embed.WithColor(new DiscordColor(255, 0, 0)); break; }
                    case "tal vez": { _ = embed.WithColor(new DiscordColor(255, 255, 0)); break; }
                    case "deslumbrante....": { _ = embed.WithColor(new DiscordColor(255, 0, 255)); break; }
                }
            }
            embed.Description = sb.ToString(); _ = await ctx.Channel.SendMessageAsync(null, false, embed.Build()).ConfigureAwait(false);
        }
    }
    #endregion
}
