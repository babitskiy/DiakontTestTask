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
            using (ApplicationContext db = new ApplicationContext())
            {
                var repEls = db.StaffingTableElements.Join(db.Rates,
                    s => new { s.PositionId, s.StartDate },
                    r => new { r.PositionId, r.StartDate },
                    (s, r) => new
                    {
                        tempDepartmentId = s.DepartmentId,
                        tempPosition = s.PositionId,
                        tempStartDate = s.StartDate,
                        tempOveralSalary = r.Salary * s.EmployeesCount
                    });

                var result = new List<ReportElement>();

                foreach (var repEl in repEls)
                {
                    result.Add(new ReportElement
                    {
                        DepartmentId = repEl.tempDepartmentId,
                        StartDate = repEl.tempStartDate,
                        FOT = repEl.tempOveralSalary
                    });
                }
                return result;
            }
            /*return new List<ReportElement> { 
                new ReportElement
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    DepartmentId = 1,
                    FOT = 100500
                }
            };*/
        }
        public static List<ReportElement> CreateReport2(DateTime startDate, DateTime endDate)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var Rates = db.Rates.ToList();

                // why it doesnt work?????
                /*foreach (var rate in Rates)
                {
                    rate.EndDate = Rates
                        .Where(e => e.StartDate > rate.StartDate && e.Position == rate.Position)?
                        .Min(e => e.StartDate) ?? DateTime.MaxValue;
                }*/

                // добавляем в список позиций даты окончания позиций (у действующих позиций будет Now)
                foreach (var rate in Rates)
                {
                    rate.EndDate = Rates
                        .Where(e => e.StartDate > rate.StartDate && e.Position == rate.Position)
                        .DefaultIfEmpty( new Rate { StartDate = DateTime.Now })
                        .Min(e => e.StartDate);
                }

                // создаём перечень элементов объединённой таблицы
                var repEls = db.StaffingTableElements.Join(db.Rates,
                    s => new { s.PositionId, s.StartDate },
                    r => new { r.PositionId, r.StartDate },
                    (s, r) => new
                    {
                        Id = s.Id,
                        tempDepartmentId = s.DepartmentId,
                        tempPosition = s.PositionId,
                        tempStartDate = s.StartDate,
                        tempOveralSalary = r.Salary * s.EmployeesCount
                    });

                var result = new List<ReportElement>();

                foreach (var repEl in repEls)
                {
                    result.Add(new ReportElement
                    {
                        DepartmentId = repEl.tempDepartmentId,
                        StartDate = repEl.tempStartDate,
                        EndDate = Rates.First(e => e.Id == repEl.Id).EndDate,
                        FOT = repEl.tempOveralSalary
                    });
                }
                return result;
            }
            /*return new List<ReportElement> { 
                new ReportElement
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    DepartmentId = 1,
                    FOT = 100500
                }
            };*/
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
