using DtekShutdownCheckBot.Shared.Entities;

namespace DtekShutdownCheckBot.Extensions;

public static class ModelExtensions
{
    public static bool ContainsCity(this Chat chat, string city)
    {
        if (chat.IsNull())
        {
            throw new NullReferenceException(nameof(chat));
        }

        if (city == null)
        {
            throw new ArgumentNullException(nameof(city));
        }

        if (chat.Words.IsNull() || !chat.Words.Any())
        {
            throw new ArgumentException(nameof(chat.Words));
        }

        return chat.Words.Any(w => string.Equals(w.Value,city,StringComparison.InvariantCultureIgnoreCase));
    }

}