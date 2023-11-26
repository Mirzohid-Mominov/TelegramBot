using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Services;

public partial class BotUpdateHandler
{
    private async Task HandleMessageAsync(ITelegramBotClient client, Message? message, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(nameof(message));

        var from = message?.From;

        _logger.LogInformation("Received Message from {from.FirstName}", from?.FirstName);

        var handler = message?.Type switch
        {
            MessageType.Text => HandleTextMessageAsync(client, message, cancellationToken),
            _ => HandleUnknownMessageAsync(client, message, cancellationToken)
        };

        await handler;
    }

    private Task HandleUnknownMessageAsync(ITelegramBotClient client, Message message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received message type {message.Type}", message.Type);

        return Task.CompletedTask;
    }

    private async Task HandleTextMessageAsync(ITelegramBotClient client, Message message, CancellationToken cancellationToken)
    {
        var from = message.From;

        _logger.LogInformation("From: {from.FirstName}", from?.FirstName);

        await client.SendTextMessageAsync(
            chatId: message.Chat.Id,
            text: _localizer["greeting"],
            replyToMessageId: message.MessageId, 
            cancellationToken: cancellationToken);

    }
}
