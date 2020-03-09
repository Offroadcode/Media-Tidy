using Orc.MediaTidy.Constants;
using Orc.MediaTidy.Models;
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
    public class MediaAuditService
    {
        private readonly DatabaseContext _dbContext;
        private readonly IMediaService _mediaService;

        public MediaAuditService()
        {
            var dbContext = ApplicationContext.Current.DatabaseContext;
            var mediaService = ApplicationContext.Current.Services.MediaService;

            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _mediaService = mediaService ?? throw new ArgumentNullException(nameof(mediaService));
        }

        internal IList<MediaAuditItem> GetUnusedMediaData(IEnumerable<int> ids)
        {
            var unusedMedia = new List<MediaAuditItem>();
            var media = _mediaService.GetByIds(ids);

            if (media != null)
            {
                foreach (var item in media)
                {
                    var url = item.GetUrl("umbracoFile", null);
                    unusedMedia.Add(new MediaAuditItem
                    {
                        Id = item.Id,
                        Url = url,
                        IsInFileSystem = IsInFileSystem(url),
                        UsedOnPages = null
                    });
                }
            }

            return unusedMedia;
        }

        internal bool IsInFileSystem(string path)
        {
            var serverPath = HttpContext.Current.Server.MapPath(path);

            return SystemFile.Exists(serverPath);
        }

        internal IEnumerable<int> GetUnusedMediaIds()
        {
            return _dbContext.Database.Fetch<int>(SqlQueries.MediaQuery);
        }

        internal IEnumerable<int> GetAllMediaOfType(string type)
        {
            return _dbContext.Database.Fetch<int>(type == "images" ? SqlQueries.ImgMediaQuery : type == "documents" ? SqlQueries.DocsMediaQuery : SqlQueries.MediaQuery);
        }

        internal bool TryDoDelete(IEnumerable<int> ids)
        {
            var success = false;

            //var hub = new NotificationsHub();
            //hub.SendMessage("Fetching media to archive");

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
                    var targetYear = (mediaItem.CreateDate.Year <= 2013 ? 2013 : mediaItem.CreateDate.Year).ToString();
                    _mediaService.Move(mediaItem, (mediaItem.ContentType.Alias == KnownMediaTypeAliases.Image ? imageYears : docsYears).First(x => x.Name == targetYear).Id);

                    if (i % 5 == 0)
                    {
                        //hub.SendMessage($"Still going - archived {i} media items");
                    }

                    i += 1;
                }

                success = true;
            }
            catch
            {
                return false;
            }

            success = CleanEmptyFolders();

            //hub.SendMessage("Media archiving complete");

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
    }
}
