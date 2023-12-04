using System.ComponentModel.DataAnnotations;

namespace BggExt.Models
{
    public class User
    {
        public string Id { get; set; } = null!;

        [DataType(DataType.DateTime)]
        public DateTime? LastSynchronized { get; set; }
    }
}