using LivestockTrading.Domain.Entities;

namespace LivestockTrading.Application.RequestHandlers.PaymentMethods.Queries.Pick;

public class Mapper
{
	public List<ResponseModel> MapToResponse(List<PaymentMethod> items)
	{
		return items.Select(p => new ResponseModel
		{
			Id = p.Id,
			Name = p.Name,
			Code = p.Code
		}).ToList();
	}
}
