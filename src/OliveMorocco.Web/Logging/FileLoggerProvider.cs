using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace OliveMorocco.Web.Logging;

/// <summary>
/// Writes <see cref="LogLevel.Error"/> and above to a file on disk (VPS persistence).
/// </summary>
public sealed class FileLoggerProvider(string path) : ILoggerProvider
{
    private readonly ConcurrentDictionary<string, FileLogger> _loggers = new();

    public ILogger CreateLogger(string categoryName) =>
        _loggers.GetOrAdd(categoryName, _ => new FileLogger(path, categoryName));

    public void Dispose() => _loggers.Clear();

    private sealed class FileLogger(string path, string category) : ILogger
    {
        private static readonly object Gate = new();

        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= LogLevel.Error;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
                return;

            var message = formatter(state, exception);
            var line = $"{DateTimeOffset.UtcNow:O} [{logLevel}] {category}: {message}";

            if (exception is not null)
                line += Environment.NewLine + exception;

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
                Directory.CreateDirectory(directory);

            lock (Gate)
            {
                File.AppendAllText(path, line + Environment.NewLine);
            }
        }
    }
}
