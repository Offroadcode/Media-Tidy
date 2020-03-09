using System.Collections.Generic;

namespace Orc.MediaTidy.Models
{
    internal class MediaAuditItem
    {
        public int Id { get; set; }

        public string Url { get; set; }

        public bool IsInFileSystem { get; set; }

        public IEnumerable<int> UsedOnPages { get; set; }
    }
}
