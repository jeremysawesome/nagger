namespace Nagger.Data
{
    using Interfaces;

    public class SettingsRepository : LocalBaseRepository, ISettingsRepository
    {
        public T GetSetting<T>(string name)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = "SELECT Value FROM Settings WHERE Name = @Name";
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Name", name);

                using (var reader = cmd.ExecuteReader())
                {
                    return !reader.Read() ? default(T) : reader.Get<T>("Value");
                }
            }
        }

        public void SaveSetting(string name, string value)
        {
            using (var cnn = GetConnection())
            using (var cmd = cnn.CreateCommand())
            {
                cmd.CommandText = @"INSERT OR REPLACE INTO Settings (Name, Value)
                                VALUES (@Name, @Value)";

                cmd.Prepare();
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Value", value);

                cmd.ExecuteNonQuery();
            }
        }
    }
}
