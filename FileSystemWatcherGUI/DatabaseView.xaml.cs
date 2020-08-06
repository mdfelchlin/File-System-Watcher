using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FileSystemWatcherGUI
{
    /// <summary>
    /// Interaction logic for DatabaseView.xaml
    /// </summary>
    public partial class DatabaseView : Window
    {
        private DatabaseManager _databaseManager;

        public DatabaseView()
        {
            InitializeComponent();
            _databaseManager = new DatabaseManager();
            ShowDatabaseContents.ItemsSource = _databaseManager.GetDatabaseContents();
        }

        private void clearMenuOption_onMouseClick(object sender, RoutedEventArgs e)
        {
            if(_databaseManager.GetDatabaseContents().Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure that you want to clear the database?", "Clear Database", MessageBoxButton.YesNo);

                if (result == MessageBoxResult.Yes)
                    _databaseManager.ClearDatabase();
            }
            ShowDatabaseContents.ItemsSource = _databaseManager.GetDatabaseContents();
        }
    }
}
