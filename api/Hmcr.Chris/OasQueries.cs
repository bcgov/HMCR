using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Hmcr.Chris
{
    public class OasQueries
    {
        private string _pointOnRfiSeqQuery;
        public string PointOnRfiSegQuery
        {
            get { return _pointOnRfiSeqQuery ?? (_pointOnRfiSeqQuery = File.ReadAllText(@"XmlTemplates\IsPointOnRfiSegment.xml")); }
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
