// namespace LivestockTrading.Application.EventHandlers.Students.Commands.LogStudentActivity;

// /// <summary>
// /// Öğrenci aktivite log işlemi için request model
// /// Hangfire job tarafından çağrılabilir
// /// </summary>
// public class RequestModel : IRequestModel
// {
//     /// <summary>
//     /// Kaç gün öncesine kadar log tutulacak (varsayılan: 7 gün)
//     /// </summary>
//     public int DaysToKeep { get; set; } = 7;
// }

// /// <summary>
// /// İşlem sonucu
// /// </summary>
// public class ResponseModel : IResponseModel
// {
//     /// <summary>
//     /// İşlenen öğrenci sayısı
//     /// </summary>
//     public int ProcessedCount { get; set; }

//     /// <summary>
//     /// İşlem zamanı
//     /// </summary>
//     public DateTime ProcessedAt { get; set; }

//     /// <summary>
//     /// Sonuç mesajı
//     /// </summary>
//     public string Message { get; set; }
// }
