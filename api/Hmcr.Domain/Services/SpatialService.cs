using Hmcr.Chris;
using Hmcr.Chris.Models;
using Hmcr.Model;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GJFeature = GeoJSON.Net.Feature;  // use an alias since Feature exists in HttpClients.Models
namespace Hmcr.Domain.Services
{
    public interface ISpatialService
    {
        Task<(SpValidationResult result, LrsPointResult lrsResult, RfiSegment rfiSegment)> ValidateGpsPointAsync
            (Point point, string rfiSegment, string rfiSegmentName, string thresholdLevel, Dictionary<string, List<string>> errors);
        Task<(SpValidationResult result, LrsPointResult startPointResult, LrsPointResult endPointResult, List<Line> lines, RfiSegment rfiSegment)> ValidateGpsLineAsync
            (Point startPoint, Point endPoint, string rfiSegment, string rfiSegmentName, string thresholdLevel, Dictionary<string, List<string>> errors);
        Task<(SpValidationResult result, decimal snappedOffset, Point point, RfiSegment rfiSegment)> ValidateLrsPointAsync
            (decimal offset, string rfiSegment, string rfiSegmentName, string thresholdLevel, Dictionary<string, List<string>> errors);
        Task<(SpValidationResult result, decimal snappedStartOffset, decimal snappedEndOffset, Point startPoint, Point endPoint, List<Line> lines, RfiSegment rfiSegment)> 
            ValidateLrsLineAsync(decimal startOffset, decimal endOffset, string rfiSegment, string rfiSegmentName, string thresholdLevel, Dictionary<string, List<string>> errors);
        
        Task<(SpValidationResult result, List<Structure> structures)> GetStructuresOnRFISegmentAsync(string rfiSegmentName, string recordNumber);
        Task<(SpValidationResult result, List<RestArea> restAreas)> GetRestAreasOnRFISegmentAsync(string rfiSegmentName, string recordNumber);

        Task<(SpValidationResult result, string message, List<SurfaceType> surfaceTypes)> GetSurfaceTypesAssocWithRFISegment(string rfiSegmentName, string recordNumber, decimal startOffset, decimal endOffset);
        Task<(SpValidationResult result, string message, List<MaintenanceClass> maintenanceClasses)> GetMaintenanceClassAssocWithRFISegment(string rfiSegmentName, string recordNumber, decimal startOffset, decimal endOffset);
        Task<(SpValidationResult result, string message, List<HighwayProfile> highwayProfiles)> GetHighwayProfileAssocWithRFISegment(string rfiSegmentName, string recordNumber, decimal startOffset, decimal endOffset);
        Task<(SpValidationResult result, string message, List<Guardrail> guardrails)> GetGuardrailsAssocWithRFISegment(string rfiSegmentName, string recordNumber, decimal startOffset, decimal endOffset);
    }

    public class SpatialService : ISpatialService
    {
        private IOasApi _oasApi;
        private IInventoryApi _inventoryApi;

        private IFieldValidatorService _validator;
        private ILookupCodeService _lookupService;

        private IEnumerable<string> _nonSpHighwayUniques = null;
        private IEnumerable<string> NonSpHighwayUniques => _nonSpHighwayUniques ??= _validator.CodeLookup.Where(x => x.CodeSet == CodeSet.NonSpHighwayUnique).Select(x => x.CodeValue).ToArray().ToLowercase();

        public SpatialService(IOasApi oasApi, IFieldValidatorService validator, ILookupCodeService lookupService, IInventoryApi inventoryApi)
        {
            _oasApi = oasApi;
            _validator = validator;
            _lookupService = lookupService;
            _inventoryApi = inventoryApi;
        }

        public async Task<(SpValidationResult result, LrsPointResult lrsResult, RfiSegment rfiSegment)> ValidateGpsPointAsync
            (Point point, string rfiSegment, string rfiSegmentName, string thresholdLevel, Dictionary<string, List<string>> errors)
        {
            var rfiResult = await ValidateRfiSegmentAsync(rfiSegment, rfiSegmentName, errors);
            if (rfiResult.result != SpValidationResult.Success)
            {
                return (rfiResult.result, null, null);
            }

            var threshold = _lookupService.GetThresholdLevel(thresholdLevel);

            var isOnRfi = await _oasApi.IsPointOnRfiSegmentAsync(threshold.Error, point, rfiSegment);

            if (!isOnRfi)
            {
                errors.AddItem($"GPS position", $"GPS position [{point.Latitude},{point.Longitude}] is not on the {rfiSegmentName} [{rfiSegment}] within the tolerance [{threshold.Error}] metres");
                return (SpValidationResult.Fail, null, rfiResult.segment);
            }

            var result = await _oasApi.GetOffsetMeasureFromPointAndRfiSegmentAsync(point, rfiSegment);

            if (result.success)
            {

                return (SpValidationResult.Success, result.result, rfiResult.segment);
            }
            else
            {
                errors.AddItem($"GPS position", $"Couldn't get offset on the {rfiSegmentName} [{rfiSegment}] from the GPS position [{point.Latitude},{point.Longitude}]");
                return (SpValidationResult.Fail, result.result, rfiResult.segment);
            }
        }

