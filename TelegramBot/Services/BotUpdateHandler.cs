using Microsoft.Extensions.Localization;
using System.Globalization;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Resources;

namespace TelegramBot.Services;

public partial class BotUpdateHandler : IUpdateHandler
{
    private readonly ILogger<BotUpdateHandler> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IStringLocalizer _localizer;
    private UserService _userService;

    public BotUpdateHandler(ILogger<BotUpdateHandler> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Error Occured with Telegram Bot: {ex.Message}", exception);

        return Task.CompletedTask;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await SetCultureForUser(update);

        var culture = new CultureInfo("en-US");

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        using var scope = _scopeFactory.CreateScope();

        _localizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<BotLocalizer>>();

        _userService = scope.ServiceProvider.GetRequiredService<UserService>();

        var handler = update.Type switch
        {
            UpdateType.Message => HandleMessageAsync(botClient, update.Message, cancellationToken),
            UpdateType.EditedMessage => HandleEditMessageAsync(botClient, update.EditedMessage, cancellationToken),
            // handle other updates
            _ => HandleUnknownUpdate(botClient, update, cancellationToken)
        };

        try
        {
            await handler;
        }
        catch(Exception ex)
        {
            await HandlePollingErrorAsync(botClient, ex, cancellationToken);
        }
    }

    private async Task SetCultureForUser(Update update)
    {
        var from = update.Type switch
        {
            UpdateType.Message => update.Message?.From,
            UpdateType.EditedMessage => update.EditedMessage?.From,
            UpdateType.CallbackQuery => update.CallbackQuery?.From,
            UpdateType.InlineQuery => update.InlineQuery?.From,
            _ => update.Message?.From
        };

        var user = await _userService.GetUserAsync(from.Id);
    }

    private Task HandleUnknownUpdate(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update type {update.Type} received", update.Type);

        return Task.CompletedTask;
    }
}
