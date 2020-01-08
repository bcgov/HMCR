﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Model.Dtos.WorkReport
{
    public class WorkReportDto
    {
        public int RowNumber { get; set; }
        public decimal WorkReportId { get; set; }
        public decimal SubmissionObjectId { get; set; }
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
        public string Landmark { get; set; }
        public decimal? StartOffset { get; set; }
        public decimal? EndOffset { get; set; }
        public decimal? StartLatitude { get; set; }
        public decimal? StartLongitude { get; set; }
        public decimal? EndLatitude { get; set; }
        public decimal? EndLongitude { get; set; }
        public string StructureNumber { get; set; }
        public string SiteNumber { get; set; }
        public decimal? ValueOfWork { get; set; }
        public string Comments { get; set; }
    }
}
