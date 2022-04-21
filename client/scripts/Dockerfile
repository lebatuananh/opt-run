FROM nginx:1.14.2-alpine as base
LABEL name="lebatuananh/runotp.webapp"

FROM node:12 as dep
WORKDIR /runotp
RUN chmod 777 -R /runotp
COPY ./package.json /runotp
RUN npm install --unsafe-perm
RUN npm dedupe

COPY . /runotp
RUN npm run build -- --aot --prod --output-hashing=all --output-path /dist

FROM base
COPY --from=dep /dist /dist
COPY ./scripts/site.conf /etc/nginx/conf.d/default.conf
CMD nginx -g 'daemon off;'
