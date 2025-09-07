using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Orders.Domain {
    public class OutboxMessage {
        public Guid Id { get; set; }
        public DateTime OccurredOnUtc { get; set; }
        public string Type { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime? ProcessedOnUtc { get; set; }
    }
}
