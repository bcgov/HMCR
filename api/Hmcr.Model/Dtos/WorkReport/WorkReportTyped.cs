using Hmcr.Model.Dtos.ActivityCode;
using System;
using System.Collections.Generic;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkReportTyped
    {
        public decimal WorkReportId { get; set; }
        public decimal SubmissionObjectId { get; set; }
        public decimal RowId { get; set; }
        public decimal? ValidationStatusId { get; set; }
        public string RecordType { get; set; }
        public decimal ServiceArea { get; set; }
        public string RecordNumber { get; set; }
        public string TaskNumber { get; set; }
        public string ActivityNumber { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? Accomplishment { get; set; }
        public string UnitOfMeasure { get; set; }
        public DateTime? PostedDate { get; set; }
        public string HighwayUnique { get; set; }
        public string HighwayUniqueName { get; set; }
        public decimal? HighwayUniqueLength { get; set; }
        public string Landmark { get; set; }
        public decimal? StartOffset { get; set; }
        public decimal? EndOffset { get; set; }
        public decimal? StartLatitude { get; set; }
        public decimal? StartLongitude { get; set; }
        public decimal? EndLatitude { get; set; }
        public decimal? EndLongitude { get; set; }
        public decimal? WorkLength { get; set; }
        public string StructureNumber { get; set; }
        public string SiteNumber { get; set; }
        public decimal? ValueOfWork { get; set; }
        public string Comments { get; set; }
        //public Geometry Geometry { get; set; }

        /// <summary>
        /// Line number of the record in the CSV file
        /// </summary>
        public decimal? RowNum { get; set; }

        /// <summary>
        /// Feature type derived from activity code
        /// </summary>
        public string FeatureType { get; set; }

        /// <summary>
        /// D2, D3, D4 report type
        /// </summary>
        public SpatialData SpatialData { get; set; }
        public string SpThresholdLevel { get; set; }

        /// <summary>
        /// Road feature data retrieved from CHRIS
        /// </summary>
        public virtual IList<WorkReportSurfaceType> SurfaceTypes { get; set; }
        public virtual IList<WorkReportMaintenanceClass> MaintenanceClasses { get; set; }
        public virtual IList<WorkReportHighwayProfile> HighwayProfiles { get; set; }
        public virtual IList<WorkReportGuardrail> Guardrails { get; set; }

        public ActivityCodeValidationDto ActivityCodeValidation { get; set; }
    }
}
