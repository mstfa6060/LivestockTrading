// using System.ComponentModel.DataAnnotations.Schema;
// using Common.Definitions.Base.Entity;



// namespace Common.Definitions.Domain.Entities;

// public class JobApplication : BaseEntity
// {
//     public Guid JobId { get; set; }
//     public Guid WorkerId { get; set; }
//     public ApplicationStatus Status { get; set; } // Bekliyor, Kabul Edildi vb.
//     public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

//     [ForeignKey("JobId")]
//     public Job Job { get; set; }

//     [ForeignKey("WorkerId")]
//     public User Worker { get; set; }
// }

// public enum ApplicationStatus
// {
//     Pending = 1,
//     Accepted = 2,
//     Rejected = 3
// }
