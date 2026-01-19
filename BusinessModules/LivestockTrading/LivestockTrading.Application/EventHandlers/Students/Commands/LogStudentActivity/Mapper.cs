namespace LivestockTrading.Application.EventHandlers.Students.Commands.LogStudentActivity;

public class Mapper
{
    public ResponseModel MapToResponse(int processedCount, string message)
    {
        return new ResponseModel
        {
            ProcessedCount = processedCount,
            ProcessedAt = DateTime.UtcNow,
            Message = message
        };
    }
}
