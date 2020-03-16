using Orc.MediaTidy.Constants;
using Orc.MediaTidy.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Orc.MediaTidy.Services
{
    internal class ArchiveService
    {
        private readonly DatabaseContext _dbContext;
        private readonly IMediaService _mediaService;
        private readonly MediaTypeService _mediaTypeService;

        public ArchiveService()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            var mediaService = ApplicationContext.Current.Services.MediaService;
            var mediaTypeService = new MediaTypeService();

            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
            _mediaTypeService = mediaTypeService ?? throw new ArgumentNullException(nameof(mediaTypeService));
        }

        internal bool TryDoArchive(IEnumerable<int> ids)
        {
            var success = false;

            try
            {
                var root = CheckArchiveFolderStructure();
                var imageArchive = root.Children().FirstOrDefault(x => x.Name == "Images");
                var docsArchive = root.Children().FirstOrDefault(x => x.Name == "Documents");

                var imageYears = imageArchive.Children();
                var docsYears = docsArchive.Children();

                IEnumerable<IMedia> mediaItems = _mediaService.GetByIds(ids).Where(x => !x.Path.Contains(root.Id.ToString()));

                //hub.SendMessage($"Archiving {mediaItems.Count()} media items");

                var i = 1;
                foreach (var mediaItem in mediaItems)
                {
                    // If the media item is in the recycle bin, don't move it to the archive folder
                    if (!mediaItem.Trashed)
                    {
                        var targetYear = (mediaItem.CreateDate.Year <= 2013 ? 2013 : mediaItem.CreateDate.Year).ToString();
                        _mediaService.Move(mediaItem, (mediaItem.ContentType.Alias == KnownMediaTypeAliases.Image ? imageYears : docsYears).First(x => x.Name == targetYear).Id);

                        if (i % 5 == 0)
                        {
                            //hub.SendMessage($"Still going - archived {i} media items");
                        }

                        i += 1;
                    }
                }

                success = true;
            }
            catch
            {
                return false;
            }

            success = CleanEmptyFolders();

            return success;
        }

        private bool CleanEmptyFolders()
        {
            try
            {
                var folders = _mediaService.GetMediaOfMediaType((int)KnownMediaTypeIds.Folder);
                foreach (var folder in folders)
                {
                    if (!folder.Children().Any())
                    {
                        _mediaService.Delete(folder);
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Ensure the media archive exists, if not create it - root level folders for type (img/file), then by year
        /// </summary>
        internal IMedia CheckArchiveFolderStructure()
        {
            var root = _mediaService.GetRootMedia().FirstOrDefault(x => x.ContentType.Alias == KnownMediaTypeAliases.ArchiveFolder);

            // create root if needed
            if (root == null)
            {
                // This checks to make sure the Archive Folder media type exists, and makes it if it doesn't
                _mediaTypeService.AddArchivedFolderMediaType();

                // Check to make sure the Archive Folder exists
                root = _mediaService.CreateMedia("Archive", -1, KnownMediaTypeAliases.ArchiveFolder);
                _mediaService.Save(root);
            }

            // create image/docs folders if needed
            if (!root.Children().Any())
            {
                _mediaService.Save(new[] {
                    _mediaService.CreateMedia("Images", root, KnownMediaTypeAliases.ArchiveFolder),
                    _mediaService.CreateMedia("Documents", root, KnownMediaTypeAliases.ArchiveFolder)
                });
            }

            // add years for each file type - five prior to current
            foreach (var folder in root.Children())
            {
                // first run will create year folders back to 2013
                if (!folder.Children().Any())
                {
                    for (var i = 0; i <= 6; i += 1)
                    {
                        var year = DateTime.Now.Year - i;
                        var yearFolder = _mediaService.CreateMedia(year.ToString(), folder, KnownMediaTypeAliases.ArchiveFolder);

                        _mediaService.Save(yearFolder);
                    }
                }

                // always check a folder exists for the current year
                var currentYear = folder.Children().FirstOrDefault(x => x.Name == DateTime.Now.Year.ToString());
                if (currentYear == null)
                {
                    currentYear = _mediaService.CreateMedia(DateTime.Now.Year.ToString(), folder, KnownMediaTypeAliases.ArchiveFolder);
                    _mediaService.Save(currentYear);
                }
            }

            return root;
        }

        internal List<int> GetPrimaryKeysForParentId(int id)
        {
            var primaryKeys = _dbContext.Database.Fetch<int>(@"SELECT id FROM umbracoRelation WHERE parentId = @0", id);

            return primaryKeys;
        }

        internal void DeleteMultipleFromRelationTable(List<int> ids)
        {
            foreach(var id in ids)
            {
                DeleteFromRelationTable(id);
            }
        }

        internal void DeleteFromRelationTable(int id)
        {
            _dbContext.Database.Delete<UmbracoRelation>(id);
        }
    }
}
