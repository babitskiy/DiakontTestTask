using System.Collections.Generic;

namespace DiakontTestTask.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        public List<Rate> Rates { get; set; }
    }
}
