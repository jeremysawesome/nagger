namespace Nagger.Data
{
    using System.Data.SQLite;

    public abstract class LocalBaseRepository
    {
        protected SQLiteConnection GetConnection()
        {
            // this still needs work. the db needs to already exist... it should be setup already.
            // we can take a dependency on it
            var cnn = new SQLiteConnection("Data Source=.//nagger.ndb");
            cnn.Open();

            // Encrypts the database. The connection remains valid and usable afterwards.
            //const string pw = "nAgg3R48529Pw0rdRoxIt5907";
            //cnn.ChangePassword(pw);

            return cnn;
        }
    }
}
