using AutoMapper;
using Orc.MediaTidy.Models;
using Orc.MediaTidy.Models.Reporting;
using Orc.MediaTidy.Services;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web;

namespace Orc.MediaTidy.TypeConverters
{
    public class MediaAuditRowTypeConverter : ITypeConverter<MediaAuditItem, MediaAuditRow>
    {
        private readonly IContentService _contentService = ApplicationContext.Current.Services.ContentService;
        private readonly ArchiveService _archiveService = new ArchiveService();

        public MediaAuditRow Convert(ResolutionContext context)
        {
            var row = new MediaAuditRow();
            var item = context.SourceValue as MediaAuditItem;

            if(item != null)
            {
                var umbracoPages = new List<string>();

                if (item.UsedOnPages != null && item.UsedOnPages.Any())
                {
                    foreach (var id in item.UsedOnPages)
                    {
                        var page = UmbracoContext.Current.ContentCache.GetById(id);
                        if (page != null)
                        {
                            umbracoPages.Add($"{page.Name} ({page.Id})");
                        }
                        else
                        {
                            var unpublishedPage = _contentService.GetById(id);

                            if(unpublishedPage != null)
                            {
                                umbracoPages.Add($"[UNPUBLISHED] {unpublishedPage.Name} ({unpublishedPage.Id})");
                            }
                            else
                            {
                                // TODO: Remove this particular row from the relations table? There are actually some pages that can't seem to be found but the relation exists
                                // - I assume that means there's something that exists in the database versioning or some such, even when the pages are no longer there that Nexu
                                // hooks into? -JC

                                var pks = _archiveService.GetPrimaryKeysForParentId(id);

                                _archiveService.DeleteMultipleFromRelationTable(pks);

                                return null;
                            }
                        }
                    }
                }

                return new MediaAuditRow
                {
                    Id = item.Id.ToString(),
                    Url = item.Url,
                    IsInFileSystem = item.IsInFileSystem.ToString(),
                    UsedOnPages = string.Join(", ", umbracoPages)
                };
            }

            return row;
        }
    }
}
