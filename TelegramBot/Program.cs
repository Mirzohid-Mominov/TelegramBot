using Telegram.Bot;
using Telegram.Bot.Polling;
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

            builder.Services.AddSingleton(p => new TelegramBotClient(token));
            builder.Services.AddHostedService<BotBackGroundServices>();
            builder.Services.AddSingleton<IUpdateHandler, BotUpdateHandler>();

            builder.Services.AddLocalization();

            var app = builder.Build();

            builder.Services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en-US", "fr" };
                options.SetDefaultCulture(supportedCultures[0])
                    .AddSupportedCultures(supportedCultures)
                    .AddSupportedUICultures(supportedCultures);
            });

            //app.UseRequestLocalization(localizationOptions);

            app.Run();
        }
    }
}
