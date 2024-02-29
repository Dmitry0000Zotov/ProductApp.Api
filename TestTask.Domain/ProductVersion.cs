using System;
using System.Collections.Generic;

namespace TestTask.Domain;

public partial class ProductVersion
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatingDate { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public decimal Length { get; set; }

    public virtual Product Product { get; set; } = null!;
}
