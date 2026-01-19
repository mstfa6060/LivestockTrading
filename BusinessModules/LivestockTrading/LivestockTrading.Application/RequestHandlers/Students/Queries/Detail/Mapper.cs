namespace LivestockTrading.Application.RequestHandlers.Students.Queries.Detail;

public class Mapper
{
    public ResponseModel MapToResponse(Student s)
    {
        return new ResponseModel
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
            CreatedAt = s.CreatedAt,
            UpdatedAt = s.UpdatedAt
        };
    }
}
