namespace Logging
{
    static  class LoggerSystem
    {
        public static readonly ILogger Default = new DebugConsoleLogger();
        public static readonly ILogger DebugConsole = new DebugConsoleLogger();
        public static readonly ILogger NoLogging = new NoLoggingLogger();
        public static readonly ILogger TextLogTag = new TextLogTagLogger();

        public static void Log(string s, ILogger logger)
        {
            logger.Log("---Log: \n" + s);
        }

        public static void Log(string s)
        {
            Default.Log("---Log: \n" + s);
        }
    }
}
