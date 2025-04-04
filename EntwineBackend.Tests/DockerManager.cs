﻿using Docker.DotNet.Models;
using Docker.DotNet;
using Npgsql;

namespace EntwineBackend_Tests
{
    public class DockerManager : IDisposable
    {
        private DockerClient _dockerClient;
        private string _containerId;
        private string _volumeName;

        public DockerManager()
        {
            _dockerClient = new DockerClientConfiguration(
                new Uri("npipe://./pipe/docker_engine")
            ).CreateClient();
        }

        public async Task StartPostgresContainerAsync(string password)
        {
            _volumeName = $"pgdata_{Guid.NewGuid()}";

            await _dockerClient.Volumes.CreateAsync(new VolumesCreateParameters
            {
                Name = _volumeName
            });

            var schemaPath = Path.GetFullPath("./schema_dump.sql").Replace("\\", "/");

            if (!File.Exists(schemaPath))
            {
                throw new FileNotFoundException("The schema dump file was not found.", schemaPath);
            }

            var containerParams = new CreateContainerParameters
            {
                Image = "postgres:latest",
                Name = "test-instance",
                Env = [$"POSTGRES_PASSWORD={password}"],
                HostConfig = new HostConfig
                {
                    PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    { "5432/tcp", new List<PortBinding> { new PortBinding { HostPort = "5433" } } }
                },
                    Binds = new List<string> {
                        $"{_volumeName}:/var/lib/postgresql/data",
                        $"{schemaPath}:/docker-entrypoint-initdb.d/schema_dump.sql"
                    }
                }
            };

            await _dockerClient.Images.CreateImageAsync(new ImagesCreateParameters { FromImage = "postgres", Tag = "latest" }, null, new Progress<JSONMessage>());

            var container = await _dockerClient.Containers.CreateContainerAsync(containerParams);
            _containerId = container.ID;

            await _dockerClient.Containers.StartContainerAsync(_containerId, null);

            await WaitForPostgresAsync("Host=localhost;Port=5433;Username=postgres;Password=eleanordonahue;Database=postgres");
        }

        private async Task WaitForPostgresAsync(string connectionString)
        {
            bool isPostgresReady = false;
            int retries = 10; // Retry up to 10 times
            while (!isPostgresReady && retries > 0)
            {
                try
                {
                    using (var conn = new NpgsqlConnection(connectionString))
                    {
                        await conn.OpenAsync(); // Try to open a connection
                        isPostgresReady = true;
                    }
                }
                catch (Exception)
                {
                    retries--;
                    await Task.Delay(2000); // Wait for 2 seconds before retrying
                }
            }

            if (!isPostgresReady)
            {
                throw new Exception("PostgreSQL container did not become ready in time.");
            }
        }

        public async Task StopAndRemoveContainerAsync()
        {
            if (!string.IsNullOrEmpty(_containerId))
            {
                await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters());

                await _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters { Force = true });

                await _dockerClient.Volumes.RemoveAsync(_volumeName, true);
            }
        }

        public void Dispose()
        {
            StopAndRemoveContainerAsync().Wait();
            _dockerClient?.Dispose();
        }
    }
}
