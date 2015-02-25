namespace Nagger.Data
{
    using System;
    using System.Data.SQLite;

    public static class SQLiteDataReaderExtensions
    {
        public static T Get<T>(this SQLiteDataReader reader, string columnName)
        {
            var value = reader.GetValue(reader.GetOrdinal(columnName));
            return (T) Convert.ChangeType(value, typeof (T));
        }
    }
}
