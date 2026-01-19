using Microsoft.Extensions.Logging;

namespace LivestockTrading.Application.EventHandlers.Students.Commands.LogStudentActivity;

/// <summary>
/// Öğrenci Aktivite Log İşlemi
/// Bu event handler, son X gün içinde oluşturulan/güncellenen öğrencileri loglar.
/// Hangfire job tarafından periyodik olarak çağrılabilir.
/// </summary>
[EventHandler]
public class Handler : IRequestHandler
{
    private readonly DataAccess _dal;
    private readonly ILogger<Handler> _logger;

    public Handler(ArfBlocksDependencyProvider dp, object dataAccess)
    {
        _dal = (DataAccess)dataAccess;

        try
        {
            _logger = dp.GetInstance<ILogger<Handler>>();
        }
        catch
        {
            _logger = null;
        }
    }

    private void Log(string message)
    {
        if (_logger != null)
            _logger.LogInformation(message);
        else
            Console.WriteLine(message);
    }

    private void LogError(Exception ex, string message)
    {
        if (_logger != null)
            _logger.LogError(ex, message);
        else
            Console.WriteLine($"ERROR: {message} - {ex.Message}");
    }

    public async Task<ArfBlocksRequestResult> Handle(
        IRequestModel payload,
        EndpointContext context,
        CancellationToken cancellationToken)
    {
        try
        {
            var mapper = new Mapper();
            var request = (RequestModel)payload;

            Log($"[LogStudentActivity] Job başlatıldı: {DateTime.UtcNow}");

            // Son X gün içinde oluşturulan/güncellenen öğrencileri getir
            var recentStudents = await _dal.GetRecentStudents(request.DaysToKeep, cancellationToken);

            if (!recentStudents.Any())
            {
                Log($"[LogStudentActivity] Son {request.DaysToKeep} gün içinde aktivite bulunamadı");
                return ArfBlocksResults.Success(mapper.MapToResponse(0, "Aktivite bulunamadı"));
            }

            Log($"[LogStudentActivity] {recentStudents.Count} öğrenci aktivitesi bulundu");

            // Her öğrenci için log oluştur
            foreach (var student in recentStudents)
            {
                var activityType = student.UpdatedAt.HasValue ? "Güncellendi" : "Oluşturuldu";
                var activityDate = student.UpdatedAt ?? student.CreatedAt;

                Log($"[LogStudentActivity] Öğrenci: {student.FirstName} {student.LastName} ({student.StudentNumber}) - {activityType}: {activityDate:yyyy-MM-dd HH:mm}");
            }

            Log($"[LogStudentActivity] Job tamamlandı. {recentStudents.Count} aktivite loglandı");

            return ArfBlocksResults.Success(mapper.MapToResponse(recentStudents.Count, "Aktiviteler başarıyla loglandı"));
        }
        catch (Exception ex)
        {
            LogError(ex, "[LogStudentActivity] Job hatası");
            throw;
        }
    }
}
