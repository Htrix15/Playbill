
namespace TelegramBot.Helpers;

public static class CollbackCommandHelper
{
    public static string Create(string command) => command.TrimStart('/');
}
