using DiakontTestTask.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;

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

        // метод создания отчёта
        public static List<ReportElement> CreateReport(DateTime startDate, DateTime endDate)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var rates = db.Rates.ToList();
                var staffingTableElements = db.StaffingTableElements.ToList();

                // добавляем в список позиций даты окончания позиций (у действующих позиций будет Today)
                foreach (var rate in rates)
                {
                    rate.EndDate = rates
                        .Where(e => e.StartDate > rate.StartDate && e.PositionId == rate.PositionId)
                        .DefaultIfEmpty(new Rate { StartDate = DateTime.Today })
                        .Min(e => e.StartDate);
                }

                // добавляем в список штатного расписания даты окончания позиций (у действующих позиций будет Today)
                foreach (var staffingTableElement in staffingTableElements)
                {
                    staffingTableElement.EndDate = staffingTableElements
                        .Where(e => e.StartDate > staffingTableElement.StartDate && e.PositionId == staffingTableElement.PositionId)
                        .DefaultIfEmpty(new StaffingTableElement { StartDate = DateTime.Today })
                        .Min(e => e.StartDate);
                }

                // создаём перечень элементов объединённой таблицы
                var reportElements = staffingTableElements.Join(rates,
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

                // заполняем список reportData
                var reportData = new List<ReportElement>();
                foreach (var reportElement in reportElements)
                {
                    reportData.Add(new ReportElement
                    {
                        DepartmentId = reportElement.tempDepartmentId,
                        StartDate = reportElement.tempStartDate,
                        EndDate = rates.First(e => e.Id == reportElement.Id).EndDate,
                        FOT = reportElement.tempOveralSalary
                    });
                }

                // усекаем список элементов отчёта введёнными через интерфейс датами
                List<ReportElement> filteredreportData = reportData
                    .Where(e => e.StartDate <= endDate && e.EndDate >= startDate)
                    .Select(e => new ReportElement
                    {
                        DepartmentId = e.DepartmentId,
                        StartDate = e.StartDate < startDate ? startDate : e.StartDate,
                        EndDate = e.EndDate > endDate ? endDate : e.EndDate,
                        FOT = e.FOT
                    })
                    .ToList(); 

                return filteredreportData;
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
