
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using TelegramBot.Services;

namespace TelegramBot
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var token = builder.Configuration.GetValue("BotToken", string.Empty);

            //builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>();

            builder.Services.AddSingleton(new TelegramBotClient(token));
            builder.Services.AddHostedService<BotBackGroundServices>();
            builder.Services.AddSingleton<IUpdateHandler, BotUpdateHandler>();

            var app = builder.Build();

            app.Run();
        }
    }
}