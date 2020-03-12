using AutoMapper;
using OfficeOpenXml;
using Orc.MediaTidy.Models;
using Orc.MediaTidy.Models.Reporting;
using System;
using System.Collections.Generic;
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

            var filePath = HttpContext.Current.Server.MapPath($"/App_Plugins/MediaTidy/reports/MediaReport_{DateTime.Now.ToString("dd-MM-yyyy")}.xlsx");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var fileInfo = new FileInfo(filePath);

            using (var excelPackage = new ExcelPackage(fileInfo))
            {
                AddUsedMediaToExcelPackage(excelPackage);
                AddArchivedMediaToExcelPackage(excelPackage);

                excelPackage.Save();
            }
        }

        internal void AddUsedMediaToExcelPackage(ExcelPackage excelPackage)
        {
            var relations = _auditService.GetUsedMediaRelations();
            var usedMedia = _auditService.GetMediaAuditByRelations(relations);

            if (usedMedia != null)
            {
                var ws = excelPackage.Workbook.Worksheets.Add("Used Media");
                var usedMediaRows = new List<MediaAuditRow>();

                // Add the headline
                usedMediaRows.Add(new MediaAuditRow
                {
                    Id = "Media Id",
                    Url = "Url",
                    IsInFileSystem = "On File System",
                    UsedOnPages = "Used on Pages"
                });

                usedMediaRows.AddRange(Mapper.Map<List<MediaAuditRow>>(usedMedia));

                ws.Cells.LoadFromCollection(usedMediaRows);

                // Set the first row to bold
                ws.Cells[1, 1, 1, 4].Style.Font.Bold = true;
                ws.Cells.AutoFitColumns();
            }
        }

        internal void AddArchivedMediaToExcelPackage(ExcelPackage excelPackage)
        {
            var ids = _auditService.GetUnusedMediaIds();
            var unusedMedia = _auditService.GetMediaAuditByIds(ids);

            if (unusedMedia != null)
            {
                var ws = excelPackage.Workbook.Worksheets.Add("Unused (Archived) Media");
                var usedMediaRows = new List<MediaAuditRow>();

                // Add the headline
                usedMediaRows.Add(new MediaAuditRow
                {
                    Id = "Id",
                    Url = "Path",
                    IsInFileSystem = "On File System"
                });

                usedMediaRows.AddRange(Mapper.Map<List<MediaAuditRow>>(unusedMedia));

                ws.Cells.LoadFromCollection(usedMediaRows);

                // Set the first row to bold
                ws.Cells[1, 1, 1, 4].Style.Font.Bold = true;
                ws.Cells.AutoFitColumns();
            }
        }
    }
}
