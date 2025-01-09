To make a new dump if schema changes:
pg_dump --schema-only --username=postgres --dbname=postgres > schema_dump.sql

//TODO use the schema_dump.sql in the project
//TODO generate the schema_dump.sql automatically

If the path to this project contains spaces, the tests using Docker will NOT work.