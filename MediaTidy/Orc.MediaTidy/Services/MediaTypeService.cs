using Orc.MediaTidy.Constants;
using System;
using System.Collections.Generic;
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
                    AllowedAsRoot = ArchiveFolderMediaType.AllowedAtRoot,
                    Name = ArchiveFolderMediaType.Name,
                    Alias = ArchiveFolderMediaType.Alias,
                    Description = ArchiveFolderMediaType.Description,
                    IsContainer = ArchiveFolderMediaType.IsContainer,
                    Icon = ArchiveFolderMediaType.Icon
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

                mediaType.AddPropertyGroup("Contents");

                //Add properties
                var contents = new PropertyType(dataTypeService.GetDataTypeDefinitionById(KnownDataTypeIds.ListViewMedia), "contents");
                contents.Name = "Contents";
                contents.SortOrder = 0;

                mediaType.AddPropertyType(contents, "Contents");

                _contentTypeService.Save(mediaType);
            }
        }
    }
}
