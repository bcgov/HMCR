﻿using System;

namespace Hmcr.Model.Dtos.ActivityCode
{
    public class ActivityCodeCreateDto
    {
        public string ActivityNumber { get; set; }
        public string ActivityName { get; set; }
        public string UnitOfMeasure { get; set; }
        public string MaintenanceType { get; set; }
        public decimal LocationCodeId { get; set; }
        public string FeatureType { get; set; }
        public bool IsSiteNumRequired { get; set; }
        public DateTime? EndDate { get; set; }
    }
}