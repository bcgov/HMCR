FROM node:26-alpine AS builder
WORKDIR /app

RUN apk add --no-cache python3 make g++

COPY package*.json ./
RUN npm ci

COPY . .
RUN npm run build

FROM nginx:1.27-alpine

ENV HOME=/opt/app-root/src

RUN apk add --no-cache gettext \
    && mkdir -p /opt/app-root/src/js \
      /opt/app-root/etc/nginx.d \
      /opt/app-root/etc/nginx.default.d \
      /var/cache/nginx \
      /var/run \
      /run \
      /var/log/nginx \
    && chgrp -R 0 /opt/app-root /var/cache/nginx /var/run /run /var/log/nginx /etc/nginx \
    && chmod -R g+rwX /opt/app-root /var/cache/nginx /var/run /run /var/log/nginx /etc/nginx

COPY --from=builder /app/build/ /opt/app-root/src/
COPY nginx.conf.tmpl /tmp/src/nginx.conf.tmpl
COPY nginx-start/start.sh /docker-entrypoint.d/10-start.sh

RUN chmod +x /docker-entrypoint.d/10-start.sh \
    && touch /opt/app-root/src/js/env.config.js \
    && chgrp 0 /opt/app-root/src/js/env.config.js \
    && chmod g+rw /opt/app-root/src/js/env.config.js

EXPOSE 8080
CMD ["nginx", "-g", "daemon off;"]
