using Orc.MediaTidy.Services;
using System.Linq;
using System.Net;
using System.Web.Http;
using Umbraco.Web.WebApi;

namespace Orc.MediaTidy.Controllers
{
    public class MediaTidyController : UmbracoApiController
    {
        private readonly MediaAuditService _mediaAuditService = new MediaAuditService();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult GetUnusedMedia()
        {
            var ids = _mediaAuditService.GetUnusedMediaIds();
            var unusedMedia = _mediaAuditService.GetUnusedMediaData(ids);

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
        public IHttpActionResult DeleteAllUnusedMedia(string type = null)
        {
            var items = _mediaAuditService.GetAllUnusedMediaOfType(type);

            return Json(new
            {
                status = HttpStatusCode.OK,
                data = _mediaAuditService.TryDoDelete(items)
            }, Constants.JsonSettings.Settings);
        }

        [HttpPost]
        public IHttpActionResult DeleteSelectedMedia(int[] ids)
        {
            return Json(new
            {
                status = HttpStatusCode.OK,
                data = _mediaAuditService.TryDoDelete(ids)
            }, Constants.JsonSettings.Settings);
        }
    }
}
