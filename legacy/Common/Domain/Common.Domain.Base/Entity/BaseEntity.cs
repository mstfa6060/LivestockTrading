using Common.Definitions.Base.Entity;

namespace Common.Definitions.Base.Entity;

public abstract class BaseEntity : CoreEntity
{
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public void SetCreatedAt(DateTime createdAt)
    {
        this.CreatedAt = createdAt;
    }
}
    