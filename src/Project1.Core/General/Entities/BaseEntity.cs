using Project1.Core.General.Interfaces;

namespace Project1.Core.General.Entities;

public class BaseEntity : IBaseEntity
{
    public long Id { get; set; }
}
