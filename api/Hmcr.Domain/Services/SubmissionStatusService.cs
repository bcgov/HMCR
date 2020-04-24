using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionStatus;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface ISubmissionStatusService
    {
        decimal FileReceived { get; }
        decimal FileError { get; }
        decimal FileDuplicate { get; }
        decimal FileInProgress { get; }
        decimal FileBasicError { get; }
        decimal FileConflictionError { get; }
        decimal FileLocationError { get; }
        decimal FileUnexpectedError { get; }
        decimal FileSuccess { get; }

        decimal RowReceived { get; }
        decimal RowError { get; }
        decimal RowDuplicate { get; }
        decimal RowSuccess { get; }

        Task<IEnumerable<SubmissionStatusDto>> GetSubmissionStatusAsync();
        Task<IEnumerable<SubmissionStatusDto>> GetActiveStatusesAsync();
    }
    public class SubmissionStatusService : ISubmissionStatusService
    {
        private ISubmissionStatusRepository _statusRepo;

        #region File Status
        private decimal? _fileReceived;
        public decimal FileReceived => _fileReceived ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileReceived);

        private decimal? _fileError;
        public decimal FileError => _fileError ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileError);

        private decimal? _fileDuplicate;
        public decimal FileDuplicate => _fileDuplicate ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileDuplicate);

        private decimal? _fileInProgress;
        public decimal FileInProgress => _fileInProgress ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileInProgress);

        private decimal? _fileDataError;
        public decimal FileBasicError => _fileDataError ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileBasicError);

        private decimal? _fileConflictError;
        public decimal FileConflictionError => _fileConflictError ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileConflictionError);

        private decimal? _fileLocationError;
        public decimal FileLocationError => _fileLocationError ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileLocationError);

        private decimal? _fileUnexpectedError;
        public decimal FileUnexpectedError => _fileUnexpectedError ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileUnexpectedError);

        private decimal? _fileSuccess;
        public decimal FileSuccess => _fileSuccess ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileSuccess);
        #endregion

        #region Row Status
        private decimal? _rowReceived;
        public decimal RowReceived => _rowReceived ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.RowReceived);

        private decimal? _rowError;
        public decimal RowError => _rowError ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.RowError);

        private decimal? _rowDuplicate;
        public decimal RowDuplicate => _rowDuplicate ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.RowDuplicate);

        private decimal? _rowSuccess;
        public decimal RowSuccess => _rowSuccess ??= _statusRepo.GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.RowSuccess);
        #endregion

        public SubmissionStatusService(ISubmissionStatusRepository statusRepo)
        {
            _statusRepo = statusRepo;
        }

        public async Task<IEnumerable<SubmissionStatusDto>> GetSubmissionStatusAsync()
        {
            return await _statusRepo.GetActiveStatusesAsync();
        }

        public async Task<IEnumerable<SubmissionStatusDto>> GetActiveStatusesAsync()
        {
            return await _statusRepo.GetActiveStatusesAsync();
        }
    }
}
