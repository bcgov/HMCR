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
                var folder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                return _pointWithinServiceAreaQuery ?? (_pointWithinServiceAreaQuery = File.ReadAllText(Path.Combine(folder, (@"XmlTemplates\IsPointWithinServiceArea.xml")))); 
            }
        }
    }
}
