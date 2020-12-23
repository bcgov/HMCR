using Hmcr.Chris;
using Hmcr.Chris.Models;
using Hmcr.Model;
using Hmcr.Model.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        Task<(SpValidationResult result, List<SurfaceType> surfaceTypes)> GetSurfaceTypeAssocWithLineAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber);
        Task<(SpValidationResult result, SurfaceType surfaceType)> GetSurfaceTypeAssocWithPointAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber);
        Task<(SpValidationResult result, List<MaintenanceClass> maintenanceClasses)> GetMaintenanceClassAssocWithLineAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber);
        Task<(SpValidationResult result, MaintenanceClass maintenanceClass)> GetMaintenanceClassAssocWithPointAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber);
        Task<(SpValidationResult result, List<Structure> structures)> GetStructuresOnRFISegmentAsync(string rfiSegmentName, string recordNumber);
        Task<(SpValidationResult result, HighwayProfile highwayProfile)> GetHighwayProfileAssocWithPointAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber);
        Task<(SpValidationResult result, List<HighwayProfile> highwayProfiles)> GetHighwayProfileAssocWithLineAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber);
        Task<(SpValidationResult result, List<Guardrail> guardrails)> GetGuardrailAssociatedWithLineAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber);
        Task<(SpValidationResult result, Guardrail guardrail)> GetGuardrailAssociatedWithPointAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber);
        Task<(SpValidationResult result, List<RestArea> restAreas)> GetRestAreasOnRFISegmentAsync(string rfiSegmentName, string recordNumber);

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

        public async Task<(SpValidationResult result, List<SurfaceType> surfaceTypes)> GetSurfaceTypeAssocWithLineAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber)
        {
            var surfaceTypes = await _inventoryApi.GetSurfaceTypeAssociatedWithLine(geometry, recordNumber);

            return (SpValidationResult.Success, surfaceTypes);
        }

        public async Task<(SpValidationResult result, SurfaceType surfaceType)> GetSurfaceTypeAssocWithPointAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber)
        {
            var surfaceType = await _inventoryApi.GetSurfaceTypeAssociatedWithPoint(geometry, recordNumber);

            return (SpValidationResult.Success, surfaceType);
        }

        public async Task<(SpValidationResult result, MaintenanceClass maintenanceClass)> GetMaintenanceClassAssocWithPointAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber)
        {
            var maintenanceClass = await _inventoryApi.GetMaintenanceClassesAssociatedWithPoint(geometry, recordNumber);

            return (SpValidationResult.Success, maintenanceClass);
        }

        public async Task<(SpValidationResult result, List<MaintenanceClass> maintenanceClasses)> GetMaintenanceClassAssocWithLineAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber)
        {
            var maintenanceClasses = await _inventoryApi.GetMaintenanceClassesAssociatedWithLine(geometry, recordNumber);

            return (SpValidationResult.Success, maintenanceClasses);
        }

        public async Task<(SpValidationResult result, List<Structure> structures)> GetStructuresOnRFISegmentAsync(string rfiSegmentName, string recordNumber)
        {
            var structures = await _inventoryApi.GetStructuresOnRFISegment(rfiSegmentName, recordNumber);

            return (SpValidationResult.Success, structures);
        }

        public async Task<(SpValidationResult result, List<HighwayProfile> highwayProfiles)> GetHighwayProfileAssocWithLineAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber)
        {
            var highwayProfiles = await _inventoryApi.GetHighwayProfileAssociatedWithLine(geometry, recordNumber);

            return (SpValidationResult.Success, highwayProfiles);
        }

        public async Task<(SpValidationResult result, HighwayProfile highwayProfile)> GetHighwayProfileAssocWithPointAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber)
        {
            var highwayProfile = await _inventoryApi.GetHighwayProfileAssociatedWithPoint(geometry, recordNumber);

            return (SpValidationResult.Success, highwayProfile);
        }

        public async Task<(SpValidationResult result, List<Guardrail> guardrails)> GetGuardrailAssociatedWithLineAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber)
        {
            var guardrails = await _inventoryApi.GetGuardrailAssociatedWithLine(geometry, recordNumber);

            return (SpValidationResult.Success, guardrails);
        }

        public async Task<(SpValidationResult result, Guardrail guardrail)> GetGuardrailAssociatedWithPointAsync(NetTopologySuite.Geometries.Geometry geometry, string recordNumber)
        {
            var guardrail = await _inventoryApi.GetGuardrailAssociatedWithPoint(geometry, recordNumber);

            return (SpValidationResult.Success, guardrail);
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
