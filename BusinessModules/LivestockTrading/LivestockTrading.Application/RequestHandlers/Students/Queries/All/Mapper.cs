namespace LivestockTrading.Application.RequestHandlers.Students.Queries.All;

public class Mapper
{
    public List<ResponseModel> MapToResponse(List<Student> students)
    {
        return students.Select(s => new ResponseModel
        {
            Id = s.Id,
            StudentNumber = s.StudentNumber,
            FirstName = s.FirstName,
            LastName = s.LastName,
            FullName = $"{s.FirstName} {s.LastName}",
            Email = s.Email,
            PhoneNumber = s.PhoneNumber,
            BirthDate = s.BirthDate,
            Department = s.Department,
            IsActive = s.IsActive,
            CreatedAt = s.CreatedAt
        }).ToList();
    }
}
