using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Arch.CMessaging.Client.MetaEntity.Entity;

namespace Arch.CMessaging.Client.Core.Bo
{
    public class SchemaView
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public int Version { get; set; }
        public DateTime CreateTime { get; set; }
        public string Compatibility { get; set; }
        public string Description { get; set; }
        public List<Property> Properties { get; set; }
        public string SchemaPreview { get; set; }
        public long TopicID { get; set; }

    }
}