        public async Task<(SpValidationResult result, LrsPointResult startPointResult, LrsPointResult endPointResult, List<Line> lines, RfiSegment rfiSegment)> ValidateGpsLineAsync
            (Point startPoint, Point endPoint, string rfiSegment, string rfiSegmentName, string thresholdLevel, Dictionary<string, List<string>> errors)
        {
            var rfiResult = await ValidateRfiSegmentAsync(rfiSegment, rfiSegmentName, errors);
            if (rfiResult.result != SpValidationResult.Success)
            {
                return (rfiResult.result, null, null, null, null);
            }

            var threshold = _lookupService.GetThresholdLevel(thresholdLevel);

            var isStartOnRfi = await _oasApi.IsPointOnRfiSegmentAsync(threshold.Error, startPoint, rfiSegment);

            if (!isStartOnRfi)
            {
                errors.AddItem($"Start GPS position", $"Start GPS position [{startPoint.Latitude},{startPoint.Longitude}] is not on the {rfiSegmentName} [{rfiSegment}] within the tolerance [{threshold.Error}] metres");
            }

            var isEndOnRfi = await _oasApi.IsPointOnRfiSegmentAsync(threshold.Error, endPoint, rfiSegment);

            if (!isEndOnRfi)
            {
                errors.AddItem($"End GPS position", $"End GPS position [{endPoint.Latitude},{endPoint.Longitude}] is not on the {rfiSegmentName} [{rfiSegment}] within the tolerance [{threshold.Error}] metres");
            }

            if (errors.Count > 0)
            {
                return (SpValidationResult.Fail, null, null, null, rfiResult.segment);
            }

            var startResult = await _oasApi.GetOffsetMeasureFromPointAndRfiSegmentAsync(startPoint, rfiSegment);
            if (!startResult.success)
            {
                errors.AddItem($"Start GPS position", $"Couldn't get offset on the {rfiSegmentName} [{rfiSegment}] from the start GPS position [{startPoint.Latitude},{startPoint.Longitude}]");
            }

            var endResult = await _oasApi.GetOffsetMeasureFromPointAndRfiSegmentAsync(endPoint, rfiSegment);
            if (!endResult.success)
            {
                errors.AddItem($"End GPS position", $"Couldn't get offset on the {rfiSegmentName} [{rfiSegment}] from the end GPS position [{endPoint.Latitude},{endPoint.Longitude}]");
            }

            if (errors.Count > 0)
            {
                return (SpValidationResult.Fail, null, null, null, rfiResult.segment);
            }

            var lines = new List<Line>();

            if (rfiResult.segment.Dimension != RecordDimension.Line || startResult.result.Offset == endResult.result.Offset)
            {
                lines.Add(new Line(startResult.result.SnappedPoint));
                return (SpValidationResult.Success, startResult.result, endResult.result, lines, rfiResult.segment);
            }

            lines = await _oasApi.GetLineFromOffsetMeasureOnRfiSegmentAsync(rfiSegment, startResult.result.Offset, endResult.result.Offset);

            return (SpValidationResult.Success, startResult.result, endResult.result, lines, rfiResult.segment);
        }

        public async Task<(SpValidationResult result, decimal snappedOffset, Point point, RfiSegment rfiSegment)> ValidateLrsPointAsync
            (decimal offset, string rfiSegment, string rfiSegmentName, string thresholdLevel, Dictionary<string, List<string>> errors)
        {
            var rfiResult = await ValidateRfiSegmentAsync(rfiSegment, rfiSegmentName, errors);
            if (rfiResult.result != SpValidationResult.Success)
            {
                return (rfiResult.result, offset, null, null);
            }

            var (withinTolerance, snappedOffset) = GetSnappedOffset(rfiResult.segment, offset, rfiSegment, rfiSegmentName, thresholdLevel, errors);
            if (!withinTolerance)
            {
                return (SpValidationResult.Fail, rfiResult.segment.Length, null, rfiResult.segment);
            }

            //Get point by snappedOffset. Otherwise, it will raise an exception when the offset is greater than length of the road
            var point = await _oasApi.GetPointFromOffsetMeasureOnRfiSegmentAsync(rfiSegment, snappedOffset);

            if (point == null)
            {
                errors.AddItem("Offset", $"Couldn't get GPS position from offset [{offset}] and {rfiSegmentName} [{rfiSegment}]");
                return (SpValidationResult.Fail, snappedOffset, point, rfiResult.segment);
            }

            return (SpValidationResult.Success, snappedOffset, point, rfiResult.segment);
        }

