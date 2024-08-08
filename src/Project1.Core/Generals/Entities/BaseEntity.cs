using Project1.Core.Generals.Interfaces;

namespace Project1.Core.Generals.Entities;

public class BaseEntity : IBaseEntity
{
    public long Id { get; set; }
}
