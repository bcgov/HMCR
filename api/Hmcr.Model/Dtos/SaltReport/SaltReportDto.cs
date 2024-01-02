using System.Collections.Generic;

namespace Hmcr.Model.Dtos.SaltReport
{
    public class SaltReportDto
    {
        public int SaltReportId { get; set; }
        public string ServiceArea { get; set; }
        public string ContactName { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public Sect1Dto Sect1 { get; set; }
    }

    public class Sect1Dto {
        public string PlanDeveloped { get; set; }
        public string PlanReviewed { get; set; }
        public string PlanUpdated { get; set; }
        public TrainingDto Training { get; set; }
        public ObjectivesDto Objectives { get; set; }
    }

    public class TrainingDto
    {
        public string Manager { get; set; }
        public string Supervisor { get; set; }
        public string Operator { get; set; }
        public string Mechanical { get; set; }
        public string Patroller { get; set; }
    }

    public class ObjectivesDto
    {
        public MaterialStorageDto MaterialStorage { get; set; }
        public SaltApplicationDto SaltApplication { get; set; }
    }

    public class MaterialStorageDto
    {
        public string Identified { get; set; }
        public string Achieved { get; set; }
    }

    public class SaltApplicationDto
    {
        public string Identified { get; set; }
        public string Achieved { get; set; }
    }
}
