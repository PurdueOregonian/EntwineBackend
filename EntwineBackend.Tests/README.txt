IMPORTANT: You need to create a file called ApiKey.txt with a Google Cloud API key, and put it in EntwineBackend.
IMPORTANT: You need to set the environment variable POSTGRES_LOCATION to the location of the bin folder (that contains pg_dump.exe) in Postgres.

To add a migration and make a new dump if schema changes:
Run Add-Migration.ps1 with the name of the migration.

If the path to this project contains spaces, the tests using Docker will NOT work.