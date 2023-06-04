using DiakontTestTask.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace DiakontTestTask
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static ListView AllRatesView;
        public static ListView AllStaffingTableElementsView;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new DataManageVM();

            AllStaffingTableElementsView = ViewAllStaffingTableElements;
            AllRatesView = ViewAllRates;
        }
    }
}
