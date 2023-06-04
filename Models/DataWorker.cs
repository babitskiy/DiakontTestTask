using DiakontTestTask.Models.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DiakontTestTask.Models
{
    public class DataWorker
    {
        // получить все отделы
        public static List<Department> GetAllDepartments()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var result = db.Departments.ToList();
                return result;
            }
        }

        // получить все позиции
        public static List<Position> GetAllPositions()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var result = db.Positions.ToList();
                return result;
            }
        }

        // получить все ставки
        public static List<Rate> GetAllRates()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.Rates.ToList();
            }
        }

        // получить все элементы штатного расписания
        public static List<StaffingTableElement> GetAllStaffingTableElements()
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                return db.StaffingTableElements.ToList();
            }
        }

        // создать ставку
        public static void CreateRate(DateTime startDate, decimal salary, Position position)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Rate newRate = new Rate
                {
                    StartDate = startDate,
                    Salary = salary,
                    PositionId = position.Id
                };
                db.Rates.Add(newRate);
                db.SaveChanges();
            }
        }

        // создать элемент штатного рассписания
        public static void CreateStaffingTableElement (DateTime startDate, int employeesCount, Position position, Department department)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                StaffingTableElement newStaffingTableElement = new StaffingTableElement
                {
                    StartDate = startDate,
                    EmployeesCount = employeesCount,
                    PositionId = position.Id,
                    DepartmentId = department.Id
                };
                db.StaffingTableElements.Add(newStaffingTableElement);
                db.SaveChanges();
            }
        }

        // создание отчёта
        public static List<ReportElement> CreateReport(DateTime startDate, DateTime endDate)
        {
            string sql = @"select 
	                        s.department, 
	                        s.position,
	                        r.startDate, 
	                        s.startDate, 
	                        r.salary * s.employee_count 
                        from dbo.Rates r
	                        join dbo.StaffingTableElements s on r.position = s.position AND r.startDate = s.startDate
                        where r.startDate >= '01.01.2015' and r.startDate <= '01.07.2015'";
            using (ApplicationContext db = new ApplicationContext())
            {
                var rep = db.StaffingTableElements.Join(db.Rates,
                    s => s.PositionId,
                    r => r.PositionId,
                    (s, r) => new
                    {
                        tempDepartment = s.Department,
                        tempPosition = s.PositionId,
                        tempStartDate = s.StartDate,
                        tempOveralSalary = r.Salary * s.EmployeesCount
                    });
            }
            return new List<ReportElement> { 
                new ReportElement
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    DepartmentId = 1,
                    FOT = 100500
                }
            };
        }

        // получение должности по id должности
        public static Position GetPositionById(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Position pos = db.Positions.FirstOrDefault(p => p.Id == id);
                return pos;
            }
        }

        // получение отдела по id отдела
        public static Department GetDepartmentById(int id)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                Department dep = db.Departments.FirstOrDefault(d => d.Id == id);
                return dep;
            }
        }
    }
}
