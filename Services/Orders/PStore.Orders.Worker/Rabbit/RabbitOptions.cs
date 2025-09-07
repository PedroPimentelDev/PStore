using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PStore.Orders.Worker.Rabbit
{
    public sealed class RabbitOptions
    {
        public string Host { get; }
        public int Port { get; }
        public string User { get; }
        public string Pass { get; }
        public string Exchange { get; }
        public string Queue { get; }
        public string[] Bindings { get; }

        public RabbitOptions(IConfiguration cfg)
        {
            Host = cfg["Host"]!;
            Port = int.Parse(cfg["Port"]!);
            User = cfg["User"]!;
            Pass = cfg["Pass"]!;
            Exchange = cfg["Exchange"]!;
            Queue = cfg["Queue"]!;
            Bindings = cfg.GetSection("Bindings").Get<string[]>() ?? Array.Empty<string>();
        }
    }
}
