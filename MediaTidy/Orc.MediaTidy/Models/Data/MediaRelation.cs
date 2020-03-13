using Umbraco.Core.Persistence;

namespace Orc.MediaTidy.Models.Data
{
    public class MediaRelation
    {
        public int NodeId { get; set; }

        public int ChildId { get; set; }

        public int ParentId { get; set; }
    }
}
