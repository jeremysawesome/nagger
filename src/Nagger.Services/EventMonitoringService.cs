namespace Nagger.Services
{
    using System;
    using System.Diagnostics;
    using Interfaces;
    using Microsoft.Win32;

    public class EventMonitoringService : IDisposable
    {
        readonly SessionSwitchEventHandler _sessionSwitchEventHandler;
        readonly Action<EventMonitoringService> _eventHandler;
        readonly IOutputService _outputService;

        public EventMonitoringService(IOutputService outputService, Action<EventMonitoringService> handler)
        {
            _eventHandler = handler;
            _sessionSwitchEventHandler = HandleLock;
            _outputService = outputService;
        }

        public void Monitor()
        {
            SystemEvents.SessionSwitch += _sessionSwitchEventHandler;
        }

        void HandleLock(object sender, SessionSwitchEventArgs e)
        {
            switch (e.Reason)
            {
                case SessionSwitchReason.SessionLock:
                    _outputService.ShowInterface();
                    _eventHandler.Invoke(this);
                    break;
                case SessionSwitchReason.SessionUnlock:
                    _outputService.HideInterface();
                    break;
            }
        }

        public void Dispose()
        {
            SystemEvents.SessionSwitch -= _sessionSwitchEventHandler;
        }
    }
}
