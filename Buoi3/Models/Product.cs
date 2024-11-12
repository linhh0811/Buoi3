using System;
using System.Collections.Generic;

namespace Buoi3.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }

    public int? Quantity { get; set; }

    public int? CategoryId { get; set; }

    public string? Brand { get; set; }

    public string? Model { get; set; }

    public DateOnly? ManufactureDate { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public double? Rating { get; set; }

    public bool? IsAvailable { get; set; }

    public DateTime? CreatedDate { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Category? Category { get; set; }
}
