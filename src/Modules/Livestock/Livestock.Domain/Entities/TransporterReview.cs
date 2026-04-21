namespace Livestock.Domain.Entities;

public class TransporterReview : BaseEntity
{
    public Guid TransporterId { get; set; }
    public Guid ReviewerUserId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public Guid? TransportRequestId { get; set; }

    public Transporter Transporter { get; set; } = null!;
}