        public async Task<(SpValidationResult result, decimal snappedStartOffset, decimal snappedEndOffset, Point startPoint, Point endPoint, List<Line> lines, RfiSegment rfiSegment)>
            ValidateLrsLineAsync(decimal startOffset, decimal endOffset, string rfiSegment, string rfiSegmentName, string thresholdLevel, Dictionary<string, List<string>> errors)
        {
            var snappedStartOffset = startOffset;
            var snappedEndOffset = endOffset;

            var rfiResult = await ValidateRfiSegmentAsync(rfiSegment, rfiSegmentName, errors);
            if (rfiResult.result != SpValidationResult.Success)
            {
                return (rfiResult.result, snappedStartOffset, snappedEndOffset, null, null, null, null);
            }

            var startTolCheck = GetSnappedOffset(rfiResult.segment, startOffset, rfiSegment, rfiSegmentName, thresholdLevel, errors);
            if (!startTolCheck.withinTolerance)
            {
                return (SpValidationResult.Fail, rfiResult.segment.Length, rfiResult.segment.Length, null, null, null, rfiResult.segment);
            }
            snappedStartOffset = startTolCheck.snappedOffset;

            //Get point by snappedStartOffset. Otherwise, it will raise an exception when the offset is greater than length of the road
            var startPoint = await _oasApi.GetPointFromOffsetMeasureOnRfiSegmentAsync(rfiSegment, snappedStartOffset);

            if (startPoint == null)
            {
                errors.AddItem("Start Offset", $"Couldn't get start GPS position from offset [{startOffset}] and {rfiSegmentName} [{rfiSegment}]");
            }

            var endTolCheck = GetSnappedOffset(rfiResult.segment, endOffset, rfiSegment, rfiSegmentName, thresholdLevel, errors);
            if (!endTolCheck.withinTolerance)
            {
                return (SpValidationResult.Fail, rfiResult.segment.Length, rfiResult.segment.Length, null, null, null, rfiResult.segment);
            }
            snappedEndOffset = endTolCheck.snappedOffset;

            var endPoint = startPoint;

            if (snappedStartOffset != snappedEndOffset)
            {
                //Get point by snappedEndOffset. Otherwise, it will raise an exception when the offset is greater than length of the road
                endPoint = await _oasApi.GetPointFromOffsetMeasureOnRfiSegmentAsync(rfiSegment, snappedEndOffset);

                if (endPoint == null)
                {
                    errors.AddItem("End Offset", $"Couldn't get end GPS position from offset [{endOffset}] and {rfiSegmentName} [{rfiSegment}]");
                }
            }

            if (errors.Count > 0)
            {
                return (SpValidationResult.Fail, snappedStartOffset, snappedEndOffset, startPoint, endPoint, null, rfiResult.segment);
            }

            var lines = new List<Line>();

            if (rfiResult.segment.Dimension != RecordDimension.Line || snappedStartOffset == snappedEndOffset)
            {
                lines.Add(new Line(startPoint));
                return (SpValidationResult.Success, snappedStartOffset, snappedEndOffset, startPoint, endPoint, lines, rfiResult.segment);
            }

            lines = await _oasApi.GetLineFromOffsetMeasureOnRfiSegmentAsync(rfiSegment, snappedStartOffset, snappedEndOffset);

            return (SpValidationResult.Success, snappedStartOffset, snappedEndOffset, startPoint, endPoint, lines, rfiResult.segment);
        }

        private async Task<(SpValidationResult result, RfiSegment segment)> ValidateRfiSegmentAsync(string rfiSegment, string rfiSegmentName, Dictionary<string, List<string>> errors)
        {
            var rfiDetail = await _oasApi.GetRfiSegmentDetailAsync(rfiSegment);

            if (rfiDetail.Dimension == RecordDimension.Na)
            {
                if (NonSpHighwayUniques.Any(x => x == rfiSegment.ToLowerInvariant()))
                {
                    return (SpValidationResult.NonSpatial, rfiDetail);
                }

                errors.AddItem($"{rfiSegmentName}", $"{rfiSegmentName} [{rfiSegment}] is not valid.");
                return (SpValidationResult.Fail, rfiDetail);
            }

            return (SpValidationResult.Success, rfiDetail);
        }

