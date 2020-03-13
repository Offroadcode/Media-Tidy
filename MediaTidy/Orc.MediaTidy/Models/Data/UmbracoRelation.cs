using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Orc.MediaTidy.Models.Data
{
    [TableName("umbracoRelation")]
    [PrimaryKey("Id", autoIncrement = true)]
    internal class UmbracoRelation
    {
        [Column("id")]
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int Id { get; set; }

        [Column("parentId")]
        public int ParentId { get; set; }

        [Column("childId")]
        public int ChildId { get; set; }

        [Column("datetime")]
        public DateTime DateTime { get; set; }

        [Column("comment")]
        public string Comment { get; set; }
    }
}
