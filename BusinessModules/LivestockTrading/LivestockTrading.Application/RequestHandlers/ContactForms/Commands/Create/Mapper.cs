using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.ContactForms.Commands.Create;

public class Mapper
{
	public ContactForm MapToEntity(RequestModel request)
	{
		return new ContactForm
		{
			Id = Guid.NewGuid(),
			Name = request.Name,
			Email = request.Email,
			Subject = request.Subject,
			Message = request.Message,
			IsRead = false,
			CreatedAt = DateTime.UtcNow
		};
	}
}
