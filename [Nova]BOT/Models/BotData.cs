using Newtonsoft.Json;

namespace NovaBOT.Models
{
    public class TokenData
    {
        [JsonProperty("prefix")]
        public string CommandPrefix { get; private set; }

        [JsonProperty("discord")]
        public string DiscordToken { get; private set; }
    }
    public enum EmbedType
    {
        Default,
        Good,
        Warning,
        Missing,
        Error
    }

    public enum UserStateChange
    {
        Ban,
        Deafen,
        Kick,
        Mute,
        Unban,
        Undeafen,
        Unmute
    }
}