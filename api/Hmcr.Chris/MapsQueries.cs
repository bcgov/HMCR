using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hmcr.Chris
{
    public class MapsQueries
    {
        private string _pointWithinServiceAreaQuery;
        public string PointWithinServiceAreaQuery
        {
            get { return _pointWithinServiceAreaQuery ?? (_pointWithinServiceAreaQuery = File.ReadAllText(@"XmlTemplates\IsPointWithinServiceArea.xml")); }
        }
    }
}
