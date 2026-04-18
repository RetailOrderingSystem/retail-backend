using Serilog;
using Serilog.Events;

namespace RetailAPI.Logs
{
    public static class LoggerService
    {
        public static void ConfigureSerilog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(outputTemplate:
                    "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "Logs/retail-api-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}",
                    retainedFileCountLimit: 30)
                .CreateLogger();
        }

        public static void LogInfo(string message) => Log.Information(message);
        public static void LogWarning(string message) => Log.Warning(message);
        public static void LogError(string message, System.Exception? ex = null)
        {
            if (ex != null) Log.Error(ex, message);
            else Log.Error(message);
        }
    }
}
