using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Moe.Core.Models.Entities;

public class City : BaseEntity
{
    #region One-To-N
    [DeleteBehavior(DeleteBehavior.Cascade)]
    public Country Country { get; set; }
    public Guid CountryId { get; set; }
    #endregion

    #region Non-Functional
    [DeleteBehavior(DeleteBehavior.SetNull)]
    public LocalizedContent? Name { get; set; }
    public Guid? NameId { get; set; }
    #endregion
}