using System.Collections.Generic;
using System.Windows.Documents;

namespace DiakontTestTask.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Position> Positions { get; set; }
    }
}
