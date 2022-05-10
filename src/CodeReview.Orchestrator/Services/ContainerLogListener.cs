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
        private readonly CancellationTokenSource _cancellationTokenSource;

        private Task _listenTask;

        public ContainerLogListener(ILogger<ContainerLogListener> logger,
            long waitTimeoutSeconds,
            string containerId, 
            MultiplexedStream stream)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _containerId = containerId ?? throw new ArgumentNullException(nameof(containerId));
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));

            _cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(waitTimeoutSeconds));
        }

        public void StartListening()
        {
            if (_listenTask != null)
                throw new InvalidOperationException("Already listening a stream");

            _listenTask = Task.Factory.StartNew(Listen, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning);
        }

        public void StopListening()
        {
            if (_listenTask == null || _cancellationTokenSource.IsCancellationRequested)
                return;

            _cancellationTokenSource.Cancel();
        }

        private void Listen(object obj)
        {
            var buffer = ArrayPool<byte>.Shared.Rent(81920);

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
            catch (TaskCanceledException) { }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);

                _stream.Dispose();
            }
        }
    }
}