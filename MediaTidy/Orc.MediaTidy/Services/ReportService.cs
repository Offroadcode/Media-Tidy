using OfficeOpenXml;
using System;
using System.IO;
using System.Web;

namespace Orc.MediaTidy.Services
{
    internal class ReportService
    {
        private readonly AuditService _auditService = new AuditService();

        internal void GenerateMediaReport()
        {
            var folder = HttpContext.Current.Server.MapPath($"/App_Plugins/MediaTidy/reports/");

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var fileInfo = new FileInfo(HttpContext.Current.Server.MapPath($"/App_Plugins/MediaTidy/reports/MediaReport_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx"));

            var relations = _auditService.GetUsedMediaRelations();
            var usedMedia = _auditService.GetMediaAuditByRelations(relations);

            using (var excelPackage = new ExcelPackage(fileInfo))
            {
                var ws = excelPackage.Workbook.Worksheets.Add("Used Media");

                if (usedMedia != null)
                {
                    ws.Cells.LoadFromCollection(usedMedia);
                }

                excelPackage.Save();
            }
        }
    }
}
