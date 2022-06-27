using System;
using System.Buffers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Services
{
    public sealed class ContainerLogListener : IContainerLogListener
    {
        private readonly ILogger<ContainerLogListener> _logger;

        private readonly string _containerId;
        private readonly MultiplexedStream _stream;

        private Task _listenTask;
        private CancellationTokenSource _cancellationTokenSource;

        public ContainerLogListener(ILogger<ContainerLogListener> logger,
            string containerId,
            MultiplexedStream stream)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _containerId = containerId ?? throw new ArgumentNullException(nameof(containerId));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public void Start(long timeoutSeconds)
        {
            if (_listenTask != null)
                throw new InvalidOperationException($"ContainerId={_containerId} Listener already listen the container");

            _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));

            _listenTask = Task.Factory.StartNew(Listen, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
        }

        public Task CloseAsync()
        {
            if (_listenTask == null || _cancellationTokenSource?.IsCancellationRequested != false)
                return Task.CompletedTask;

            _cancellationTokenSource.Cancel(false);
            return _listenTask;
        }

        private void Listen(object obj)
        {
            const int minimumBufferLength = 81920;

            var buffer = ArrayPool<byte>.Shared.Rent(minimumBufferLength);

            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var result = _stream.ReadOutputAsync(buffer, 0, buffer.Length, _cancellationTokenSource.Token).GetAwaiter().GetResult();

                    if (result.EOF)
                        return;

                    var dockerLog = Encoding.Default.GetString(buffer, 0, result.Count);
                    var dockerLogs = dockerLog.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    foreach (var log in dockerLogs)
                    {
                        switch (result.Target)
                        {
                            case MultiplexedStream.TargetStream.StandardOut:
                                _logger.LogInformation("Container={containerId} Stdout: {stdOut}", _containerId, log);
                                break;
                            case MultiplexedStream.TargetStream.StandardError:
                                _logger.LogError("Container={containerId} Stderr: {stdOut}", _containerId, log);
                                break;
                        }
                    }
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogDebug("Container={containerId} The container listening task has been canceled", _containerId);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (!_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel(false);
                await _listenTask;
            }

            _stream.Dispose();
        }
    }
}