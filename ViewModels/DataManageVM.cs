﻿using DiakontTestTask.Models;
using DiakontTestTask.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Documents;

namespace DiakontTestTask.ViewModels
{
    public class DataManageVM : INotifyPropertyChanged
    {
        // все отделы
        private List<Department> allDepartments = DataWorker.GetAllDepartments();
        public List<Department> AllDepartments
        {
            get { return allDepartments; }
            set
            {
                allDepartments = value;
                NotifyPropertyChanged("AllDepartments");
            }
        }

        // все позиции
        private List<Position> allPositions = DataWorker.GetAllPositions();
        public List<Position> AllPositions
        {
            get { return allPositions; }
            set
            {
                allPositions = value;
                NotifyPropertyChanged("AllPositions");
            }
        }

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


        #region COMMANDS TO ADD
        // свойства 

        // свойства ставки
        public DateTime RateStartDate { get; set; }
        public decimal RateSalary { get; set; }
        public Position RatePosition { get; set; }

        // метод добавления ставки
        private RelayCommand addNewRate;
        public RelayCommand AddNewRate
        {
            get
            {
                return addNewRate ?? new RelayCommand(obj =>
                {
                    DataWorker.CreateRate(RateStartDate, RateSalary, RatePosition);
                    string resultStr = "Ставка добавлена";
                    UpdateAllDataView();

                    ShowMessageToUser(resultStr);
                });
            }
        }
        #endregion

        #region UPDATE VIEWS
        private void UpdateAllDataView()
        {
            UpdateAllRatesView();
            UpdateAllStaffingTableElementsView();
        }
        private void UpdateAllRatesView()
        {
            AllRates = DataWorker.GetAllRates();
            MainWindow.AllRatesView.ItemsSource = null;
            MainWindow.AllRatesView.Items.Clear();
            MainWindow.AllRatesView.ItemsSource = AllRates;
            MainWindow.AllRatesView.Items.Refresh();
        }
        private void UpdateAllStaffingTableElementsView()
        {
            AllStaffingTableElements = DataWorker.GetAllStaffingTableElements();
            MainWindow.AllStaffingTableElementsView.ItemsSource = null;
            MainWindow.AllStaffingTableElementsView.Items.Clear();
            MainWindow.AllStaffingTableElementsView.ItemsSource = AllStaffingTableElements;
            MainWindow.AllStaffingTableElementsView.Items.Refresh();
        }
        #endregion

        private void ShowMessageToUser(string message)
        {
            MessageView messageView = new MessageView(message);
            SetCenterPositionAndOpen(messageView);
        }
        private void SetCenterPositionAndOpen(Window window)
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.ShowDialog();
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