using Hmcr.Data.Repositories;
using Hmcr.Model;
using Hmcr.Model.Dtos.SubmissionStatus;
using System.Collections.Generic;
using System.Linq;
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
        decimal FileStage3InProgress { get; }
        decimal FileStage4InProgress { get; }
        decimal FileServiceAreaError { get; }
        decimal FileSuccessWithWarnings { get; }
        
        decimal RowReceived { get; }
        decimal RowError { get; }
        decimal RowDuplicate { get; }
        decimal RowSuccess { get; }
        
        Task<IEnumerable<SubmissionStatusDto>> GetSubmissionStatusAsync();

        bool IsFileInProgress(decimal submissionStatusId);
    }
    public class SubmissionStatusService : ISubmissionStatusService
    {
        private ISubmissionStatusRepository _statusRepo;

        private IEnumerable<SubmissionStatusDto> _statuses;
        public IEnumerable<SubmissionStatusDto> Statuses => _statuses ??= _statusRepo.GetActiveStatuses();

        #region File Status
        private decimal? _fileReceived;
        public decimal FileReceived => _fileReceived ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileReceived);

        private decimal? _fileError;
        public decimal FileError => _fileError ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileError);

        private decimal? _fileDuplicate;
        public decimal FileDuplicate => _fileDuplicate ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileDuplicate);

        private decimal? _fileInProgress;
        public decimal FileInProgress => _fileInProgress ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileInProgress);

        private decimal? _fileDataError;
        public decimal FileBasicError => _fileDataError ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileBasicError);

        private decimal? _fileConflictError;
        public decimal FileConflictionError => _fileConflictError ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileConflictionError);

        private decimal? _fileLocationError;
        public decimal FileLocationError => _fileLocationError ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileLocationError);

        private decimal? _fileUnexpectedError;
        public decimal FileUnexpectedError => _fileUnexpectedError ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileUnexpectedError);

        private decimal? _fileSuccess;
        public decimal FileSuccess => _fileSuccess ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileSuccess);

        private decimal? _fileStage3InProgress;
        public decimal FileStage3InProgress => _fileStage3InProgress ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileStage3InProgress);

        private decimal? _fileStage4InProgress;
        public decimal FileStage4InProgress => _fileStage4InProgress ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileStage4InProgress);

        private decimal? _fileServiceAreaError;
        public decimal FileServiceAreaError => _fileServiceAreaError ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileServiceAreaError);

        private decimal? _fileSuccessWithWarnings;
        public decimal FileSuccessWithWarnings => _fileSuccessWithWarnings ??= GetStatusIdByTypeAndCode(StatusType.File, FileStatus.FileSuccessWithWarnings);
        #endregion

        #region Row Status
        private decimal? _rowReceived;
        public decimal RowReceived => _rowReceived ??= GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.RowReceived);

        private decimal? _rowError;
        public decimal RowError => _rowError ??= GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.RowError);

        private decimal? _rowDuplicate;
        public decimal RowDuplicate => _rowDuplicate ??= GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.RowDuplicate);

        private decimal? _rowSuccess;
        public decimal RowSuccess => _rowSuccess ??= GetStatusIdByTypeAndCode(StatusType.Row, RowStatus.RowSuccess);
        #endregion

        public SubmissionStatusService(ISubmissionStatusRepository statusRepo)
        {
            _statusRepo = statusRepo;
        }

        public async Task<IEnumerable<SubmissionStatusDto>> GetSubmissionStatusAsync()
        {
            return await _statusRepo.GetActiveStatusesAsync();
        }

        private decimal GetStatusIdByTypeAndCode(string type, string code)
        {
            return Statuses.First(x => x.StatusType == type && x.StatusCode == code).StatusId;
        }

        public bool IsFileInProgress(decimal submissionStatusId)
        {
            bool isFileInProgress = false;

            if ((submissionStatusId == FileInProgress) 
                || (submissionStatusId == FileStage3InProgress) 
                || (submissionStatusId == FileStage4InProgress))
            {
                isFileInProgress = true;
            }

            return isFileInProgress;
        }
    }
}
