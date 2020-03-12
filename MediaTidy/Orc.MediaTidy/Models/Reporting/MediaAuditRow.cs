namespace Orc.MediaTidy.Models.Reporting
{
    public class MediaAuditRow
    {
        public string Id { get; set; }

        public string Url { get; set; }

        public string IsInFileSystem { get; set; }

        public string UsedOnPages { get; set; }
    }
}
