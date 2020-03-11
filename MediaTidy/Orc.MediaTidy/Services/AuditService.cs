using Orc.MediaTidy.Constants;
using Orc.MediaTidy.Models;
using Orc.MediaTidy.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using SystemFile = System.IO.File;

namespace Orc.MediaTidy.Services
{
    public class AuditService
    {
        private readonly DatabaseContext _dbContext;
        private readonly IMediaService _mediaService;

        public AuditService()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            var mediaService = ApplicationContext.Current.Services.MediaService;

            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
        }

        internal IList<MediaAuditItem> GetMediaAuditByRelations(IEnumerable<MediaRelation> relations)
        {
            var mediaAudit = new List<MediaAuditItem>();
            var groupedRelations = relations.GroupBy(x => x.NodeId).ToDictionary(x => x.Key, x => x.ToList());

            var media = _mediaService.GetByIds(groupedRelations.Select(x => x.Key));

            if(media != null)
            {
                foreach (var item in media)
                {
                    var url = item.GetUrl("umbracoFile", null);
                    mediaAudit.Add(new MediaAuditItem
                    {
                        Id = item.Id,
                        Url = url,
                        IsInFileSystem = IsInFileSystem(url),
                        UsedOnPages = GetUsedPagesByMediaItemId(groupedRelations, item.Id)
                    });
                }
            }

            return mediaAudit;
        }

        internal IList<int> GetUsedPagesByMediaItemId(Dictionary<int, List<MediaRelation>> mediaRelations, int id)
        {
            var relations = mediaRelations[id];

            if(relations != null)
            {
                var ids = relations.Select(x => x.ParentId);
                return ids.Where(x => x != id).Distinct().ToList();
            }

            return null;
        }

        internal IList<MediaAuditItem> GetMediaAuditByIds(IEnumerable<int> ids)
        {
            var mediaAudit = new List<MediaAuditItem>();
            var media = _mediaService.GetByIds(ids);

            if (media != null)
            {
                foreach (var item in media)
                {
                    var url = item.GetUrl("umbracoFile", null);
                    mediaAudit.Add(new MediaAuditItem
                    {
                        Id = item.Id,
                        Url = url,
                        IsInFileSystem = IsInFileSystem(url),
                        UsedOnPages = null
                    });
                }
            }

            return mediaAudit;
        }

        internal bool IsInFileSystem(string path)
        {
            var serverPath = HttpContext.Current.Server.MapPath(path);

            return SystemFile.Exists(serverPath);
        }

        internal IEnumerable<MediaRelation> GetUsedMediaRelations()
        {
            return _dbContext.Database.Fetch<MediaRelation>(SqlQueries.MediaQuery);
        }

        internal IEnumerable<int> GetUnusedMediaIds()
        {
            return _dbContext.Database.Fetch<int>(SqlQueries.UnusedMediaQuery);
        }

        internal IEnumerable<int> GetAllUnusedMediaOfType(string type)
        {
            return _dbContext.Database.Fetch<int>(type == "images" ? SqlQueries.UnusedImgMediaQuery : type == "documents" ? SqlQueries.UnusedDocsMediaQuery : SqlQueries.UnusedMediaQuery);
        }
    }
}
