namespace TelegramBot.Helpers;

public static class CommandKeyHelper
{
    public static string Create(string command) => $"{command.TrimStart('/')}_";
}
