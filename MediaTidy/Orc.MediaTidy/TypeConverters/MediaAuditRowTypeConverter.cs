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
                                // If we can't find the attached page in published or unpublished content, we remove it from the relations.
                                // This primarily seems to happen when an item is in the recycle bin and it's looking for associations that may also be
                                // recycled or have been deleted in the past - JC 16/03/2020

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
