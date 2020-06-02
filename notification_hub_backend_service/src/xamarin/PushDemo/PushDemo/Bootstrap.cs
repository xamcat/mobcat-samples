using System;
using PushDemo.Services;

namespace PushDemo
{
    public static class Bootstrap
    {
        public static void Begin(Func<IDeviceInstallationService> deviceInstallationService)
        {
            ServiceContainer.Register(deviceInstallationService);
            ServiceContainer.Register<IPushDemoNotificationActionService>(() => new PushDemoNotificationActionService());
            ServiceContainer.Register<INotificationRegistrationService>(() => new NotificationRegistrationService(Config.BackendServiceEndpoint, Config.ApiKey));
        }
    }
}