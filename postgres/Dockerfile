FROM postgres:11-alpine

ENV DB_MASTER_USER=postgres
ENV DB_MASTER_PASS=password
ENV DB_HOST=postgres
ENV DB_PORT=5432
ENV DB_NAME=appdb

COPY ./init.sh /docker-entrypoint-initdb.d/init.sh
