using System.ComponentModel.DataAnnotations;

namespace Project1.Core.Generals.Interfaces;

public interface IBaseEntity
{
    [Key]
    public long Id { get; set; }
}
