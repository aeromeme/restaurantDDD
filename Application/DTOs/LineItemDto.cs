using Domain.Entities;
using Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class LineItemDto : LineItemDtoBase
    {
        public Guid LineItemId { get;  set; }

        public ProductDto Product { get; set; } = null!;

        public Money Price { get;  set; } = null!;
    }
    public abstract class LineItemDtoBase
    {
      
        public int Quantity { get;  set; }
    }
    public class LineItemCreateDto : LineItemDtoBase
    {
        public Guid ProductId { get; set; }

    }
    public class LineItemUpdateDto : LineItemDtoBase
    {
        public Guid LineItemId { get; set; }

        public Guid ProductId { get; set; }
    }
}
