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
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult DeleteAll(string type)
        {
            var items = _mediaAuditService.GetAllMediaOfType(type);

            return Json(new
            {
                status = HttpStatusCode.OK,
                data = _mediaAuditService.TryDoDelete(items)
            });
        }

        [HttpPost]
        public IHttpActionResult DeleteSelected(int[] ids)
        {
            return Json(new
            {
                status = HttpStatusCode.OK,
                data = _mediaAuditService.TryDoDelete(ids)
            });
        }
    }
}
