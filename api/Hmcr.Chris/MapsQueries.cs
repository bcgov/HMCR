using System.IO;
using System.Reflection;

namespace Hmcr.Chris
{
    public class MapsQueries
    {
        private string _pointWithinServiceAreaQuery;
        public string PointWithinServiceAreaQuery
        {
            get {
                var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "XmlTemplates");
                return _pointWithinServiceAreaQuery ?? (_pointWithinServiceAreaQuery = File.ReadAllText(Path.Combine(folder, "IsPointWithinServiceArea.xml"))); 
            }
        }
    }
}
