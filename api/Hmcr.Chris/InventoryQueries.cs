using System.IO;
using System.Reflection;

namespace Hmcr.Chris
{
    public class InventoryQueries
    {
        private string _inventoryAssocWithLineQuery;
        private string _inventoryAssocWithPointQuery;

        public string InventoryAssocWithLineQuery
        {
            get
            {
                var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "XmlTemplates");
                return _inventoryAssocWithLineQuery ?? (_inventoryAssocWithLineQuery = File.ReadAllText(Path.Combine(folder, "GetInventoryAssocWithWorkActivity.xml")));
            }
        }

        public string InventoryAssocWithPointQuery
        {
            get
            {
                var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "XmlTemplates");
                return _inventoryAssocWithPointQuery ?? (_inventoryAssocWithPointQuery = File.ReadAllText(Path.Combine(folder, "GetInventoryAssocWithWorkActivity.xml")));
            }
        }

        public readonly string StructureOnRfiSegment
            = "service=WFS&version=1.1.0&request=GetFeature&typeName=cwr:BSR_BY_RFI&srsName=EPSG:4326&outputFormat=application/json&cql_filter=RFI_UNIQUE='{0}'";

        public readonly string RestAreaOnRfiSegment
            = "service=WFS&version=1.1.0&request=GetFeature&typeName=cwr:RA_BY_RFI&srsName=EPSG:4326&outputFormat=application/json&cql_filter=RFI_UNIQUE='{0}'";
    }
}
