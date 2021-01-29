using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace GodelTech.CodeReview.Orchestrator.Utils
{
    public class SimplifiedConsoleLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new SimplifiedConsoleLogger(categoryName);
        }

        public class SimplifiedConsoleLogger : ILogger
        {
            private readonly string _categoryName;

            public bool IncludeLogLevel { get; set; }
            public bool IncludeEventId { get; set; }
            public bool IncludeCategory { get; set; }
            
            public SimplifiedConsoleLogger(string categoryName)
            {
                _categoryName = categoryName;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                if (!IsEnabled(logLevel))
                    return;

                var builder = new StringBuilder();

                builder.Append("[");
                builder.Append(DateTime.UtcNow.ToString("HH:mm:ss.ffff"));
                builder.Append("] ");

                if (IncludeLogLevel)
                {
                    builder.Append('$');
                    builder.Append(logLevel);
                    builder.Append(": ");
                }

                if (IncludeCategory)
                {
                    builder.Append(_categoryName);
                }

                if (IncludeEventId)
                {
                    builder.Append('[');
                    builder.Append(eventId.Id);
                    builder.Append("]:");
                }

                builder.Append(formatter(state, exception));

                Console.WriteLine(builder);
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }
        }
    }
}