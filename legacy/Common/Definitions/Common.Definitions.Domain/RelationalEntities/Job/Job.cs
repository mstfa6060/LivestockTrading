// using System.ComponentModel.DataAnnotations.Schema;
// using Common.Definitions.Base.Entity;



// namespace Common.Definitions.Domain.Entities;

// public class Job : BaseEntity
// {
//     public string Title { get; set; } // Örn: "Garson Aranıyor"
//     public string Description { get; set; }
//     public decimal Salary { get; set; }
//     public JobType Type { get; set; }  // Tam zamanlı, yarı zamanlı vb.
//     public Guid EmployerId { get; set; } // İş ilanı açan işverenin ID'si

//     [ForeignKey("EmployerId")]
//     public User Employer { get; set; }

//     public JobStatus Status { get; set; } // Aktif, Pasif, Dolduruldu vb.
// }


// public enum JobType
// {
//     FullTime = 1,
//     PartTime = 2,
//     Freelance = 3
// }

// public enum JobStatus
// {
//     Active = 1,
//     Closed = 2,
//     Filled = 3
// }
