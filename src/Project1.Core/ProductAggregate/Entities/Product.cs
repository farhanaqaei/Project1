using Project1.Core.General.Entities;
using Project1.Core.General.Interfaces;

namespace Project1.Core.ProductAggregate.Entities;

public class Product: BaseEntity
{
    public string Name { get; set; }
}
