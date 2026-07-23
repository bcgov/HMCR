ALTER TABLE IF EXISTS hmr_log
  ADD COLUMN IF NOT EXISTS support_id text,
  ADD COLUMN IF NOT EXISTS error_code text,
  ADD COLUMN IF NOT EXISTS correlation_id text,
  ADD COLUMN IF NOT EXISTS trace_id text,
  ADD COLUMN IF NOT EXISTS source text,
  ADD COLUMN IF NOT EXISTS operation text,
  ADD COLUMN IF NOT EXISTS actor_type text,
  ADD COLUMN IF NOT EXISTS actor_username text,
  ADD COLUMN IF NOT EXISTS actor_directory text,
  ADD COLUMN IF NOT EXISTS actor_user_guid text,
  ADD COLUMN IF NOT EXISTS http_method text,
  ADD COLUMN IF NOT EXISTS request_path text,
  ADD COLUMN IF NOT EXISTS status_code text,
  ADD COLUMN IF NOT EXISTS submission_object_id text,
  ADD COLUMN IF NOT EXISTS service_area_number text;

CREATE INDEX IF NOT EXISTS ix_hmr_log_support_id ON hmr_log (support_id) WHERE support_id IS NOT NULL;
CREATE INDEX IF NOT EXISTS ix_hmr_log_error_code ON hmr_log (error_code) WHERE error_code IS NOT NULL;
CREATE INDEX IF NOT EXISTS ix_hmr_log_correlation_id ON hmr_log (correlation_id) WHERE correlation_id IS NOT NULL;
CREATE INDEX IF NOT EXISTS ix_hmr_log_actor_username ON hmr_log (actor_username) WHERE actor_username IS NOT NULL;
CREATE INDEX IF NOT EXISTS ix_hmr_log_source_operation ON hmr_log (source, operation) WHERE source IS NOT NULL;
CREATE INDEX IF NOT EXISTS ix_hmr_log_request_path ON hmr_log (request_path) WHERE request_path IS NOT NULL;
CREATE INDEX IF NOT EXISTS ix_hmr_log_submission_object_id ON hmr_log (submission_object_id) WHERE submission_object_id IS NOT NULL;
CREATE INDEX IF NOT EXISTS ix_hmr_log_service_area_number ON hmr_log (service_area_number) WHERE service_area_number IS NOT NULL;