        public async Task<(SpValidationResult result, string message, List<SurfaceType> surfaceTypes)> GetSurfaceTypesAssocWithRFISegment(string rfiSegmentName, string recordNumber, 
            decimal startOffset, decimal endOffset)
        {
            var surfaceTypes = new List<SurfaceType>();
            var validationResult = SpValidationResult.Success;

            //retrieve the feature collection for the highway unique from the ogs endpoint
            var (featureCollection, errorMsg) = await _inventoryApi.GetSurfaceType(rfiSegmentName, recordNumber);
            
            var foundFeatures = FindFeaturesWithinSegment(featureCollection, startOffset, endOffset);

            if (foundFeatures.Features.Count > 0)
            {
                foreach (var feature in foundFeatures.Features)
                {
                    SurfaceType surfaceType = new SurfaceType();
                    surfaceType.Length = (double)feature.Properties["LENGTH_KM"];
                    surfaceType.Type = (string)feature.Properties["SURFACE_TYPE"];
                    surfaceTypes.Add(surfaceType);
                }
            }
            else
            {
                if (errorMsg.Length > 0)
                    validationResult = SpValidationResult.Fail;
            }

            return (validationResult, errorMsg, surfaceTypes);
        }

        public async Task<(SpValidationResult result, string message, List<MaintenanceClass> maintenanceClasses)> GetMaintenanceClassAssocWithRFISegment(string rfiSegmentName, string recordNumber,
            decimal startOffset, decimal endOffset)
        {
            var maintenanceClasses = new List<MaintenanceClass>();
            var validationResult = SpValidationResult.Success;
            
            var (featureCollection, errorMsg) = await _inventoryApi.GetMaintenanceClass(rfiSegmentName, recordNumber);

            var foundFeatures = FindFeaturesWithinSegment(featureCollection, startOffset, endOffset);
            
            if (foundFeatures != null)
            {
                foreach (var feature in foundFeatures.Features)
                {
                    MaintenanceClass maintenanceClass = new MaintenanceClass();
                    maintenanceClass.Length = (double)feature.Properties["LENGTH_KM"];
                    maintenanceClass.SummerRating = (string)feature.Properties["SUMMER_CLASS_RATING"];
                    maintenanceClass.WinterRating = (string)feature.Properties["WINTER_CLASS_RATING"];

                    maintenanceClasses.Add(maintenanceClass);
                }
            }
            else
            {
                if (errorMsg.Length > 0)
                    validationResult = SpValidationResult.Fail;
            }

            return (validationResult, errorMsg, maintenanceClasses);
        }

        public async Task<(SpValidationResult result, string message, List<HighwayProfile> highwayProfiles)> GetHighwayProfileAssocWithRFISegment(string rfiSegmentName, string recordNumber,
            decimal startOffset, decimal endOffset)
        {
            var highwayProfiles = new List<HighwayProfile>();
            var validationResult = SpValidationResult.Success;

            var (featureCollection, errorMsg) = await _inventoryApi.GetHighwayProfile(rfiSegmentName, recordNumber);

            var foundFeatures = FindFeaturesWithinSegment(featureCollection, startOffset, endOffset);

            if (foundFeatures.Features.Count > 0)
            {
                foreach (var feature in foundFeatures.Features)
                {
                    HighwayProfile highwayProfile = new HighwayProfile();
                    highwayProfile.Length = (double)feature.Properties["LENGTH_KM"];
                    highwayProfile.NumberOfLanes = Convert.ToInt32(feature.Properties["NUMBER_OF_LANES"]);
                    highwayProfile.DividedHighwayFlag = (string)feature.Properties["DIVIDED_HIGHWAY_FLAG"];

                    highwayProfiles.Add(highwayProfile);
                }
            }
            else
            {
                if (errorMsg.Length > 0)
                    validationResult = SpValidationResult.Fail;
            }

            return (validationResult, errorMsg, highwayProfiles);
        }

