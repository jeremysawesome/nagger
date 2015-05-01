using System;
using System.Data.SQLite;

namespace Nagger.Data
{
    public static class SQLiteDataReaderExtensions
    {
        public static T Get<T>(this SQLiteDataReader reader, string columnName)
        {
            var value = reader.GetValue(reader.GetOrdinal(columnName));
            return (T) Convert.ChangeType(value, typeof (T));
        }
    }
}