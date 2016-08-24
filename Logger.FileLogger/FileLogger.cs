using Common.Rolling;
using Common.Logging;
using Common.Logging.Interfaces;

namespace Logger.FileLoggerImpl
{
    public class FileLogger : ILogger
    {
        private readonly IRollingFileTextWriter _fileTextWriter;
        public FileLogger(IRollingFileTextWriter fileTextWriter)
        {
            _fileTextWriter = fileTextWriter;
        }

        public int LogFileSize
        {
            get { return LogFileSize; }
            set
            {
                LogFileSize = 10000000;
            }
        }


        public async void Log(LogLevel level, int eventId, string message)
        {
            _fileTextWriter.RollingSize = LogFileSize;
            var logMessage = $"LogLevel => {level} - EventId => {eventId} - Message => {message}";
            await _fileTextWriter.RollLogFile(logMessage);
        }
    }
}
