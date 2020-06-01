using System.ComponentModel.DataAnnotations;

namespace PushDemoApi.Models
{
    public class NotificationHubOptions
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string ConnectionString { get; set; }
    }
}