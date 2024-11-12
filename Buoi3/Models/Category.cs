using System;
using System.Collections.Generic;

namespace Buoi3.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime? CreatedDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
