using ClosedXML.Excel;
using CsvHelper;
using CsvHelper.Excel;
using System;
using System.Web;

namespace Orc.MediaTidy.Services
{
    internal class ReportService
    {
        private readonly AuditService _auditService = new AuditService();

        internal void GenerateMediaReport()
        {
            using (var workbook = new XLWorkbook(XLEventTracking.Disabled))
            {
                // Get used media for the report
                var relations = _auditService.GetUsedMediaRelations();
                var usedMedia = _auditService.GetMediaAuditByRelations(relations);
                
                var usedMediaWorksheet = workbook.AddWorksheet("Used Media");

                using (var writer = new CsvWriter(new ExcelSerializer(usedMediaWorksheet)))
                {
                    writer.WriteRecords(usedMedia);
                }

                // Get unused media for the report
                //var ids = _auditService.GetUnusedMediaIds();
                //var unusedMedia = _auditService.GetMediaAuditByIds(ids);

                var path = HttpContext.Current.Server.MapPath($"/App_Plugins/MediaTidy/reports/MediaReport_{DateTime.Now.ToString("dd-mm-yyyy")}");
                workbook.SaveAs(path);
            }
        }
    }
}
