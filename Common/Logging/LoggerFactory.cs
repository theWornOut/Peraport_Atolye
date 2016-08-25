using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Common.Logging.ConfigurationSections;
using Common.Logging.Interfaces;
using Common.Exceptions;

namespace Common.Logging
{
    public class LoggerFactory : ILoggerFactory
    {
        public LoggerWriter CreateLogWriter()
        {
            var lggers = GetLoggers();
            return new LoggerWriter(lggers.ToArray());
        }

        IEnumerable<ILogger> GetLoggers()
        {
            var loggerConfigSection = ConfigurationManager.GetSection("LoggerSection") as LoggerConfigurationSection;
            //todo:Unutulanlar unutanlar� asla unutmaz.
            if (loggerConfigSection == null) new PeraportException("Config ayarlar�nda null s�k�nt�s� var", new ArgumentNullException());
            var loggerDictionary =
                loggerConfigSection.Loggers.Cast<LoggerConfigurationElement>()
                    .ToDictionary(k => k.Name, element => element.Type);
            foreach (var type in loggerDictionary)
            {
                var parameters = loggerConfigSection.Loggers.Cast<LoggerConfigurationElement>()
                    .Single(s => s.Name == type.Key)
                    .Constructor.Parameters.Cast<ParameterElement>().ToList();

                if (parameters.Any())
                {
                    var typeParameters = parameters.Select(s => Convert.ChangeType(s.Value, s.Type)).ToArray();
                    yield return Activator.CreateInstance(type.Value, typeParameters) as ILogger;
                }
                else
                {
                    yield return Activator.CreateInstance(type.Value) as ILogger;
                }
            }
        }
    }
}