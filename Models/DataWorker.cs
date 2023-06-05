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
        /*public static List<ReportElement> CreateReportOld(DateTime startDate, DateTime endDate)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var rates = db.Rates.ToList();
                var staffingTableElements = db.StaffingTableElements.ToList();

                // why it doesnt work?????
                *//*foreach (var rate in Rates)
                {
                    rate.EndDate = Rates
                        .Where(e => e.StartDate > rate.StartDate && e.Position == rate.Position)?
                        .Min(e => e.StartDate) ?? DateTime.MaxValue;
                }*//*

                // добавляем в список позиций даты окончания позиций (у действующих позиций будет Now)
                foreach (var rate in rates)
                {
                    rate.EndDate = rates
                        .Where(e => e.StartDate > rate.StartDate && e.Position == rate.Position)
                        .DefaultIfEmpty(new Rate { StartDate = DateTime.Now })
                        .Min(e => e.StartDate);
                }

                // добавляем в список штатного расписания даты окончания позиций (у действующих позиций будет Now)
                foreach (var staffingTableElement in staffingTableElements)
                {
                    staffingTableElement.EndDate = staffingTableElements
                        .Where(e => e.StartDate > staffingTableElement.StartDate && e.Position == staffingTableElement.Position)
                        .DefaultIfEmpty(new StaffingTableElement { StartDate = DateTime.Now })
                        .Min(e => e.StartDate);
                }

                // создаём перечень элементов объединённой таблицы
                var repEls = db.StaffingTableElements.Join(db.Rates,
                    s => new { s.PositionId, s.StartDate },
                    r => new { r.PositionId, r.StartDate },
                    (s, r) => new
                    {
                        Id = r.Id,
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
                        EndDate = rates.First(e => e.Id == repEl.Id).EndDate,
                        FOT = repEl.tempOveralSalary
                    });
                }
                return result;
            }
        }*/

        public static List<ReportElement> CreateReport(DateTime startDate, DateTime endDate)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                // очищаем таблицу ReportElements
                db.Database.ExecuteSqlRaw("TRUNCATE TABLE ReportElements");

                // заполняем в таблице Rates столбец EndDate
                db.Database.ExecuteSqlRaw("UPDATE Rates SET EndDate = ( SELECT MIN(StartDate) FROM Rates as s WHERE s.StartDate > Rates.StartDate AND s.PositionId = Rates.PositionId)");

                // заменяем null в поле EndDate на текущую дату
                db.Database.ExecuteSqlRaw("UPDATE Rates SET EndDate = GETDATE() WHERE EndDate IS NULL");

                // заполняем в таблице StaffingTableElements столбец EndDate
                db.Database.ExecuteSqlRaw("UPDATE StaffingTableElements SET EndDate = ( SELECT MIN(StartDate) FROM StaffingTableElements as s WHERE s.StartDate > StaffingTableElements.StartDate AND s.PositionId = StaffingTableElements.PositionId)");

                // заменяем null в поле EndDate на текущую дату в таблице staffingTableElements
                db.Database.ExecuteSqlRaw("UPDATE StaffingTableElements SET EndDate = GETDATE() WHERE EndDate IS NULL");

                //var tempResult = db.Rates.FromSqlRaw("SELECT s.DepartmentId, r.StartDate, r.EndDate, SUM(r.Salary * s.EmployeesCount) AS OveralSalary FROM Rates r JOIN StaffingTableElements s ON r.PositionId = s.PositionId AND r.StartDate >= s.StartDate AND r.EndDate <= s.EndDate GROUP BY s.DepartmentId, r.StartDate, r.EndDate ORDER BY s.DepartmentId, r.StartDate, r.EndDate").ToList();

                var rates = db.Rates.ToList();
                var staffingTableElements = db.StaffingTableElements.ToList();

                /*var query = (from r in rates
                             join s in staffingTableElements on new { r.PositionId, r.StartDate, r.EndDate } equals new { s.PositionId, s.StartDate, s.EndDate }
                             group new { s.DepartmentId, r.StartDate, r.EndDate, r.Salary, s.EmployeesCount } by new { s.DepartmentId, r.StartDate, r.EndDate } into g
                             select new
                             {
                                 g.Key.DepartmentId,
                                 g.Key.StartDate,
                                 g.Key.EndDate,
                                 TotalSalary = g.Sum(x => x.Salary * x.EmployeesCount)
                             }).OrderBy(x => x.DepartmentId).ThenBy(x => x.StartDate).ThenBy(x => x.EndDate);*/

                db.Database.ExecuteSqlRaw("INSERT INTO ReportElements (DepartmentId, StartDate, EndDate, FOT) SELECT s.DepartmentId, r.StartDate, r.EndDate, SUM(r.Salary * s.EmployeesCount) AS OveralSalary FROM Rates r JOIN StaffingTableElements s ON r.PositionId = s.PositionId AND r.StartDate >= s.StartDate AND r.EndDate <= s.EndDate GROUP BY s.DepartmentId, r.StartDate, r.EndDate ORDER BY s.DepartmentId, r.StartDate, r.EndDate");

                var report = db.ReportElements.ToList();

                List<ReportElement> filteredReport = report
                    .Where(e => e.StartDate <= endDate && e.EndDate >= startDate) // элемент попадает в период
                    .Select(e => new ReportElement // создаем новый объект расходов
                    {
                        DepartmentId = e.DepartmentId,
                        StartDate = e.StartDate < startDate ? startDate : e.StartDate, // если дата начала расходов раньше заданной даты начала периода, то берем заданную дату начала периода
                        EndDate = e.EndDate > endDate ? endDate : e.EndDate, // если дата окончания расходов позже заданной даты окончания периода, то берем заданную дату окончания периода
                        FOT = e.FOT
                    })
                    .ToList();

                return filteredReport;
            }
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
