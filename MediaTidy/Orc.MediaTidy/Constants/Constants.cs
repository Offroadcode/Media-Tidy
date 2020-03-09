using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace Orc.MediaTidy.Constants
{
    public class KnownMediaTypeAliases
    {
        public const string Image = "Image";
        public const string ArchiveFolder = "archiveFolder";
    }

    public class KnownMediaTypeIds
    {
        public const int Folder = 1031;
    }

    internal class SqlQueries
    {
        internal const string ImgMediaQuery =
            @"SELECT nodeId from cmsMedia
                WHERE cmsMedia.nodeId not in (select distinct parentId from umbracoRelation where parentId is not null)
                AND cmsMedia.nodeId not in (select distinct childId from umbracoRelation where childId is not null)
                AND (cmsMedia.mediaPath like '%.png' OR cmsMedia.mediaPath like '%.jpg')";

        internal const string DocsMediaQuery =
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
