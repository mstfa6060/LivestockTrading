namespace Livestock.Features.ContactForms;

public record ContactFormItem(
    Guid Id,
    string Name,
    string Email,
    string Subject,
    string Message,
    bool IsRead,
    Guid? UserId,
    DateTime CreatedAt);

public record CreateContactFormRequest(
    string Name,
    string Email,
    string Subject,
    string Message);

public record GetContactFormRequest(Guid Id);
public record MarkContactFormReadRequest(Guid Id);
public record DeleteContactFormRequest(Guid Id);
