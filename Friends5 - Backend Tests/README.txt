docker run --name test-instance -e POSTGRES_PASSWORD=eleanordonahue -p 5433:5432 -v "C:/Users/dellG5/Desktop/schema_dump.sql:/docker-entrypoint-initdb.d/schema_dump.sql" postgres

TODO: clear database before tests
TODO: use schema_dump.sql that is inside the project