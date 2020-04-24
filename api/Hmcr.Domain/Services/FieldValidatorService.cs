using Hmcr.Model;
using Hmcr.Model.Dtos.CodeLookup;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hmcr.Domain.Services
{
    public interface IFieldValidatorService
    {
        void Validate<T>(string entityName, string fieldName, T value, Dictionary<string, List<string>> errors, int rowNum = 0);
        void Validate<T>(string entityName, T entity, Dictionary<string, List<string>> errors, int rowNum = 0, params string[] fieldsToSkip);
        IEnumerable<CodeLookupCache> CodeLookup { get; set; }
    }
    public class FieldValidatorService : IFieldValidatorService
    {
        List<FieldValidationRule> _rules;
        RegexDefs _regex;
        public IEnumerable<CodeLookupCache> CodeLookup { get; set; }
        public FieldValidatorService(RegexDefs regex)
        {
            _rules = new List<FieldValidationRule>();
            _regex = regex;

            LoadUserEntityRules();
            LoadRoleEntityRules();

            LoadWorkReportInitRules();
            LoadWorkReportD2Rules();
            LoadWorkReportD3Rules();
            LoadWorkReportD4Rules();
            LoadWorkReportHighwayUniqueRule();
            LoadWorkReportSiteRule();
            LoadWorkReportStructureRule();
            LoadWorkReportValueOfWorkRule();

            LoadRockfallReportInitRules();
            LoadRockfallReportRules();
            LoadRockfallReportGpsRules();
            LoadRockfallReportLrsRules();
            LoadRockfallOtherDitchVolumeRules();
            LoadRockfallOtherTravelledLanesVolumeRules();

            LoadWildlifeReportInitRules();
            LoadWildlifeReportRules();
            LoadWildlifeReportGpsRules();
            LoadWildlifeReportLrsRules();

            LoadActivityCodeRules();
            LoadActivityCodeForLocationCRules();
        }


        public IEnumerable<FieldValidationRule> GetFieldValidationRules(string entityName)
        {
            return _rules.Where(x => x.EntityName.ToLowerInvariant() == entityName.ToLowerInvariant());
        }

        private void LoadActivityCodeRules()
        {
            _rules.Add(new FieldValidationRule(Entities.ActivityCode, Fields.ActivityNumber, FieldTypes.String, true, 1, 6, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Alphanumeric), null));
            _rules.Add(new FieldValidationRule(Entities.ActivityCode, Fields.ActivityName, FieldTypes.String, true, 1, 150, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.ActivityCode, Fields.UnitOfMeasure, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.UnitOfMeasure));
            _rules.Add(new FieldValidationRule(Entities.ActivityCode, Fields.MaintenanceType, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WrkRptMaintType));
            _rules.Add(new FieldValidationRule(Entities.ActivityCode, Fields.FeatureType, FieldTypes.String, false, null, null, null, null, null, null, null, CodeSet.FeatureType));
            _rules.Add(new FieldValidationRule(Entities.ActivityCode, Fields.SpThresholdLevel, FieldTypes.String, false, null, null, null, null, null, null, null, CodeSet.ThresholdSp, LookupItem.Name));
        }

        /// <summary>
        /// When Location Code is C, point line feature and tolerance levle is required
        /// </summary>
        private void LoadActivityCodeForLocationCRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.ActivityCode).Select(x => x.ShallowCopy(Entities.ActivityCodeLocationCodeC)).ToArray());

            _rules.First(x => x.EntityName == Entities.ActivityCodeLocationCodeC && x.FieldName == Fields.FeatureType).Required = true;
            _rules.First(x => x.EntityName == Entities.ActivityCodeLocationCodeC && x.FieldName == Fields.SpThresholdLevel).Required = true;
        }

        private void LoadUserEntityRules()
        {
            _rules.Add(new FieldValidationRule(Entities.User, Fields.Username, FieldTypes.String, true, 1, 32, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.UserType, FieldTypes.String, true, 1, 30, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.EndDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.User, Fields.Email, FieldTypes.String, true, 1, 100, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Email), null));
        }

        private void LoadRoleEntityRules()
        {
            _rules.Add(new FieldValidationRule(Entities.Role, Fields.Name, FieldTypes.String, true, 1, 30, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.Role, Fields.Description, FieldTypes.String, true, 1, 150, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.Role, Fields.EndDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
        }

        private void LoadWorkReportInitRules()
        {
            //Common for all work reports
            _rules.Add(new FieldValidationRule(Entities.WorkReportInit, Fields.RecordNumber, FieldTypes.String, true, 1, 12, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportInit, Fields.EndDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
        }

        private void LoadWorkReportD2Rules()
        {
            //Common for all work reports
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.RecordType, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WrkRptMaintType));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.RecordNumber, FieldTypes.String, true, 1, 12, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.TaskNumber, FieldTypes.String, false, 0, 12, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.ActivityNumber, FieldTypes.String, true, 6, 6, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartDate, FieldTypes.Date, false, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.Accomplishment, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.DollarValue), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.UnitOfMeasure, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.UnitOfMeasure));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.PostedDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));

            //Highway Unique is not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.HighwayUnique, FieldTypes.String, false, 1, 16, null, null, null, null, null, null));

            //GPS info is not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));

            //LRS info is not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StartOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Offset), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.EndOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Offset), null));

            //Structure and Site info are not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.StructureNumber, FieldTypes.String, false, 2, 6, null, null, null, null, _regex.GetRegexInfo(RegexDefs.StructureNumber), null));
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.SiteNumber, FieldTypes.String, false, 0, 8, null, null, null, null, _regex.GetRegexInfo(RegexDefs.SiteNumber), null));

            //Value of work is not required
            _rules.Add(new FieldValidationRule(Entities.WorkReportD2, Fields.ValueOfWork, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.DollarValue), null));
        }


        # region For work report field validation which depends on other fields 

        /// <summary>
        /// For D2 and location code = 'B', HighwayUnique is required
        /// </summary>
        private void LoadWorkReportHighwayUniqueRule()
        {
            var rule = _rules.First(x => x.EntityName == Entities.WorkReportD2 && x.FieldName == Fields.HighwayUnique).ShallowCopy(Entities.WorkReportHighwayUnique);
            _rules.Add(rule);
            rule.Required = true;
        }
        /// <summary>
        /// For special activity numbers (ActivityNumbers.SiteRequired), SiteNumber is required
        /// </summary>
        private void LoadWorkReportSiteRule()
        {
            var rule = _rules.First(x => x.EntityName == Entities.WorkReportD2 && x.FieldName == Fields.SiteNumber).ShallowCopy(Entities.WorkReportSite);
            _rules.Add(rule);
            rule.Required = true;
        }
        /// <summary>
        /// For activity numbers which start with 6, StructureNumber is required
        /// </summary>
        private void LoadWorkReportStructureRule()
        {
            var rule = _rules.First(x => x.EntityName == Entities.WorkReportD2 && x.FieldName == Fields.StructureNumber).ShallowCopy(Entities.WorkReportStructure);
            _rules.Add(rule);
            rule.Required = true;
        }
        /// <summary>
        /// For activity numbers which start with 6, StructureNumber is required
        /// </summary>
        private void LoadWorkReportValueOfWorkRule()
        {
            var rule = _rules.First(x => x.EntityName == Entities.WorkReportD2 && x.FieldName == Fields.ValueOfWork).ShallowCopy(Entities.WorkReportValueOfWork);
            _rules.Add(rule);
            rule.Required = true;
        }
        #endregion

        private void LoadWorkReportD3Rules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WorkReportD2).Select(x => x.ShallowCopy(Entities.WorkReportD3)).ToArray());

            //For D3, Highway Unique, GPS info and value of work are required.
            _rules.First(x => x.EntityName == Entities.WorkReportD3 && x.FieldName == Fields.HighwayUnique).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD3 && x.FieldName == Fields.StartLatitude).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD3 && x.FieldName == Fields.StartLongitude).Required = true;
        }

        private void LoadWorkReportD4Rules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WorkReportD2).Select(x => x.ShallowCopy(Entities.WorkReportD4)).ToArray());

            //For D4, Highway Unique, LRS info and value of work are required.
            _rules.First(x => x.EntityName == Entities.WorkReportD4 && x.FieldName == Fields.HighwayUnique).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD4 && x.FieldName == Fields.Landmark).Required = true;
            _rules.First(x => x.EntityName == Entities.WorkReportD4 && x.FieldName == Fields.StartOffset).Required = true;
        }

        private void LoadRockfallReportInitRules()
        {
            _rules.Add(new FieldValidationRule(Entities.RockfallReportInit, Fields.McrrIncidentNumber, FieldTypes.String, true, 1, 12, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReportInit, Fields.ReportDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
        }

        public void LoadRockfallReportRules()
        {
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.RecordType, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.RkflRptRecordType));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.McrrIncidentNumber, FieldTypes.String, true, 1, 12, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EstimatedRockfallDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EstimatedRockfallTime, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Time), null));

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.StartLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.StartLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EndLatitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EndLongitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.HighwayUnique, FieldTypes.String, true, 1, 16, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.HighwayUniqueName, FieldTypes.String, false, 1, 255, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.LandmarkName, FieldTypes.String, false, 0, 255, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.StartOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Offset), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.EndOffset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Offset), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.DirectionFromLandmark, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Direction), null));

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.LocationDescription, FieldTypes.String, false, 0, 4000, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.DitchVolume, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.VolumeRange));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.TravelledLanesVolume, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.VolumeRange));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.OtherDitchVolume, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Volume), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.OtherTravelledLanesVolume, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Volume), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.HeavyPrecip, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.YN), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.FreezeThaw, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.YN), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.DitchSnowIce, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.YN), null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.VehicleDamage, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.YN), null));

            //todo: other ditch volume and other travelled lanes volume

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.McName, FieldTypes.String, false, 1, 150, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.Name, FieldTypes.String, true, 1, 150, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.McPhoneNumber, FieldTypes.String, true, 10, 12, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.RockfallReport, Fields.ReportDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));

        }

        public void LoadRockfallReportGpsRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.RockfallReport).Select(x => x.ShallowCopy(Entities.RockfallReportGps)).ToArray());

            _rules.First(x => x.EntityName == Entities.RockfallReportGps && x.FieldName == Fields.StartLatitude).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReportGps && x.FieldName == Fields.StartLongitude).Required = true;
        }

        public void LoadRockfallReportLrsRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.RockfallReport).Select(x => x.ShallowCopy(Entities.RockfallReportLrs)).ToArray());

            _rules.First(x => x.EntityName == Entities.RockfallReportLrs && x.FieldName == Fields.Landmark).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReportLrs && x.FieldName == Fields.LandmarkName).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReportLrs && x.FieldName == Fields.StartOffset).Required = true;
            _rules.First(x => x.EntityName == Entities.RockfallReportLrs && x.FieldName == Fields.DirectionFromLandmark).Required = true;
        }

        #region Field Validation for Rockfall
        public void LoadRockfallOtherDitchVolumeRules()
        {
            _rules.Add(new FieldValidationRule(Entities.RockfallReportOtherDitchVolume, Fields.OtherDitchVolume, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Volume), null));
        }

        public void LoadRockfallOtherTravelledLanesVolumeRules()
        {
            _rules.Add(new FieldValidationRule(Entities.RockfallReportOtherTravelledLanesVolume, Fields.OtherTravelledLanesVolume, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Volume), null));
        }
        #endregion

        public void LoadWildlifeReportInitRules()
        {
            _rules.Add(new FieldValidationRule(Entities.WildlifeReportInit, Fields.AccidentDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
        }

        public void LoadWildlifeReportRules()
        {
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.RecordType, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsRptRecordType));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.AccidentDate, FieldTypes.Date, true, null, null, null, null, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), null, null));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.TimeOfKill, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsTime));

            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Latitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Longitude, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.GpsCoords), null));

            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.HighwayUnique, FieldTypes.String, true, 1, 16, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Landmark, FieldTypes.String, false, 0, 8, null, null, null, null, null, null));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Offset, FieldTypes.String, false, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Offset), null));
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.NearestTown, FieldTypes.String, false, 0, 150, null, null, null, null, null, null));

            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.WildlifeSign, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsRptSign)); //LOV
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Quantity, FieldTypes.String, true, null, null, null, null, null, null, _regex.GetRegexInfo(RegexDefs.Quantity), null));

            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Species, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarSpecies)); //LOV
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Sex, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsSex)); //LOV
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Age, FieldTypes.String, true, null, null, null, null, null, null, null, CodeSet.WarsAge)); //LOV
            _rules.Add(new FieldValidationRule(Entities.WildlifeReport, Fields.Comments, FieldTypes.String, false, 0, 1024, null, null, null, null, null, null));
        }

        #region Field validation for Wildlife
        public void LoadWildlifeReportGpsRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WildlifeReport).Select(x => x.ShallowCopy(Entities.WildlifeReportGps)).ToArray());

            _rules.First(x => x.EntityName == Entities.WildlifeReportGps && x.FieldName == Fields.Latitude).Required = true;
            _rules.First(x => x.EntityName == Entities.WildlifeReportGps && x.FieldName == Fields.Longitude).Required = true;
        }

        public void LoadWildlifeReportLrsRules()
        {
            _rules.AddRange(_rules.Where(x => x.EntityName == Entities.WildlifeReport).Select(x => x.ShallowCopy(Entities.WildlifeReportLrs)).ToArray());

            _rules.First(x => x.EntityName == Entities.WildlifeReportLrs && x.FieldName == Fields.Landmark).Required = true;
            _rules.First(x => x.EntityName == Entities.WildlifeReportLrs && x.FieldName == Fields.Offset).Required = true;
            _rules.First(x => x.EntityName == Entities.WildlifeReportLrs && x.FieldName == Fields.NearestTown).Required = true;
        }
        #endregion

        public void Validate<T>(string entityName, T entity, Dictionary<string, List<string>> errors, int rowNum = 0, params string[] fieldsToSkip)
        {
            var fields = typeof(T).GetProperties();

            foreach (var field in fields)
            {
                if (fieldsToSkip.Any(x => x == field.Name))
                    continue;

                Validate(entityName, field.Name, field.GetValue(entity), errors, rowNum);
            }
        }

        public void Validate<T>(string entityName, string fieldName, T val, Dictionary<string, List<string>> errors, int rowNum = 0)
        {
            var rule = _rules.FirstOrDefault(r => r.EntityName == entityName && r.FieldName == fieldName);

            if (rule == null)
                return;

            var messages = new List<string>();

            switch (rule.FieldType)
            {
                case FieldTypes.String:
                    messages.AddRange(ValidateStringField(rule, val, rowNum));
                    break;
                case FieldTypes.Date:
                    messages.AddRange(ValidateDateField(rule, val));
                    break;
                default:
                    throw new NotImplementedException($"Validation for {rule.FieldType} is not implemented.");
            }

            if (messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    errors.AddItem(rule.FieldName, message);
                }
            }
        }

        private List<string> ValidateStringField<T>(FieldValidationRule rule, T val, int rowNum = 0)
        {
            var messages = new List<string>();

            var rowNumPrefix = rowNum == 0 ? "" : $"Row # {rowNum}: ";

            var field = rule.FieldName.WordToWords();

            if (rule.Required && val is null)
            {
                messages.Add($"{rowNumPrefix}The {field} field is required.");
                return messages;
            }

            if (!rule.Required && (val is null || val.ToString().IsEmpty()))
                return messages;

            string value = Convert.ToString(val);

            if (rule.Required && value.IsEmpty())
            {
                messages.Add($"{rowNumPrefix}The {field} field is required.");
                return messages;
            }

            if (rule.MinLength != null && rule.MaxLength != null)
            {
                if (value.Length < rule.MinLength || value.Length > rule.MaxLength)
                {
                    messages.Add($"{rowNumPrefix}The length of {field} field must be between {rule.MinLength} and {rule.MaxLength}.");
                }
            }

            if (rule.Regex != null)
            {
                if (!Regex.IsMatch(value, rule.Regex.Regex))
                {
                    var message = string.Format(rule.Regex.ErrorMessage, val.ToString());
                    messages.Add($"{rowNumPrefix}{message}.");
                }
            }

            if (rule.CodeSet != null)
            {
                var exists = rule.LookupItem == LookupItem.Value ?
                    CodeLookup.Any(x => x.CodeSet == rule.CodeSet && x.CodeValue.ToLowerInvariant() == value.ToLowerInvariant()) :
                    CodeLookup.Any(x => x.CodeSet == rule.CodeSet && x.CodeName.ToLowerInvariant() == value.ToLowerInvariant());

                if (!exists)
                {
                    messages.Add($"{rowNumPrefix}Invalid value. [{value}] doesn't exist in the code set {rule.CodeSet}.");
                }
            }

            return messages;
        }

        private List<string> ValidateDateField<T>(FieldValidationRule rule, T val, int rowNum = 0)
        {
            var messages = new List<string>();

            var rowNumPrefix = rowNum == 0 ? "" : $"Row # {rowNum}: ";

            var field = rule.FieldName.WordToWords();

            if (rule.Required && val is null)
            {
                messages.Add($"{rowNumPrefix}{field} field is required.");
                return messages;
            }

            if (!rule.Required && (val is null || val.ToString().IsEmpty()))
                return messages;

            var (parsed, parsedDate) = DateUtils.ParseDate(val);

            if (!parsed)
            {
                messages.Add($"{rowNumPrefix}Cannot convert {field} field to date");
                return messages;
            }

            var value = parsedDate;

            if (rule.MinDate != null && rule.MaxDate != null)
            {
                if (value < rule.MinDate || value > rule.MaxDate)
                {
                    messages.Add($"{rowNumPrefix}The length of {field} must be between {rule.MinDate} and {rule.MaxDate}.");
                }
            }

            return messages;
        }
    }
}
