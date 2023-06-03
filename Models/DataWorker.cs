using DiakontTestTask.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DiakontTestTask.Models
{
    public class DataWorker
    {
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
        public static void CreateRate(string name, DateTime startDate, decimal salary, Position position)
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
