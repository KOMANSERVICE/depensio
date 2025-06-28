
namespace depensio.Domain.Abstractions;

internal interface IEntity<T> : IEntity
{
    public T Id { get; set; }
}

internal interface IEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string CreatedBy { get; set; }
    public string UpdatedBy { get; set; }
}
