using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Expenses.API.Models.Base;

/// <summary>
/// Class representing a base entity with common properties.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// The unique identifier for the entity. This property is typically used as the primary key in a database and
    /// should be assigned a unique value when a new entity is created. 
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// The date and time when the entity was created. This property is automatically set when a new entity is
    /// created and should not be modified afterward.
    /// </summary>
    [Column("Date of Creation")]
    [Display(Name = "Date of Creation"), DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTimeOffset CreatedAt { get; set; }
    
    /// <summary>
    /// The date and time when the entity was last updated. This property should be updated whenever the entity is
    /// modified. It helps to track changes and maintain a history of updates for the entity.
    /// </summary>
    [Column("Latest Update")]
    [Display(Name = "Latest Update"), DataType(DataType.DateTime)]
    [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
    public DateTimeOffset UpdatedAt { get; set; }
}