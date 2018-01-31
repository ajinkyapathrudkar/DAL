using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Kale.Logistics.Framework.Common.DataAccessLayer
{
    /// <summary>
    /// DataReader extension method, this will create a method on any 
    /// IDataReader that has access to this static class
    /// </summary>
    public static class DataReaderExtensions
    {
        /// <summary>
        /// Checks whether field name exists in data reader or not
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static bool FieldExists(this IDataReader reader, string fieldName)
        {
            reader.GetSchemaTable().DefaultView.RowFilter = string.Format("ColumnName= '{0}'", fieldName);
            return (reader.GetSchemaTable().DefaultView.Count > 0);
        }
    }
}
