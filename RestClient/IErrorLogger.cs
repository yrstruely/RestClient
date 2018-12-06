using NLog;
using System;

public interface IErrorLogger
{
    void LogInfo(string infoMessage);
    void LogWarning(string warningMessage);
    void LogError(Exception ex, string errorMessage);
}

public class ErrorLogger : IErrorLogger
{
    private readonly Logger logger = LogManager.GetCurrentClassLogger();

    public void LogInfo(string infoMessage)
    {
        logger.Info(infoMessage);
    }

    public void LogWarning(string warningMessage)
    {
        logger.Warn(warningMessage);
    }

    public void LogError(Exception ex, string errorMessage)
    {
        logger.Error(ex, errorMessage);
    }
}