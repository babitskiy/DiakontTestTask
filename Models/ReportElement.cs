using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DiakontTestTask.Models
{
    public class ReportElement
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal FOT { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        [NotMapped]
        public Department ReportElementDepartment
        {
            get
            {
                return DataWorker.GetDepartmentById(DepartmentId);
            }
        }
    }
}
