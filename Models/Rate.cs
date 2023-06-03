using System;

namespace DiakontTestTask.Models
{
    public class Rate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Salary { get; set; }
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }
    }
}