        public async Task<(SpValidationResult result, string message, List<Guardrail> guardrails)> GetGuardrailsAssocWithRFISegment(string rfiSegmentName, string recordNumber,
            decimal startOffset, decimal endOffset)
        {
            var guardrails = new List<Guardrail>();
            var validationResult = SpValidationResult.Success;

            var (featureCollection, errorMsg) = await _inventoryApi.GetGuardrail(rfiSegmentName, recordNumber);

            var foundFeatures = FindFeaturesWithinSegment(featureCollection, startOffset, endOffset);

            if (foundFeatures.Features.Count > 0)
            {
                foreach (var feature in foundFeatures.Features)
                {
                    Guardrail guardrail = new Guardrail();
                    guardrail.Length = (double)feature.Properties["LENGTH_KM"];
                    guardrail.GuardrailType = (string)feature.Properties["GUARDRAIL_TYPE"];

                    guardrails.Add(guardrail);
                }
            }
            else
            {
                if (errorMsg.Length > 0)
                    validationResult = SpValidationResult.Fail;
            }

            return (validationResult, errorMsg, guardrails);
        }


        private GJFeature.FeatureCollection FindFeaturesWithinSegment(GJFeature.FeatureCollection featureCollection, decimal startOffset, decimal endOffset)
        {
            GJFeature.FeatureCollection foundFeatures = new GJFeature.FeatureCollection();

            if (featureCollection != null)
            {
                //since we got the features for the entire highway we need to determine which ones actually fall within our offsets

                //need to handle scenarios when the coordinates are 'backwards'
                // to do this we'll set the end offset as the larger value and the start as the smaller
                decimal adjStartOffset = (startOffset > endOffset) ? endOffset : startOffset;
                decimal adjEndOffset = (startOffset > endOffset) ? startOffset : endOffset;
                
                decimal newStartOffset = adjStartOffset;

                foreach (var feature in featureCollection.Features)
                {
                    //get the begin & end KM, have to use convert instead of explicit cast because of 0
                    var beginKM = Convert.ToDecimal(feature.Properties["BEGIN_KM"]);
                    var endKM = Convert.ToDecimal(feature.Properties["END_KM"]);

                    //it's in our range
                    if (newStartOffset <= adjEndOffset)
                    {
                        if ((newStartOffset >= beginKM) && (newStartOffset <= endKM))
                        {
                            var lengthKM = GetAdjustedLengthKM(adjStartOffset, adjEndOffset, beginKM, endKM, Convert.ToDecimal(feature.Properties["LENGTH_KM"]));
                            newStartOffset += lengthKM;

                            feature.Properties["LENGTH_KM"] = (double)lengthKM;

                            foundFeatures.Features.Add(feature);
                        }
                    }
                }
            }
            
            return foundFeatures;
        }

        private decimal GetAdjustedLengthKM(decimal startOffset, decimal endOffset, decimal beginKM, decimal endKM, decimal lengthKM)
        {
            //length needs to be adjusted to segment begin
            if (startOffset >= beginKM)
            {
                //don't let it go below zero
                lengthKM -= Math.Max(startOffset - beginKM, 0);
            }

            //length needs to be adjusted to segment end
            if (endOffset <= endKM)
            {
                lengthKM -= Math.Max(endKM - endOffset, 0);
            }

            return lengthKM;
        }

        public async Task<(SpValidationResult result, List<Structure> structures)> GetStructuresOnRFISegmentAsync(string rfiSegmentName, string recordNumber)
        {
            var structures = await _inventoryApi.GetStructuresOnRFISegment(rfiSegmentName, recordNumber);


            return (SpValidationResult.Success, structures);
        }

        public async Task<(SpValidationResult result, List<RestArea> restAreas)> GetRestAreasOnRFISegmentAsync(string rfiSegmentName, string recordNumber)
        {
            var restAreas = await _inventoryApi.GetRestAreasOnRFISegment(rfiSegmentName, recordNumber);

            return (SpValidationResult.Success, restAreas);
        }

        private (bool withinTolerance, decimal snappedOffset) GetSnappedOffset(RfiSegment segment, decimal offset, string rfiSegment,
            string rfiSegmentName, string thresholdLevel, Dictionary<string, List<string>> errors)
        {
            var snappedOffset = offset;

            if (segment.Length < offset)
            {
                var threshold = _lookupService.GetThresholdLevel(thresholdLevel);

                if (segment.Length + threshold.Error / 1000M < offset)
                {
                    errors.AddItem($"Offset", $"Offset [{offset}] is not on the {rfiSegmentName} [{rfiSegment}] within the tolerance [{threshold.Error}] metres");
                    return (false, snappedOffset);
                }
                else
                {
                    return (true, segment.Length);
                }
            }

            return (true, offset);
        }
    }
}
