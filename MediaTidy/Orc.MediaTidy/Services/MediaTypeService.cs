using Orc.MediaTidy.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Orc.MediaTidy.Services
{
    internal class MediaTypeService
    {
        private readonly IContentTypeService _contentTypeService;

        internal MediaTypeService()
        {
            var contentTypeService = ApplicationContext.Current.Services.ContentTypeService;
            _contentTypeService = contentTypeService ?? throw new ArgumentNullException(nameof(contentTypeService));
        }

        internal void AddArchivedFolderMediaType()
        {
            if (_contentTypeService.GetMediaType(KnownMediaTypeAliases.ArchiveFolder) == null)
            {
                MediaType mediaType = new MediaType(-1)
                {
                    AllowedAsRoot = ArchiveMediaType.AllowedAtRoot,
                    Name = ArchiveMediaType.Name,
                    Alias = ArchiveMediaType.Alias,
                    Description = ArchiveMediaType.Description,
                    IsContainer = ArchiveMediaType.IsContainer,
                    Icon = ArchiveMediaType.Icon
                };

                //Allowed child nodes
                var children = new List<ContentTypeSort>
                    {
                        new ContentTypeSort(KnownMediaTypeIds.Folder, 0),
                        new ContentTypeSort(KnownMediaTypeIds.Image, 1),
                        new ContentTypeSort(KnownMediaTypeIds.File, 2)
                    };

                mediaType.AllowedContentTypes = children;
                IDataTypeService dataTypeService = ApplicationContext.Current.Services.DataTypeService;

                //Add properties
                var name = new PropertyType(dataTypeService.GetDataTypeDefinitionById(KnownDataTypeIds.ListViewMedia), "contents");
                name.Name = "Contents";
                name.SortOrder = 0;

                _contentTypeService.Save(mediaType);
            }
        }
    }
}
