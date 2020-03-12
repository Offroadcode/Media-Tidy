using AutoMapper;
using Orc.MediaTidy.Models;
using Orc.MediaTidy.Models.Reporting;
using Orc.MediaTidy.TypeConverters;
using Umbraco.Core;
using Umbraco.Core.Models.Mapping;

namespace Orc.MediaTidy.Events
{
    public class AutoMapperConfiguration : MapperConfiguration
    {
        public override void ConfigureMappings(IConfiguration config, ApplicationContext applicationContext)
        {
            config.CreateMap<MediaAuditItem, MediaAuditRow>().ConvertUsing<MediaAuditRowTypeConverter>();
        }
    }
}
