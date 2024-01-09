using System;
using VCPL.Еnvironment;

namespace VCPLBrowser;

public class VCPLBrowserLogger : ILogger
{
    private readonly Action<string> _log_func;
    public VCPLBrowserLogger(Action<string> log_func) { this._log_func = log_func; }
    public void Log(string message)
    {
        _log_func(message);
        _log_func("\n");
    }
}
