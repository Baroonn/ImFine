namespace ImFine.Server
{
    public class LogTest
    {
        private readonly ILogger<LogTest> _logger;
        public LogTest(ILogger<LogTest> logger)
        {
            _logger = logger;
            var message = $" LogTest logger created at {DateTime.UtcNow.ToLongTimeString()}";
            _logger.LogInformation(message);
        }
    }
}
