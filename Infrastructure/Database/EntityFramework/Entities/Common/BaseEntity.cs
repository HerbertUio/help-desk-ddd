namespace Infrastructure.Database.EntityFramework.Entities.Common;

public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }
    public DateTime LastModifiedAt { get; set; }
    public int LastModifiedBy { get; set; }
}