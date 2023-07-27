FROM image-registry.openshift-image-registry.svc:5000/d3d940-tools/node-python:1 AS builder
LABEL maintainer="bruce.wang@gov.bc.ca"


RUN npm --version
RUN npm install -g npm@9.8.0
COPY . ./src

RUN cd ./src && npm ci && npm run build
 
FROM image-registry.openshift-image-registry.svc:5000/d3d940-tools/nginx-116-rhel8:1

COPY --from=builder /src/build/. /tmp/src
COPY --from=builder /src/nginx-start/. /tmp/src/nginx-start
COPY --from=builder /src/nginx.conf.tmpl /tmp/src/nginx.conf.tmpl

RUN $STI_SCRIPTS_PATH/assemble

CMD $STI_SCRIPTS_PATH/run