namespace Nagger.Data
{
    using System;
    using System.Data.SQLite;
    using System.IO;

    public abstract class LocalBaseRepository
    {
        protected SQLiteConnection GetConnection()
        {
            const string naggerDb = "\\nagger.ndb";
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var dataSource = appData + "\\Nagger\\" + naggerDb;

            if (!File.Exists(dataSource))
            {
                Directory.CreateDirectory(appData + "\\Nagger");
                File.Copy("."+naggerDb, dataSource);
            }

            // this still needs work. the db needs to already exist... it should be setup already.
            // we can take a dependency on it
            var cnn = new SQLiteConnection("Data Source="+dataSource);
            cnn.Open();

            // Encrypts the database. The connection remains valid and usable afterwards.
            //const string pw = "nAgg3R48529Pw0rdRoxIt5907";
            //cnn.ChangePassword(pw);

            return cnn;
        }
    }
}
