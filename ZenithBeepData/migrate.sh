#!/bin/bash

set -o allexport
source .env
set +o allexport

check_postgres_connection() {
    pg_isready -h $POSTGRES_HOST -p $POSTGRES_PORT -U $POSTGRES_USER -d $POSTGRES_DB
}

run_ef_migration() {
    dotnet ef database update
}

echo "Waiting for PostgreSQL to be ready..."
while ! check_postgres_connection; do
  echo "Postgres is not ready yet. Retrying in 1 second..."
  sleep 1
done
echo "PostgreSQL is ready"
run_ef_migration

