using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public abstract class OrderDtoBase
    {
        public DateTime OrderDate { get; set; }
    }

    public class OrderDto : OrderDtoBase
    {
        public Guid OrderId { get; set; }

        public CustomerDto Customer { get; set; }=null!;

        public List<LineItemDto> LineItems { get; set; } = new List<LineItemDto>();

        public string Status { get; set; } = null!;
    }
    public class OrderCreateDto : OrderDtoBase
    {
        public Guid CustomerId { get; set; }
        public List<LineItemCreateDto> LineItems { get; set; } = new List<LineItemCreateDto>();
    }
    public class OrderUpdateDto : OrderDtoBase
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public List<LineItemUpdateDto> LineItems { get; set; } = new List<LineItemUpdateDto>();
    }

}
