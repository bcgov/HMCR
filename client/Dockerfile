FROM image-registry.openshift-image-registry.svc:5000/d3d940-tools/node-python:1 AS builder
LABEL maintainer="bruce.wang@gov.bc.ca"

RUN cat /etc/os-release
RUN node -v

COPY . ./src

RUN cd ./src && npm ci && npm run build
 
FROM image-registry.openshift-image-registry.svc:5000/d3d940-tools/nginx-116-rhel8:1

COPY --from=builder /src/build/. /tmp/src
COPY --from=builder /src/nginx-start/. /opt/app-root/src/nginx-start
COPY --from=builder /src/nginx.conf.tmpl /tmp/src/nginx.conf.tmpl

RUN $STI_SCRIPTS_PATH/assemble

COPY --from=builder /src/nginx-start/start.sh /opt/app-root/src/start.sh
CMD ["sh", "/opt/app-root/src/start.sh"]

