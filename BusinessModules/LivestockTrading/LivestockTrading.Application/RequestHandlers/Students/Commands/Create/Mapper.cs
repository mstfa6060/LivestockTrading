namespace LivestockTrading.Application.RequestHandlers.Students.Commands.Create;

public class Mapper
{
    public Student MapToNewEntity(RequestModel m)
    {
        return new Student
        {
            Id = Guid.NewGuid(),
            StudentNumber = m.StudentNumber,
            FirstName = m.FirstName,
            LastName = m.LastName,
            Email = m.Email,
            PhoneNumber = m.PhoneNumber,
            BirthDate = m.BirthDate,
            Department = m.Department,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public ResponseModel MapToResponse(Student s)
    {
        return new ResponseModel { Id = s.Id };
    }
}
