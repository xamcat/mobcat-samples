using PushDemo.Models;

namespace PushDemo.Services
{
    public interface IDeviceInstallationService
    {
        string GetDeviceId();
        DeviceInstallation GetDeviceRegistration(params string[] tags);
    }
}