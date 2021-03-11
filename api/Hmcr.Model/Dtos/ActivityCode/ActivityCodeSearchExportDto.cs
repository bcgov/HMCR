using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using Hmcr.Model.Utils;

namespace Hmcr.Model.Dtos.ActivityCode
{
    public class ActivityCodeSearchExportDto
    {
        [JsonPropertyName("id")]
        public decimal ActivityCodeId { get; set; }
        public string ActivityNumber { get; set; }
        public string ActivityName { get; set; }
        public string UnitOfMeasure { get; set; }
        public string MaintenanceType { get; set; }
        public string LocationCode { get; set; }
        public string FeatureType { get; set; }
        public string SpThresholdLevel { get; set; }
        public string ActivityApplication { get; set; }
        
        //is site required is set to Y or N in the Mapper
        public string IsSiteNumRequired { get; set; }
        public DateTime? EndDate { get; set; }
        public string IsActive => (EndDate == null || EndDate > DateTime.Today) ? "Y" : "N";
        //is referenced is handled in the ActivityCodeRepo
        public string IsReferenced { get; set; }

        //public decimal RoadLengthRule { get; set; }
        public string RoadLengthRuleName { get; set; }
        //public decimal SurfaceTypeRule { get; set; }
        public string SurfaceTypeRuleName { get; set; }
        //public decimal RoadClassRule { get; set; }
        public string RoadClassRuleName { get; set; }
        public string ServiceAreas { get; set; }

        public decimal? MinValue { get; set; }
        public decimal? MaxValue { get; set; }
        public int? ReportingFrequency { get; set; }

        public string ToCsv()
        {
            var wholeNumberFields = new string[] { Fields.Username, Fields.UserType };
            return CsvUtils.ConvertToCsv<ActivityCodeSearchExportDto>(this, wholeNumberFields);
        }
    }
}
