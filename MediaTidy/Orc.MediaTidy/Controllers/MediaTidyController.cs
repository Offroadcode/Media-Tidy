using Orc.MediaTidy.Services;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace Orc.MediaTidy.Controllers
{
    public class MediaTidyController : UmbracoAuthorizedApiController
    {
        private readonly AuditService _auditService = new AuditService();
        private readonly ArchiveService _archiveService = new ArchiveService();
        private readonly ReportService _reportService = new ReportService();

        [HttpGet]
        public IHttpActionResult GenerateMediaReport()
        {
            _reportService.GenerateMediaReport();
            return Json(new
            {
                status = HttpStatusCode.OK,
                data = $"/App_Plugins/MediaTidy/reports/MediaReport-{DateTime.Now.ToString("dd-MM-yyy")}.xlsx",
            }, Constants.JsonSettings.Settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetUsedMedia()
        {
            var relations = _auditService.GetUsedMediaRelations();
            var usedMedia = _auditService.GetMediaAuditByRelations(relations);

            return Json(new
            {
                status = HttpStatusCode.OK,
                count = usedMedia.Count(),
                media = usedMedia
            }, Constants.JsonSettings.Settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetUnusedMedia()
        {
            var ids = _auditService.GetUnusedMediaIds();
            var unusedMedia = _auditService.GetMediaAuditByIds(ids);

            return Json(new
            {
                status = HttpStatusCode.OK,
                count = unusedMedia.Count(),
                media = unusedMedia
            }, Constants.JsonSettings.Settings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult ArchiveUnusedMedia(string type = null)
        {
            var ids = _auditService.GetAllUnusedMediaOfType(type);

            return Json(new
            {
                status = HttpStatusCode.OK,
                data = _archiveService.TryDoArchive(ids)
            }, Constants.JsonSettings.Settings);
        }

        [HttpPost]
        public IHttpActionResult ArchiveSelectedMedia(int[] ids)
        {
            return Json(new
            {
                status = HttpStatusCode.OK,
                data = _archiveService.TryDoArchive(ids)
            }, Constants.JsonSettings.Settings);
        }
    }
}
