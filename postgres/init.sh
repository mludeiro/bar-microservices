#!/usr/bin/env sh
#
# initializes postgres if db is completely empty
#

set -e

if [ -f /mnt/artifacts/scripts/init_db.sql ]; then
  envsubst '$DB_HOST,$DB_PORT,$DB_NAME,$DB_USER,$DB_PASS,$DB_MASTER_USER,$DB_MASTER_PASS' < /mnt/artifacts/scripts/init_db.sql > /tmp/init_db.sql
  psql --file /tmp/init_db.sql
fi