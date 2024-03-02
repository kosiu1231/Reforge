using System.Text.Json.Serialization;

namespace Reforge.Models
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        User = 1,
        Admin = 2
    }
}
