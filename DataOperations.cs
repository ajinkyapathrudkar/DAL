using System.Collections.Generic;
using System.Data;

namespace Kale.Logistics.Framework.Common.DataAccessLayer
{
    /// <summary>
    /// This class provides methods to perform different database related operations
    /// like Insert/Update/Delete and retrieve data.
    /// </summary>
    public class DataAccessObject
    {
        #region Private members
        //public DataAccessObject()
        //{
        //    dbOperations.ConnectionName = ;
        //}

        private DBOperations dbOperations = new DBOperations();

        private bool isTransactionRequired;
        #endregion

        #region Public methods
        /// <summary>
        /// Inserts data into table using specified store procedure and its parameters. 
        /// </summary>
        /// <param name="connectionKey">Connection name from configuration</param>
        /// <param name="spName">Store procedure name</param>
        /// <param name="spParameters">Store procedure parameter collection</param>
        public int InsertData(string connectionKey, string spName, params StoreProcedureParameter[] spParameters)
        {
            try
            {
                dbOperations.ConnectionName = connectionKey;
                dbOperations.SetCommandType(CommandType.StoredProcedure);
                dbOperations.SetCommandText(spName);
                dbOperations.SetSPParameters(spParameters);
                if (isTransactionRequired)
                {
                    dbOperations.BeginTransaction();
                }

                int count = dbOperations.ExecuteNonQuery();

                foreach (StoreProcedureParameter parameter in spParameters)
                {
                    if (!parameter.IsInParameter)
                    {
                        parameter.Value = dbOperations.GetSPOutParameterValue<object>(parameter.ParameterName);
                    }
                }

                return count;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get Store procedures out parameter's valse
        /// </summary>
        /// <typeparam name="T">Typeof out parameter</typeparam>
        /// <param name="parameterName">Name of the out parameter</param>
        /// <returns>Value of out parameter</returns>
        public T GetSPOutParameterValue<T>(string parameterName)
        {
            try
            {
                return dbOperations.GetSPOutParameterValue<T>(parameterName);
            }
            catch
            {

                throw;
            }
        }

        /// <summary>
        /// Update table record using specifed store procedure
        /// </summary>
        /// <param name="connectionKey">Connection name from configuration</param>
        /// <param name="spName">Store procedure name</param>
        /// <param name="spParameters">Store procedure parameter collection</param>
        public void UpdateData(string connectionKey, string spName, params StoreProcedureParameter[] spParameters)
        {
            try
            {
                dbOperations.ConnectionName = connectionKey;
                dbOperations.SetCommandType(CommandType.StoredProcedure);
                dbOperations.SetCommandText(spName);
                dbOperations.SetSPParameters(spParameters);
                if (isTransactionRequired)
                {
                    dbOperations.BeginTransaction();
                }
                dbOperations.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Deletes table record using specifed store procedure
        /// </summary>
        /// <param name="connectionKey">Connection name from configuration</param>
        /// <param name="spName">Store procedure name</param>
        /// <param name="spParameters">Store procedure parameter collection</param>
        public void DeleteData(string connectionKey, string spName, params StoreProcedureParameter[] spParameters)
        {
            try
            {
                dbOperations.ConnectionName = connectionKey;
                dbOperations.SetCommandType(CommandType.StoredProcedure);
                dbOperations.SetCommandText(spName);
                dbOperations.SetSPParameters(spParameters);
                if (isTransactionRequired)
                {
                    dbOperations.BeginTransaction();
                }
                dbOperations.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionKey">Connection name from configuration</param>
        /// <param name="spName">Store procedure name</param>
        /// <param name="spParameters">Store procedure parameter collection</param>
        /// <returns></returns>
        public T RetrieveDataReader<T>(string connectionKey, string spName, params StoreProcedureParameter[] spParameters)
        {
            try
            {
                dbOperations.ConnectionName = connectionKey;
                dbOperations.SetCommandType(CommandType.StoredProcedure);
                dbOperations.SetCommandText(spName);
                dbOperations.SetSPParameters(spParameters);
                return dbOperations.ExecuteReader<T>();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionKey">Connection name from configuration</param>
        /// <param name="spName">Store procedure name</param>
        /// <param name="spParameters">Store procedure parameter collection</param>
        /// <returns></returns>
        public DataSet RetrieveDataSet(string connectionKey, string spName, params StoreProcedureParameter[] spParameters)
        {
            try
            {
                dbOperations.ConnectionName = connectionKey;
                dbOperations.SetCommandType(CommandType.StoredProcedure);
                dbOperations.SetCommandText(spName);
                dbOperations.SetSPParameters(spParameters);
                return dbOperations.ExecuteDataSet();
            }
            catch
            {
                throw;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="connectionKey">Connection name from configuration</param>
        /// <param name="spName">Store procedure name</param>
        /// <param name="spParameters">Store procedure parameter collection</param>
        /// <returns></returns>
        public DataTable RetrieveDataTable(string connectionKey, string tableName, string spName, params StoreProcedureParameter[] spParameters)
        {
            try
            {
                DataSet ds = RetrieveDataSet(connectionKey, spName, spParameters);
                if (ds.Tables.Count > 0 && ds.Tables.Contains(tableName))
                {
                    return ds.Tables[tableName];
                }
            }
            catch
            {
                throw;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionKey">Connection name from configuration</param>
        /// <param name="spName">Store procedure name</param>
        /// <param name="spParameters">Store procedure parameter collection</param>
        /// <param name="tableIndex"></param>
        /// <returns></returns>
        public DataTable RetrieveDataTable(string connectionKey, int tableIndex, string spName, params StoreProcedureParameter[] spParameters)
        {
            try
            {
                DataSet ds = RetrieveDataSet(connectionKey, spName, spParameters);
                if (ds.Tables.Count > 0 && ds.Tables.Count > tableIndex)
                {
                    return ds.Tables[tableIndex];
                }
            }
            catch
            {
                throw;
            }
            return null;
        }

        /// <summary>
        /// Executes the query, and returns the first column 
        /// of the first row in the result set returned by the query. 
        /// Additional columns or rows are ignored.
        /// </summary>
        /// <typeparam name="T">Type of return value</typeparam>
        /// <param name="connectionKey">Connection name from configuration</param>
        /// <param name="spName">Store procedure name</param>
        /// <param name="spParameters">Store procedure parameter collection</param>
        /// <returns></returns>
        public T RetrieveScalar<T>(string connectionKey, string spName, params StoreProcedureParameter[] spParameters)
        {
            try
            {
                dbOperations.ConnectionName = connectionKey;
                dbOperations.SetCommandType(CommandType.StoredProcedure);
                dbOperations.SetCommandText(spName);
                dbOperations.SetSPParameters(spParameters);
                if (isTransactionRequired)
                {
                    dbOperations.BeginTransaction();
                }
                return dbOperations.ExecuteScalar<T>();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Begin database transaction scope
        /// </summary>
        public void StartTransaction()
        {
            isTransactionRequired = true;
        }

        /// <summary>
        /// Commits database transaction
        /// </summary>
        public void EndTransaction()
        {
            try
            {
                dbOperations.CommitTransaction();
            }
            catch
            {
                throw;
            }
            finally
            {
                isTransactionRequired = false;
            }
        }

        /// <summary>
        /// Executes rollback operation
        /// </summary>
        public void RevertTransaction()
        {
            try
            {
                dbOperations.RollbackTransaction();
            }
            catch
            {
                throw;
            }
            finally
            {
                isTransactionRequired = false;
            }
        }

        public void Update(string connectionKey, DataSet dataset, string tableName, string insertSPName, IEnumerable<StoreProcedureParameter> insertSPParameters,
            string updateSPName, IEnumerable<StoreProcedureParameter> updateSPParameters,
            string deleteSPName, IEnumerable<StoreProcedureParameter> deleteSPParameters)
        {
            try
            {
                dbOperations.ConnectionName = connectionKey;
                dbOperations.SetCommandType(CommandType.StoredProcedure);

                if (isTransactionRequired)
                {
                    dbOperations.BeginTransaction();
                }

                dbOperations.UpdateDataSet(dataset, tableName, insertSPName, insertSPParameters,
                    updateSPName, updateSPParameters,
                    deleteSPName, deleteSPParameters);
            }
            catch
            {
                throw;
            }
        }
        #endregion
    }
}
