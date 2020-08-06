using System;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace FileSystemWatcherGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private FileSystemWatcher _fileWatcher;
        private ObservableCollection<FileInformation> _filesChanged;
        private DatabaseManager _databaseManager;

        public MainWindow()
        {
            InitializeComponent();
            _filesChanged = new ObservableCollection<FileInformation>();
            _databaseManager = new DatabaseManager();

            DisplayFileChanges.ItemsSource = _filesChanged;

            _fileWatcher = new FileSystemWatcher
            {
                NotifyFilter = NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.FileName
                                | NotifyFilters.DirectoryName
            };

            _fileWatcher.Changed += FileChanged;
            _fileWatcher.Created += FileChanged;
            _fileWatcher.Deleted += FileChanged;
            _fileWatcher.Renamed += FileChanged;

            _fileWatcher.Path = "C:/";
            _fileWatcher.Filter = "*";
            MenuStopOption.IsEnabled = false;
        }

        private void SetFileWatcherDirectory()
        {
            if (DirectoryField.Text != null && Directory.Exists(DirectoryField.Text))
            {
                _fileWatcher.Path = DirectoryField.Text;
            }
        }

        private void start()
        {
            DisableControls();
            _fileWatcher.Filter = "*" + ExtensionField.Text;
            _fileWatcher.IncludeSubdirectories = IncludeSubdirectiesCB.IsChecked ?? false;
            _fileWatcher.EnableRaisingEvents = true;
            MenuStopOption.IsEnabled = true;
            StopButton.IsEnabled = true;
            SetFileWatcherDirectory();
        }

        private void startButton_onMouseClick(object sender, RoutedEventArgs e)
        {
            start();
        }

        private void startMenuOption_onMouseClick(object sender, RoutedEventArgs e)
        {
            start();
        }

        private void stop()
        {
            EnableControls();
            _fileWatcher.EnableRaisingEvents = false;
            PromptToAddToDataBase();
            MenuStopOption.IsEnabled = false;
            StopButton.IsEnabled = false;
        }

        private void stopButton_onMouseClick(object sender, RoutedEventArgs e)
        {
            stop();
        }

        private void stopMenuOption_onClick(object sender, RoutedEventArgs e)
        {
            stop();
        }

        private void write()
        {
            if (_filesChanged.Count > 0)
            {
                _databaseManager.AddToDatabase(_filesChanged);
                MessageBox.Show("Items Saved to the database", "Success");

                _filesChanged.Clear();
            }
        }

        private void writeButton_onMouseClick(object sender, RoutedEventArgs e)
        {
            write();
        }

        private void writeMenuOption_onClick(object sender, RoutedEventArgs e)
        {
            write();
        }

        private void queryDatabase_onMouseClick(object sender, RoutedEventArgs e)
        {
            DatabaseView databaseView = new DatabaseView();
            databaseView.Show();
        }
        
        private void MenuAboutOption_onClick(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "Usage:" +
                "\n The main goal of this program is to be able to see when" +
                "\n Files and directories are Changed, Renamed, Deleted, " +
                "\n and created and then report to the user when those events" +
                "\n happen." +
                "\n\n VERSION: 1.0.0" +
                "\n\n AUTHOR: Mark Felchlin",
                "About"
                );
        }

        private void windowClose_onClick(object sender, System.ComponentModel.CancelEventArgs e)
        {
            PromptToAddToDataBase();
            _databaseManager.OnExit();
        }

        private void FileChanged(object source, FileSystemEventArgs e)
        {
            Dispatcher.BeginInvoke(
                (Action)(() =>
                {
                    var info = new FileInformation()
                                {
                                    FileName = e.Name,
                                    Extension = Path.GetExtension(e.FullPath),
                                    Event = e.ChangeType.ToString(),
                                    Path = e.FullPath,
                                    Timestamp = DateTime.Now.ToString()
                                };
                    _filesChanged.Add(info);
                }));
        }

        private void PromptToAddToDataBase()
        {
            if (_filesChanged.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Would you like to save the contents to the database?", "Save?", MessageBoxButton.YesNo);

                if(result == MessageBoxResult.Yes)
                    _databaseManager.AddToDatabase(_filesChanged);
                
                _filesChanged.Clear();
            }
        }

        private void DisableControls()
        {
            DirectoryField.IsEnabled = false;
            IncludeSubdirectiesCB.IsEnabled = false;
            ExtensionField.IsEnabled = false;
            MenuStartOption.IsEnabled = false;
            StartButton.IsEnabled = false;
        }

        private void EnableControls()
        {
            DirectoryField.IsEnabled = true;
            IncludeSubdirectiesCB.IsEnabled = true;
            ExtensionField.IsEnabled = true;
            MenuStartOption.IsEnabled = true;
            StartButton.IsEnabled = true;
        }

        private void exitMenuOption_onClick(object sender, RoutedEventArgs e)
        {
            PromptToAddToDataBase();
            _databaseManager.OnExit();
        }
    }
}
