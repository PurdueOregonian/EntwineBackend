To make a new dump if schema changes:
pg_dump --schema-only --username=postgres --dbname=postgres > schema_dump.sql