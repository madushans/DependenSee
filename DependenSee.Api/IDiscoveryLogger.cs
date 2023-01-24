namespace DependenSee.Api;

public interface IDiscoveryLogger
{
    void LogInfo(string message);
    void LogWarn(string message);
    void LogError(string message);
}

public class NullDiscoveryLogger : IDiscoveryLogger
{
    public void LogInfo(string message) { }
    public void LogWarn(string message) { }
    public void LogError(string message) { }
}