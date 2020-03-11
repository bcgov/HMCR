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
        Task<(SpValidationResult result, LrsPointResult lrsResult, RfiSegment rfiSegment)> ValidateGpsPointAsync(Point point, string rfiSegment,
            string rfiSegmentName, Dictionary<string, List<string>> errors);
        Task<(SpValidationResult result, LrsPointResult startPointResult, LrsPointResult endPointResult, Line line, RfiSegment rfiSegment)> ValidateGpsLineAsync(Point startPoint, 
            Point endPoint, string rfiSegment, string rfiSegmentName, Dictionary<string, List<string>> errors);
        Task<(SpValidationResult result, decimal snappedOffset, Point point, RfiSegment rfiSegment)> ValidateLrsPointAsync(decimal offset, string rfiSegment, string rfiSegmentName, Dictionary<string, List<string>> errors);
        Task<(SpValidationResult result, decimal snappedStartOffset, decimal snappedEndOffset, Point startPoint, Point endPoint, Line line, RfiSegment rfiSegment)> ValidateLrsLineAsync(decimal startOffset, decimal endOffset, 
            string rfiSegment, string rfiSegmentName, Dictionary<string, List<string>> errors);
    }

    public class SpatialService : ISpatialService
    {
        private IOasApi _oasApi;

        private IFieldValidatorService _validator;

        private int? _errorThreshold = null;
        private int ErrorThreshold 
        { 
            get 
            { 
                return _errorThreshold ??= Convert.ToInt32(_validator.CodeLookup.FirstOrDefault(x => x.CodeSet == CodeSet.ThresholdSpError)?.CodeValueNum ?? 0); 
            }
        }

        private IEnumerable<string> _nonSpHighwayUniques = null;
        private IEnumerable<string> NonSpHighwayUniques
        {
            get
            {
                return _nonSpHighwayUniques ??= _validator.CodeLookup.Where(x => x.CodeSet == CodeSet.NonSpHighwayUnique).Select(x => x.CodeValue).ToArray().ToLowercase();
            }
        }

        public SpatialService(IOasApi oasApi, IFieldValidatorService validator)
        {
            _oasApi = oasApi;
            _validator = validator;
        }

        public async Task<(SpValidationResult result, LrsPointResult lrsResult, RfiSegment rfiSegment)> ValidateGpsPointAsync(Point point, string rfiSegment, 
            string rfiSegmentName, Dictionary<string, List<string>> errors)
        {
            var rfiResult = await ValidateRfiSegmentAsync(rfiSegment, rfiSegmentName, errors);
            if (rfiResult.result != SpValidationResult.Success)
            {
                return (rfiResult.result, null, null);
            }

            var isOnRfi = await _oasApi.IsPointOnRfiSegmentAsync(ErrorThreshold, point, rfiSegment);

            if (!isOnRfi)
            {
                errors.AddItem($"GPS position", $"GPS position [{point.Longitude}/{point.Latitude}] is not on the {rfiSegmentName} [{rfiSegment}] within the tolerance [{ErrorThreshold}] metres");
                return (SpValidationResult.Fail, null, rfiResult.segment);
            }

            var result = await _oasApi.GetOffsetMeasureFromPointAndRfiSegmentAsync(point, rfiSegment);

            if (result.success)
            {

                return (SpValidationResult.Success, result.result, rfiResult.segment);
            }
            else
            {
                errors.AddItem($"GPS position", $"Couldn't get offset on the {rfiSegmentName} [{rfiSegment}] from the GPS position [{point.Longitude}/{point.Latitude}]");
                return (SpValidationResult.Fail, result.result, rfiResult.segment);
            }
        }

        public async Task<(SpValidationResult result, LrsPointResult startPointResult, LrsPointResult endPointResult, Line line, RfiSegment rfiSegment)> ValidateGpsLineAsync(Point startPoint, Point endPoint, string rfiSegment,
            string rfiSegmentName, Dictionary<string, List<string>> errors)
        {
            var rfiResult = await ValidateRfiSegmentAsync(rfiSegment, rfiSegmentName, errors);
            if (rfiResult.result != SpValidationResult.Success)
            {
                return (rfiResult.result, null, null, null, null);
            }

            var isStartOnRfi = await _oasApi.IsPointOnRfiSegmentAsync(ErrorThreshold, startPoint, rfiSegment);

            if (!isStartOnRfi)
            {
                errors.AddItem($"Start GPS position", $"Start GPS position [{startPoint.Longitude}/{startPoint.Latitude}] is not on the {rfiSegmentName} [{rfiSegment}] within the tolerance [{ErrorThreshold}] metres");
            }

            var isEndOnRfi = await _oasApi.IsPointOnRfiSegmentAsync(ErrorThreshold, endPoint, rfiSegment);

            if (!isEndOnRfi)
            {
                errors.AddItem($"End GPS position", $"End GPS position [{endPoint.Longitude}/{endPoint.Latitude}] is not on the {rfiSegmentName} [{rfiSegment}] within the tolerance [{ErrorThreshold}] metres");
            }

            if (errors.Count > 0)
            {
                return (SpValidationResult.Fail, null, null, null, rfiResult.segment);
            }

            var startResult = await _oasApi.GetOffsetMeasureFromPointAndRfiSegmentAsync(startPoint, rfiSegment);
            if (!startResult.success)
            {
                errors.AddItem($"Start GPS position", $"Couldn't get offset on the {rfiSegmentName} [{rfiSegment}] from the start GPS position [{startPoint.Longitude}/{startPoint.Latitude}]");
            }

            var endResult = await _oasApi.GetOffsetMeasureFromPointAndRfiSegmentAsync(endPoint, rfiSegment);
            if (!endResult.success)
            {
                errors.AddItem($"End GPS position", $"Couldn't get offset on the {rfiSegmentName} [{rfiSegment}] from the end GPS position [{endPoint.Longitude}/{endPoint.Latitude}]");
            }

            if (errors.Count > 0)
            {
                return (SpValidationResult.Fail, null, null, null, rfiResult.segment);
            }

            if (rfiResult.segment.Dimension != RecordDimension.Line || startResult.result.Offset == endResult.result.Offset)
            {
                return (SpValidationResult.Success, startResult.result, endResult.result, new Line(startResult.result.SnappedPoint), rfiResult.segment);
            }

            var line = await _oasApi.GetLineFromOffsetMeasuerOnRfiSegmentAsync(rfiSegment, startResult.result.Offset, endResult.result.Offset);

            return (SpValidationResult.Success, startResult.result, endResult.result, line, rfiResult.segment);
        }

        public async Task<(SpValidationResult result, decimal snappedOffset, Point point, RfiSegment rfiSegment)> ValidateLrsPointAsync(decimal offset, string rfiSegment, string rfiSegmentName, Dictionary<string, List<string>> errors)
        {
            var rfiResult = await ValidateRfiSegmentAsync(rfiSegment, rfiSegmentName, errors);
            if (rfiResult.result != SpValidationResult.Success)
            {
                return (rfiResult.result, offset, null, null);
            }

            var (withinTolerance, snappedOffset) = GetSnappedOffset(rfiResult.segment, offset, rfiSegment, rfiSegmentName, errors);
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

        public async Task<(SpValidationResult result, decimal snappedStartOffset, decimal snappedEndOffset, Point startPoint, Point endPoint, Line line, RfiSegment rfiSegment)> ValidateLrsLineAsync(decimal startOffset, decimal endOffset, string rfiSegment, string rfiSegmentName, Dictionary<string, List<string>> errors)
        {
            var snappedStartOffset = startOffset;
            var snappedEndOffset = endOffset;

            var rfiResult = await ValidateRfiSegmentAsync(rfiSegment, rfiSegmentName, errors);
            if (rfiResult.result != SpValidationResult.Success)
            {
                return (rfiResult.result, snappedStartOffset, snappedEndOffset, null, null, null, null);
            }

            var startTolCheck = GetSnappedOffset(rfiResult.segment, startOffset, rfiSegment, rfiSegmentName, errors);
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

            var endTolCheck = GetSnappedOffset(rfiResult.segment, endOffset, rfiSegment, rfiSegmentName, errors);
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

            if (rfiResult.segment.Dimension != RecordDimension.Line || snappedStartOffset == snappedEndOffset)
            {
                return (SpValidationResult.Success, snappedStartOffset, snappedEndOffset, startPoint, endPoint, new Line(startPoint), rfiResult.segment);
            }

            var line = await _oasApi.GetLineFromOffsetMeasuerOnRfiSegmentAsync(rfiSegment, snappedStartOffset, snappedEndOffset);

            return (SpValidationResult.Success, snappedStartOffset, snappedEndOffset, startPoint, endPoint, line, rfiResult.segment);
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

        private (bool withinTolerance, decimal snappedOffset) GetSnappedOffset(RfiSegment segment, decimal offset, string rfiSegment,
            string rfiSegmentName, Dictionary<string, List<string>> errors)
        {
            var snappedOffset = offset;

            if (segment.Length < offset)
            {
                if (segment.Length + ErrorThreshold / 1000M < offset) 
                {
                    errors.AddItem($"Offset", $"Offset [{offset}] is not on the {rfiSegmentName} [{rfiSegment}] within the tolerance [{ErrorThreshold}] metres");
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
