using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiakontTestTask.Models
{
    public class StaffingTableElement
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int EmployeesCount { get; set; }
        public int PositionId { get; set; }
        public virtual Position Position { get; set; }
        [NotMapped]
        public Position StaffingTableElementPosition
        {
            get
            {
                return DataWorker.GetPositionById(PositionId);
            }
        }
        public int DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        [NotMapped]
        public Department StaffingTableElementDepartment
        {
            get
            {
                return DataWorker.GetDepartmentById(DepartmentId);
            }
        }
    }
}
