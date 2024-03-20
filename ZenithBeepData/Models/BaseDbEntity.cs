

namespace ZenithBeepData.Models
{
    public class BaseDbEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdateAt {  get; set; }
    }
}
