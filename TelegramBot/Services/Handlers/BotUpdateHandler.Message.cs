using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramBot.Services;

public partial class BotUpdateHandler
{
    private async Task HandleMessageAsync(ITelegramBotClient client, Message? message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(nameof(message));

        var from = message?.From;

        _logger.LogInformation("Received Message from {from.FirstName}: {message.Text}", from?.FirstName, message?.Text);
    }
}
