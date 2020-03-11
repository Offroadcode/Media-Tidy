using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Orc.MediaTidy.Constants
{
    public class KnownMediaTypeAliases
    {
        public const string Folder = "Folder";
        public const string File = "File";
        public const string Image = "Image";
        public const string ArchiveFolder = "archiveFolder";
    }

    public class KnownDataTypeIds
    {
        // This is the default media list view that gets installed with Umbraco and is added to the
        // Archive Folder media type when we create it
        public const int ListViewMedia = -96;
    }

    public class ArchiveFolderMediaType
    {
        public const string Alias = KnownMediaTypeAliases.ArchiveFolder;
        public const string Name = "Archive Folder";
        public const string Icon = "icon-folder-closed color-blue-grey";
        public const string Description = "A folder to hold archived media for the purposes of testing with Nexu";
        public const bool IsContainer = false;
        public const bool AllowedAtRoot = true;
    }

    public class KnownMediaTypeIds
    {
        // These three constants are the default folder/image/file Ids that are set when Umbraco installs
        // They should always remain the same unless for some reason someone has deleted and/or remade one of these media types
        public const int Folder = 1031;
        public const int Image = 1032;
        public const int File = 1033;
    }

    internal class SqlQueries
    {
        internal const string ImgMediaQuery =
            @"SELECT nodeId from cmsMedia
                WHERE cmsMedia.nodeId in (select distinct parentId from umbracoRelation where parentId is not null)
                AND cmsMedia.nodeId in (select distinct childId from umbracoRelation where childId is not null)
                AND (cmsMedia.mediaPath like '%.png' OR cmsMedia.mediaPath like '%.jpg')";

        internal const string UnusedImgMediaQuery =
            @"SELECT nodeId from cmsMedia
                WHERE cmsMedia.nodeId not in (select distinct parentId from umbracoRelation where parentId is not null)
                AND cmsMedia.nodeId not in (select distinct childId from umbracoRelation where childId is not null)
                AND (cmsMedia.mediaPath like '%.png' OR cmsMedia.mediaPath like '%.jpg')";

        internal const string DocsMediaQuery =
            @"SELECT nodeId from cmsMedia
                WHERE cmsMedia.nodeId in (select distinct parentId from umbracoRelation where parentId is not null)
                AND cmsMedia.nodeId in (select distinct childId from umbracoRelation where childId is not null)
                AND (
                    cmsMedia.mediaPath not like '%.png' 
                    AND cmsMedia.mediaPath not like '%.jpg'
                    AND cmsMedia.mediaPath not like '%.css'  
                    AND cmsMedia.mediaPath not like '%.js')";

        internal const string UnusedDocsMediaQuery =
            @"SELECT nodeId from cmsMedia
                WHERE cmsMedia.nodeId not in (select distinct parentId from umbracoRelation where parentId is not null)
                AND cmsMedia.nodeId not in (select distinct childId from umbracoRelation where childId is not null)
                AND (
                    cmsMedia.mediaPath not like '%.png' 
                    AND cmsMedia.mediaPath not like '%.jpg'
                    AND cmsMedia.mediaPath not like '%.css'  
                    AND cmsMedia.mediaPath not like '%.js')";

        // bit clunky, but should ignore non-standard stuff like css, js, svg etc
        // these are used in canvas pages, so while archiving won't break anything, should leave them alone

        internal const string MediaQuery =
            @"SELECT nodeId, mediaPath, parentId, childId
                  FROM umbracoRelation
                  JOIN cmsMedia ON nodeId = parentId OR nodeId = childId
                  WHERE parentId in (SELECT distinct nodeId from cmsMedia where nodeId is not null)
                  OR childId in (select distinct nodeId from cmsMedia where nodeId is not null)
            AND (
                cmsMedia.mediaPath not like '%.css'  
                AND cmsMedia.mediaPath not like '%.js')";

        internal const string UnusedMediaQuery =
            @"SELECT nodeId from cmsMedia
                WHERE cmsMedia.nodeId not in (select distinct parentId from umbracoRelation where parentId is not null)
                AND cmsMedia.nodeId not in (select distinct childId from umbracoRelation where childId is not null)
                AND (
                    cmsMedia.mediaPath not like '%.css'  
                    AND cmsMedia.mediaPath not like '%.js')";
    }

    internal class JsonSettings
    {
        internal static JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        internal static JsonSerializerSettings SettingsWithJsDates = new JsonSerializerSettings()
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
            Converters = new List<JsonConverter>() { new JavaScriptDateTimeConverter() }
        };
    }
}
