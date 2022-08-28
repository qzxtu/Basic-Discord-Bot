using NovaBOT.Models;
using NovaBOT.Services;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NovaBOT.Commands
{
    internal class FunCommands : BaseCommandModule
    {
        #region secure webclient
        protected WebClient SecureWebClient()
        {
            WebClient w = new WebClient
            { Proxy = null }; return w;
        }
        #endregion

        #region Meme
        [Command("meme")]
        public async Task SendMeme(CommandContext ctx)
        {
            IRestClient restClient = new RestClient("https://meme-api.herokuapp.com/gimme");
            IRestRequest restRequest = new RestRequest(0);
            IRestResponse restResponse = restClient.Execute(restRequest);
            _ = new Random();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            try
            {
                JObject jobject = (JObject)JsonConvert.DeserializeObject(restResponse.Content);
                JToken jtoken = jobject["title"];
                JToken jtoken2 = jobject["url"];
                JToken jtoken3 = jobject["postLink"];
                bool flag = string.IsNullOrEmpty((string)jtoken);
                if (flag)
                {
                    jtoken = 0;
                }
                bool flag2 = string.IsNullOrEmpty((string)jtoken2);
                if (flag2)
                {
                    jtoken2 = 0;
                }
                bool flag3 = string.IsNullOrEmpty((string)jtoken3);
                if (flag3)
                {
                    jtoken3 = 0;
                }
                _ = embed.WithDescription("**" + string.Format("{0:n0}", $"[{jtoken}]({jtoken3})" + "**"));
                _ = embed.WithImageUrl(string.Format("{0:n0}", jtoken2));
                _ = embed.WithColor(DiscordColor.Black);
                _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
            }
            catch (NullReferenceException)
            {
                _ = await ctx.Channel.SendMessageAsync("ERROR: Data unavailable").ConfigureAwait(false);
            }
        }
        #endregion

        #region gay
        [Command("gay")]
        public async Task gay(CommandContext ctx, DiscordUser user)
        {
            Random random = new Random();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            string result = $" {random.Next(0, 100)}";
            _ = embed.WithTitle("Gay Machine :rainbow_flag:");
            _ = embed.WithDescription($"<@{user.Id}> is " + result + "% gay.");
            _ = embed.WithColor(DiscordColor.Black);
            _ = embed.WithImageUrl("https://vanidad.es/images/carpeta_gestor/archivos/2018/06/29/vanidad.es_orgullo-gay.gif");
            _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(true);
        }
        #endregion

        #region waifu
        [Command("waifu")]
        public async Task waifu(CommandContext ctx, DiscordUser user)
        {
            Random random = new Random();
            string result = $" {random.Next(-1, 100)}";
            int args = int.Parse(result);
            if (args >= 10)
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                _ = embed.WithTitle("Waifu Machine");
                _ = embed.WithDescription($"<@{user.Id}> " + result + "/100 waifu :smirk:.");
                _ = embed.WithColor(DiscordColor.Black);
                _ = embed.WithImageUrl("https://media1.tenor.com/images/134212ba34a8099c993e07a686345f84/tenor.gif?itemid=8215787");
                _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(true);

            }
            else
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                _ = embed.WithTitle("Waifu Machine");
                _ = embed.WithDescription($"<@{user.Id}> " + result + "/100 waifu.");
                _ = embed.WithColor(DiscordColor.Black);
                _ = embed.WithImageUrl("https://media1.tenor.com/images/134212ba34a8099c993e07a686345f84/tenor.gif?itemid=8215787");
                _ = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(true);
            }
        }
        #endregion

        #region blank 
        [Command("blank")]
        public async Task blank(CommandContext ctx)
        {
            _ = await ctx.Channel.SendMessageAsync("឵឵\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n឵឵");
        }
        #endregion

        #region Animals
        [Command("husky")]
        public async Task husky(CommandContext ctx)
        {
            dynamic res = JsonConvert.DeserializeObject(Globals.hclient.GetAsync("https://dog.ceo/api/breed/husky/images/random").Result.Content.ReadAsStringAsync().Result);
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Black,
                ImageUrl = res.message
            };
            _ = await ctx.Channel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }

        [Command("shibe")]
        public async Task shibe(CommandContext ctx)
        {
            dynamic res = JsonConvert.DeserializeObject(Globals.hclient.GetStringAsync("http://shibe.online/api/shibes").Result);
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Black,
                ImageUrl = res[0]
            };
            _ = await ctx.Channel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }

        [Command("cat")]
        public async Task cat(CommandContext ctx)
        {
            dynamic res = JsonConvert.DeserializeObject(Globals.hclient.GetStringAsync("http://shibe.online/api/cats").Result);
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Black,
                ImageUrl = res[0]
            };
            _ = await ctx.Channel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }

        [Command("bird")]
        public async Task bird(CommandContext ctx)
        {
            dynamic res = JsonConvert.DeserializeObject(Globals.hclient.GetStringAsync("http://shibe.online/api/birds").Result);
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder()
            {
                Color = DiscordColor.Black,
                ImageUrl = res[0]
            };
            _ = await ctx.Channel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }
        #endregion

        #region aspect
        private readonly string[] aspect ={
            "https://tenor.com/view/shocked-shocked-face-shock-oh-my-god-what-gif-14759370",
            "https://tenor.com/view/monkey-gif-8660294",
            "https://tenor.com/view/proboscis-monkey-big-nose-simian-eating-gif-16955749",
            "https://tenor.com/view/proboscis-monkey-eat-monkey-eating-munch-gif-5362874"
            };

        [Command("aspect")]
        public async Task discordpinging(CommandContext ctx)
        {
            Random random = new Random();
            _ = await ctx.Channel.SendMessageAsync(aspect[random.Next(aspect.Length)]).ConfigureAwait(false);
        }
        #endregion

        #region chiste
        [Command("chiste")]
        private async Task Chiste(CommandContext ctx)
        {
            string[] chistes = new string[] { " ¿Una robopilingui de trecientos pavos o trescientas robopilinguis de a dolar?", " Tengo que dejar de atropellar gente.No soy lo suficientemente famoso como para librarme.", " ¿Que clase de fiesta es esta?, ¡No hay alcohol y sólo se ve una furcia!", " ¡Qué horrible pesadilla! ¡Unos y ceros por todas partes!Hasta me pareció ver un 2", " Funcionan igual que los demás organismos vivos, disparándose el ADN unos a otros para fabricar crías. ¡Es insultante!", " Bender: “Díselo a mi brillante culo metálico. " + Environment.NewLine + "Fry: “A mi no me parece tan brillante… " + Environment.NewLine + "Bender: “Más que el tuyo cacho carne.”", " Bender: “Tras hojear su carta de vinos aguardentosos, he seleccionado Delicia del Vagabundo del 71, Chateau La Juerga del 57 y Sobignon Melopea del 66.”" + Environment.NewLine + " Camarero: “Exquisita elección señor.”" + Environment.NewLine + " Bender: “Y… mezclemelos todos en una jarra grande.", " Fry: “¿Tú a que partido votas, Bender ?”" + Environment.NewLine + " Bender: “No.Yo no puedo votar.”" + Environment.NewLine + " Fry: “¿Por ser robot ?”" + Environment.NewLine + " Bender: “No.Criminal convicto.”", " ¿Han probado alguna vez a apagar la tele, sentarse con sus hijos y darles una paliza?", " Todos le amaron cuando era Bender el que Ofende, ahora todos le odiarán por ser… Indefinido Bender", " Dile a Don Bot que dejo el crimen organizado, a partir de ahora me dedicaré sólo al crimen normal", " Siempre quise saber si puedo fastidiar a la gente más de lo que la fastidio", " Llámame anticuado, pero me gusta que un abandono sea tan imprevisto como cruel", " Ahh, ¿no hay sitio para Bender, eh? Vale, me construiré mi propio módulo lunar, con casinos, y furcias.Es más, paso de la nave lunar… y de los casinos. ¡Al cuerno todo!", "El cuerpo es para las furcias y los tios gordos, yo solo necesito pasta alrededor de mi cabeza", " Te juro que es cierto, tengo a tu Dios como mi testigo", " ¡No quiero un amigo! ¡Quiero una operación de cambio de sexo y la quiero ahora!", " Fry, de todos los amigos que he tenido, tú… el único", " Se hizo popular cuando prometió que no mataría a todo el que se encontrara a su paso", " La abogada soltera, lucha por su cliente, lleva minifaldas provocativas y además es autosuficiente. ¿A que no lo hago mal?", " Y a pesar de que el ordenador estaba apagado y desenchufado, una imagen permanecía en la pantalla… era… ¡¡el logotipo de Windows!!", " No quiero morir, todavía hay muchas cosas que no tengo", " Alguien: ¿No te importa vivir con un humano? Bender: No, siempre quise tener una mascota", " ¿Activar antivirus? ¡Me estoy bajando porno!No ", " Chantaje es una palabra muy fea, yo prefiero… extorsión, la X le da mucha clase ", " La llevaré con orgullo y la empeñaré en cuanto pueda ", " Comparad vuestras vidas con la mia… y luego podeis suicidaros" };
            Random random = new Random();
            _ = await ctx.Channel.SendMessageAsync(chistes.GetValue(random.Next(chistes.Length)).ToString()).ConfigureAwait(false);
        }
        #endregion

        #region pick
        [Command("pick")]
        public async Task Pick(CommandContext ctx, [RemainingText] string message)
        {
            string[] options = message.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            Random rand = new Random();
            string picked = options[rand.Next(0, options.Length)];
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            _ = embed.WithTitle("I have chosen: ");
            _ = embed.WithDescription(picked);
            _ = embed.WithColor(new DiscordColor(0, 255, 255));
            _ = await ctx.Channel.SendMessageAsync("", false, embed).ConfigureAwait(false);
        }
        #endregion

        #region owo

        private readonly string[] uwus = {"𝓤𝔀𝓤", "ÚwÚ", "(。U ω U。)", "(⁄˘⁄ ⁄ ω⁄ ⁄ ˘⁄)♡", "end my suffering",
         "✧･ﾟ: *✧･ﾟ♡*(ᵘʷᵘ)*♡･ﾟ✧*:･ﾟ✧", "𝒪𝓌𝒪", "(⁄ʘ⁄ ⁄ ω⁄ ⁄ ʘ⁄)♡", "uwu"};

        [Command("uwu")]
        [Description("*uwu*")]
        public async Task degenerecy(CommandContext ctx)
        {
            Random rng = new Random();
            _ = await ctx.RespondAsync($"{uwus[rng.Next(uwus.Length)]}");
        }
        #endregion

        #region HELLO

        [Command("hello")]
        [Aliases("hi", "howdy")]
        [Description("Welcome another user to the server")]
        public async Task Greet(CommandContext ctx, [Description("User to say hello to")][RemainingText] DiscordMember member)
        {
            _ = member is null
                ? await ctx.RespondAsync(":wave: Hello, " + ctx.User.Mention).ConfigureAwait(false)
                : await ctx.RespondAsync(":wave: Welcome " + member.Mention + " to " + ctx.Guild.Name + ". Enjoy your stay!").ConfigureAwait(false);
        }

        #endregion

        #region kill
        [Command("kill")]
        public async Task kill(CommandContext ctx, [RemainingText] string Victim)
        {
            if (Victim.Contains(ctx.User.Username))
            {
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                _ = embed.WithTitle(ctx.User.Username + " has died...");
                _ = embed.WithDescription(string.Format($"{ctx.User.Username} has commit suicide..."));
                _ = embed.WithThumbnailUrl("https://myanimelist.cdn-dena.com/s/common/uploaded_files/1453949904-c2ea6ea591f839374da8993f0764f78b.jpeg");
                _ = embed.WithColor(new DiscordColor(139, 0, 0));
                _ = await ctx.Channel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
            }
            else
            {
                string[] how = new string[7];
                how[0] = "by spraying them with a submachine gun!";
                how[1] = "by shooting them 5 times with a pistol!";
                how[2] = "by shaking them multiple times with a knife!";
                how[3] = "by beating them to death with a baseball bat!";
                how[4] = "by hiring a hitman!";
                how[5] = "by running them over!";
                how[6] = "by yeeting them **across the universe**";
                Random rand = new Random();
                string picked = how[rand.Next(0, how.Length)];
                DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
                _ = embed.WithTitle(ctx.User.Username + " has blood on their hands.");
                _ = embed.WithDescription(string.Format("{0} killed {1} {2}", ctx.User.Mention, Victim, picked));
                _ = embed.WithThumbnailUrl("https://myanimelist.cdn-dena.com/s/common/uploaded_files/1453949904-c2ea6ea591f839374da8993f0764f78b.jpeg");
                _ = embed.WithColor(new DiscordColor(0, 255, 255));
                _ = await ctx.Channel.SendMessageAsync("", false, embed).ConfigureAwait(false);
            }
        }
        #endregion

        #region suicide
        [Command("suicide")]
        public async Task CommitDie(CommandContext ctx)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            _ = embed.WithTitle(ctx.User.Username + " has died...");
            _ = embed.WithDescription(string.Format($"{ctx.User.Username} has commit suicide..."));
            _ = embed.WithThumbnailUrl("https://myanimelist.cdn-dena.com/s/common/uploaded_files/1453949904-c2ea6ea591f839374da8993f0764f78b.jpeg");
            _ = embed.WithColor(new DiscordColor(139, 0, 0));
            _ = await ctx.Channel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }
        #endregion

        #region marry
        [Command("marry")]
        public async Task Marry(CommandContext ctx, [RemainingText] string person)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            _ = embed.WithTitle("Congratulations!");
            _ = embed.WithDescription(string.Format("{0} marries {1}", ctx.User.Mention, person));
            _ = embed.WithThumbnailUrl("https://i.pinimg.com/236x/a7/ca/8c/a7ca8c84970f951d08b196bab75a4992--anime-wedding-kirito-asuna.jpg");
            _ = embed.WithColor(new DiscordColor(255, 105, 180));
            _ = await ctx.Channel.SendMessageAsync("", false, embed).ConfigureAwait(false);
        }
        #endregion

        #region divorce
        [Command("divorce")]
        public async Task Divorce(CommandContext ctx, [RemainingText] string person)
        {
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            _ = embed.WithTitle("I am sorry :(");
            _ = embed.WithDescription(string.Format("{0} gets a divorce with {1}", ctx.User.Mention, person));
            _ = embed.WithThumbnailUrl("https://media.giphy.com/media/3fmRTfVIKMRiM/giphy.gif");
            _ = embed.WithColor(new DiscordColor(139, 0, 0));
            _ = await ctx.Channel.SendMessageAsync("", false, embed).ConfigureAwait(false);
        }
        #endregion

        #region randomperson
        [Command("randomperson")]
        public async Task getRandomPerson(CommandContext ctx)
        {
            WebClient wc = SecureWebClient();
            string json;
            {
                json = wc.DownloadString("https://randomuser.me/api/?nat=US");
            }
            dynamic dataObject = JsonConvert.DeserializeObject<dynamic>(json);
            string nat = dataObject.results[0].nat.ToString();
            string gender = dataObject.results[0].gender.ToString();
            string gender2 = new CultureInfo("en-US").TextInfo.ToTitleCase(gender);
            string title = dataObject.results[0].name.title.ToString();
            string title2 = new CultureInfo("en-US").TextInfo.ToTitleCase(title);
            string firstname = dataObject.results[0].name.first.ToString();
            string firstname2 = new CultureInfo("en-US").TextInfo.ToTitleCase(firstname);
            string lastname = dataObject.results[0].name.last.ToString();
            string lastname2 = new CultureInfo("en-US").TextInfo.ToTitleCase(lastname);
            string picture = dataObject.results[0].picture.large.ToString();
            string dateofbirth = dataObject.results[0].dob.date.ToString();
            string age = dataObject.results[0].dob.age.ToString();
            DiscordEmbedBuilder embed = new DiscordEmbedBuilder();
            _ = embed.WithDescription($"Full Name: {title2 + "."} {firstname2} {lastname2}\nNationality: {nat}\nGender: {gender2}\nDate of Birth: {dateofbirth}\nAge: {age}");
            _ = embed.WithColor(new DiscordColor(0, 255, 255));
            _ = embed.WithThumbnailUrl(picture);
            _ = embed.WithTitle("Random Person:");
            _ = await ctx.Channel.SendMessageAsync("", embed: embed).ConfigureAwait(false);
        }
        #endregion

        #region poll
        [Command("poll")]
        public async Task Poll(CommandContext ctx, TimeSpan duration, params DiscordEmoji[] emojiOptions)
        {
            InteractivityExtension interactivity = ctx.Client.GetInteractivity();
            System.Collections.Generic.IEnumerable<string> options = emojiOptions.Select(x => x.ToString());
            DiscordEmbedBuilder pollEmbed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };
            DiscordMessage pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);
            foreach (DiscordEmoji option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }
            System.Collections.ObjectModel.ReadOnlyCollection<DSharpPlus.Interactivity.EventHandling.Reaction> result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);
            System.Collections.Generic.IEnumerable<DSharpPlus.Interactivity.EventHandling.Reaction> distinctResult = result.Distinct();
            System.Collections.Generic.IEnumerable<string> results = distinctResult.Select(x => $"{x.Emoji}: {x.Total}");
            _ = await ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
        }
        #endregion

        #region spam
        [Command("spam")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task spamming(CommandContext ctx, int times, string message)
        {
            if (ctx.Guild.Id.Equals(551461216598622236))
            {
                if (times > 5)
                {
                    await ctx.Message.DeleteAsync();
                    _ = await ctx.RespondAsync(ctx.User.Mention + "*YOU CANNOT GO OVER 5*").ConfigureAwait(false);
                }
                else
                {
                    if (message.ToString() == "@everyone")
                    {
                        _ = await ctx.RespondAsync("Do not mention everyone!").ConfigureAwait(false);
                    }
                    else
                    {
                        for (int i = 0; i < times; i++)
                        {
                            _ = await ctx.RespondAsync(message).ConfigureAwait(false);
                            _ = Task.Delay(1000);
                        }
                    }
                }
            }
            else
            {
                _ = ctx.Channel.SendMessageAsync("Only the owner can use this command!").ConfigureAwait(false);
            }
        }
        #endregion

        #region spam ping
        [Command("spamping")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task spamping(CommandContext ctx, int times, DiscordUser user)
        {
            if (ctx.Guild.Id.Equals(551461216598622236))
            {
                if (times > 5)
                {
                    _ = await ctx.RespondAsync(ctx.User.Mention + "*You Cannot Go Over 5*").ConfigureAwait(false);
                }
                else
                {
                    for (int i = 0; i < times; i++)
                    {
                        _ = await ctx.RespondAsync($"{user.Mention}").ConfigureAwait(false);
                        _ = Task.Delay(1000);
                    }
                }
            }
            else
            {
                _ = ctx.Channel.SendMessageAsync("Only the owner can use this command!").ConfigureAwait(false);
            }
        }
        #endregion

        #region math
        [Command("math")]
        [Aliases("calculate")]
        [Description("Perform a basic math operation")]
        public async Task Math(CommandContext ctx, [Description("First operand")] double num1, [Description("Operator")] string operation, [Description("Second operand")] double num2)
        {
            try
            {
                double result; switch (operation)
                { default: result = num1 + num2; break; case "-": result = num1 - num2; break; case "*": case "x": result = num1 * num2; break; case "/": result = num1 / num2; break; case "%": result = num1 % num2; break; }
                DiscordEmbedBuilder output = new DiscordEmbedBuilder().WithDescription($":1234: The result is {result:#,##0.00}").WithColor(DiscordColor.CornflowerBlue); _ = await ctx.RespondAsync(embed: output.Build()).ConfigureAwait(false);
            }
            catch
            { _ = ctx.Channel.SendMessageAsync("ERROR: Data unavailable").ConfigureAwait(false); }
        }
        #endregion
    }
}
