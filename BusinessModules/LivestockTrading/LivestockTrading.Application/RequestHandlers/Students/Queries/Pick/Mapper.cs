namespace LivestockTrading.Application.RequestHandlers.Students.Queries.Pick;

public class Mapper
{
    public List<ResponseModel> MapToResponse(List<Student> students)
    {
        return students.Select(s => new ResponseModel
        {
            Id = s.Id,
            Name = $"{s.FirstName} {s.LastName}",
            StudentNumber = s.StudentNumber
        }).ToList();
    }
}
