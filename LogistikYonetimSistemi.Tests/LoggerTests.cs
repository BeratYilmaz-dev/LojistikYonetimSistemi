using LogistikYonetimSistemi.Utils;
using Xunit;

namespace LogistikYonetimSistemi.Tests;

public class LoggerTests
{
    [Fact]
    public void Instance_IsNotNull()
    {
        Assert.NotNull(Logger.Instance);
    }

    [Fact]
    public void Instance_ReturnsSameObject_Singleton()
    {
        var a = Logger.Instance;
        var b = Logger.Instance;

        Assert.Same(a, b);
    }

    [Fact]
    public void Log_WritesToFile()
    {
        var logger = Logger.Instance;
        string uniqueMsg = $"TEST-{Guid.NewGuid()}";

        logger.Log(uniqueMsg);

        Assert.True(File.Exists(logger.LogFilePath));
        string content = File.ReadAllText(logger.LogFilePath);
        Assert.Contains(uniqueMsg, content);
    }

    [Fact]
    public void LogStockChange_ContainsProductId()
    {
        var logger = Logger.Instance;
        string productId = $"TEST-PROD-{Guid.NewGuid():N}";

        logger.LogStockChange(productId, 100, 80);

        string content = File.ReadAllText(logger.LogFilePath);
        Assert.Contains(productId, content);
    }
}
