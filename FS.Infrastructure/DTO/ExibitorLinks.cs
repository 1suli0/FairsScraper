using System.Collections.Concurrent;

namespace FS.Infrastructure.DTO
{
    public class ExibitorLinks
    {
        public BlockingCollection<string> Links { get; set; }
    }
}