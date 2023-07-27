FROM image-registry.openshift-image-registry.svc:5000/d3d940-tools/node-python:1 AS builder
LABEL maintainer="bruce.wang@gov.bc.ca"

RUN find / -name "package.json"
RUN ls -l
COPY . ./src
RUN pwd
RUN cd ./src
RUN pwd
RUN ls -l
RUN pwd
RUN cd src
RUN ls -l
# RUN cd ./src 
RUN npm ci
RUN npm run build

FROM image-registry.openshift-image-registry.svc:5000/d3d940-tools/nginx-116-rhel8:1

COPY --from=builder /src/build/. /tmp/src
COPY --from=builder /src/nginx-start/. /tmp/src/nginx-start
COPY --from=builder /src/nginx.conf.tmpl /tmp/src/nginx.conf.tmpl

RUN $STI_SCRIPTS_PATH/assemble

CMD $STI_SCRIPTS_PATH/run