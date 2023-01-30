namespace DependenSee.Core;

/// <summary>
/// Logger that can write different severity messages.
/// During discovery, different messages are sent via the logger
/// describing its process, which provides insights to the 
/// discovery process and aid in troubleshooting in the event 
/// of a failure or missing/incorrect results.
/// </summary>
public interface IDiscoveryLogger
{
    /// <summary>
    /// Writes an info message
    /// </summary>
    void LogInfo(string message);

    /// <summary>
    /// Writes a warning message.
    /// </summary>
    void LogWarn(string message);

    /// <summary>
    /// Writes an error message
    /// </summary>
    void LogError(string message);
}

/// <summary>
/// Logger that ignores all messages. This is used as the default
/// if no other logger is provided.
/// </summary>
public class NullDiscoveryLogger : IDiscoveryLogger
{
    /// <inheritdoc/>
    public void LogInfo(string message) { }
    /// <inheritdoc/>
    public void LogWarn(string message) { }
    /// <inheritdoc/>
    public void LogError(string message) { }
}