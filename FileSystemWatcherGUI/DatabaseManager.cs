using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;

namespace FileSystemWatcherGUI
{
    public class DatabaseManager
    {
        private SQLiteConnection sqlite_conn;
        private SQLiteCommand sqlite_cmd;
        private SQLiteDataReader sqlite_datareader;
        public DatabaseManager()
        {
            sqlite_conn = new SQLiteConnection("Data Source=filewatcher.db;Version=3;New=True;Compress=True;");
            sqlite_conn.Open();
            sqlite_cmd = sqlite_conn.CreateCommand();

            sqlite_cmd.CommandText = "CREATE TABLE if not exists FileInfo (FileName varchar(100), Extension varchar(10), Path varchar(200), Event varchar(30), Timestamp varchar (30));";
            sqlite_cmd.ExecuteNonQuery();
        }

        public void AddToDatabase(ObservableCollection<FileInformation> changesMade)
        {
            foreach (FileInformation fileInfo in changesMade)
            {
                sqlite_cmd = sqlite_conn.CreateCommand();
                sqlite_cmd.CommandText = 
                    $"INSERT INTO FileInfo (FileName, Extension, Path, Event, Timestamp) " +
                    $"VALUES ('{fileInfo.FileName}', '{fileInfo.Extension}', '{fileInfo.Path}', '{fileInfo.Event}', '{fileInfo.Timestamp}');";
                sqlite_cmd.ExecuteNonQuery();
            }
        }

        public void ClearDatabase()
        {
            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "DELETE FROM FileInfo"; // DELETE statement
            sqlite_cmd.ExecuteNonQuery();
        }

        public ObservableCollection<FileInformation> GetDatabaseContents()
        {
            ObservableCollection<FileInformation> databaseContents = new ObservableCollection<FileInformation>();

            sqlite_cmd = sqlite_conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM FileInfo";
            sqlite_datareader = sqlite_cmd.ExecuteReader();

            while (sqlite_datareader.Read())
            {
                databaseContents.Add(new FileInformation()
                {
                    FileName = (string) sqlite_datareader["FileName"],
                    Extension = (string) sqlite_datareader["Extension"],
                    Path = (string) sqlite_datareader["Path"],
                    Event = (string) sqlite_datareader["Event"],
                    Timestamp = (string) sqlite_datareader["Timestamp"]
                });
            }

            return databaseContents;
        }

        public void OnExit()
        {
            sqlite_conn.Close();
        }
    }
}
