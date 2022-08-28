using DSharpPlus.Entities;
using System;
using System.Reflection;

namespace NovaBOT.Models
{
    public class SharedData
    {
        public static string Name { get; } = "[Nova]BOT";
        public static string Version { get; } = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        public static string Logo { get; set; } = "https://cdn.discordapp.com/attachments/689717323543609386/747898267630829739/NOVABOT_LOGO.PNG";
        public static string GitHubLink { get; set; } = "https://github.com/qzxtu";
        public static string PayPal { get; set; } = "https://www.paypal.me/nova355killer";
        public static string InviteLink { get; } = "https://discord.com/api/oauth2/authorize?client_id=744245102712717407&permissions=8&scope=bot";
        public static DiscordColor DefaultColor { get; set; } = new DiscordColor("#00FF7F");
        public static DateTime ProcessStarted { get; set; }
    }
}