{{- define "hmcr.name" -}}
{{ .Values.name | default .Chart.Name }}{{ .Values.suffix }}
{{- end -}}

{{- define "hmcr.fullname" -}}
{{ .Release.Name }}-{{ include "hmcr.name" . }}
{{- end -}}
