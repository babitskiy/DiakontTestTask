using DiakontTestTask.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Documents;

namespace DiakontTestTask.ViewModels
{
    public class DataManageVM : INotifyPropertyChanged
    {
        // все ставки
        private List<Rate> allRates = DataWorker.GetAllRates();
        public List<Rate> AllRates
        {
            get { return allRates; }
            set
            {
                allRates = value;
                NotifyPropertyChanged("AllRates");
            }
        }

        // всё штатное расписание
        private List<StaffingTableElement> allStaffingTableElements = DataWorker.GetAllStaffingTableElements();
        public List<StaffingTableElement> AllStaffingTableElements
        {
            get { return allStaffingTableElements; }
            set
            {
                allStaffingTableElements = value;
                NotifyPropertyChanged("AllStaffingTableElements");
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
