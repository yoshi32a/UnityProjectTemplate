using System.Collections.Generic;

namespace Master.Editor
{
    public class GeneratedTableInfo
    {
        public string TableName { get; set; }
        public string ClassName { get; set; }
        public List<GeneratedPropertyInfo> Properties { get; set; }
        public bool HasCompositeKey { get; set; }
        public bool UsesFloat3 { get; set; }
    }
}