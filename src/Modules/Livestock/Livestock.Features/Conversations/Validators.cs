using FastEndpoints;
using FluentValidation;

namespace Livestock.Features.Conversations;

public class CreateConversationValidator : Validator<CreateConversationRequest>
{
    public CreateConversationValidator()
    {
        RuleFor(x => x.RecipientUserId).NotEmpty();
        RuleFor(x => x.FirstMessage).NotEmpty().MaximumLength(5000);
    }
}

public class SendMessageValidator : Validator<SendMessageRequest>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.Content).NotEmpty().MaximumLength(5000);
    }
}
