namespace Domain.Core.Common;

public abstract class EntityBase : IEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public abstract void Validate();
}
