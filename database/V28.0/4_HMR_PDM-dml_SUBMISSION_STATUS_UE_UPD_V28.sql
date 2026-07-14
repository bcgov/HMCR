USE HMR_DEV; -- uncomment appropriate instance
--USE HMR_TST;
--USE HMR_UAT;
--USE HMR_PRD;
GO

-- HMCR: Replace the vague 'Unexpected Error' submission status wording with
-- clear, instructive text. Shown in the Submission Status column and the
-- status help popover of the Work Reporting Submissions screen.
UPDATE HMR_SUBMISSION_STATUS
	SET DESCRIPTION = 'Processing Failed - System Error'
	-- LONG_DESCRIPTION is VARCHAR(255)
	, LONG_DESCRIPTION = 'The file could not be processed due to an unexpected system error, not a problem with the submitted data. Open the submission for detail and submit the file again; if the error persists, contact the administrator with the submission number.'
	, CONCURRENCY_CONTROL_NUMBER = CONCURRENCY_CONTROL_NUMBER + 1
	WHERE STATUS_CODE = 'UE' AND STATUS_TYPE = 'F'
GO
