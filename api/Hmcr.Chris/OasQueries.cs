using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Hmcr.Chris
{
    public class OasQueries
    {
        private string _pointOnRfiSeqQuery;

        public string PointOnRfiSegQuery
        {
            get
            {
                if (_pointOnRfiSeqQuery == null)
                {
                    var folder = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "XmlTemplates");
                    var templatePath = Path.Combine(folder, "IsPointOnRfiSegment.xml");

                    try
                    {
                        var xmlTemplate = File.ReadAllText(templatePath, Encoding.UTF8);

                        xmlTemplate = xmlTemplate.Replace("\0", "");

                        _pointOnRfiSeqQuery = xmlTemplate;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Failed to process the XML template at '{templatePath}': {ex.Message}", ex);
                    }
                }

                return _pointOnRfiSeqQuery;
            }
        }

        public readonly string LineFromOffsetMeasureOnRfiSeg
            = "service=WFS&version=1.1.0&request=GetFeature&typeName=cwr:RFI_LINE_FROM_MEASURES&srsName=EPSG:4326&outputFormat=application/json&viewParams=ne_unique:{0};measure_start:{1};measure_end:{2}";

        public readonly string OffsetMeasureFromPointAndRfiSeg
            = "service=WFS&version=1.1.0&request=GetFeature&typeName=cwr%3ARFI_MEASURE_FROM_POINT&srsName=EPSG:4326&outputFormat=application%2Fjson&viewParams=longitude:{0};latitude:{1};ne_unique:{2}";

        public readonly string PointFromOffsetMeasureOnRfiSeg
            = "service=WFS&version=1.1.0&request=GetFeature&typeName=cwr%3ARFI_POINT_FROM_MEASURE&srsName=EPSG:4326&outputFormat=application%2Fjson&viewParams=ne_unique:{0};measure:{1}";

        public readonly string RfiSegmentDetail
            = "service=WFS&version=1.1.0&request=GetFeature&typeName=cwr:V_NM_NLT_RFI_GRFI_SDO_DT&srsName=EPSG:4326&outputFormat=application/json&cql_filter=NE_UNIQUE='{0}'";

    }
}
