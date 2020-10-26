using System.IO;
using System.Reflection;

namespace Hmcr.Chris
{
    public class InventoryQueries
    {
        private string _surfaceTypeAssocWithLineQuery;
        private string _surfaceTypeAssocWithPointQuery;

        public string SurfaceTypeAssocWithLineQuery
        {
            get
            {
                var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "XmlTemplates");
                return _surfaceTypeAssocWithLineQuery ?? (_surfaceTypeAssocWithLineQuery = File.ReadAllText(Path.Combine(folder, "GetInventoryAssocWithWorkActivity.xml")));
            }
        }

        public string SurfaceTypeAssocWithPointQuery
        {
            get
            {
                var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "XmlTemplates");
                return _surfaceTypeAssocWithPointQuery ?? (_surfaceTypeAssocWithPointQuery = File.ReadAllText(Path.Combine(folder, "GetInventoryAssocWithWorkActivity.xml")));
            }
        }

    }
}
