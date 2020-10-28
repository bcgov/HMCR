﻿using Hmcr.Model.Dtos.ActivityCode;
using System;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkReportCsvDto : IReportCsvDto
    {
        public decimal RowId { get; set; }
        public string RecordType { get; set; }
        public string ServiceArea { get; set; }
        public string RecordNumber { get; set; }
        public string TaskNumber { get; set; }
        public string ActivityNumber { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Accomplishment { get; set; }
        public string UnitOfMeasure { get; set; }
        public string PostedDate { get; set; }
        public string HighwayUnique { get; set; }
        public string Landmark { get; set; }
        public string StartOffset { get; set; }
        public string EndOffset { get; set; }
        public string StartLatitude { get; set; }
        public string StartLongitude { get; set; }
        public string EndLatitude { get; set; }
        public string EndLongitude { get; set; }
        public string StructureNumber { get; set; }
        public string SiteNumber { get; set; }
        public string ValueOfWork { get; set; }
        public string Comments { get; set; }

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

        public ActivityCodeValidationDto ActivityCodeValidation { get; set; }

    }
}
