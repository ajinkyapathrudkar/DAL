using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Microsoft.Practices.EnterpriseLibrary.Data;


namespace Kale.Logistics.Framework.Common.DataAccessLayer
{


    /// <summary>
    /// Represents methods of database operations
    /// </summary>
    public interface IDBOperations
    {
        /* Return result of any database operation depends on four operations
         * Execution of SQL Command 
         * - will not return any records like insert or delete operations
         * - will return result of select command with Data Set object
         * - will return result of select command with data reader object
         * - will return result of sigle value from select command
         */

        // TODO: There will be more methods here related to 
        // lazy reader/writer once the dto and data objects are 
        // placed in proper location and compilation is error-free.

        int ExecuteNonQuery();
        DataSet ExecuteDataSet();
        T ExecuteReader<T>();
        T ExecuteScalar<T>();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }


    /// <summary>
    /// Performs Database operations
    /// </summary>
    public class DBOperations : IDBOperations
    {
        #region Private Members

        // Name of connection from web.config file
        private string connectionName;
        // Instance of Microsoft.Practices.EnterpriseLibrary.Data.Database class 
        private Database dbInstance;
        // Instance of System.Data.Common.DbCommand class
        private DbCommand dbCommand;
        // Array of stored procedure's parameters, these parameters are initialized from constructor of class
        private Collection<StoreProcedureParameter> spParams;
        // Stored procedure out parameter values
        private Hashtable spOutParams;
        // SQL Command text
        private string commandText;
        // Type of SQL command
        private CommandType commandType = CommandType.Text;
        // Instance of System.Data.Common.DbTransaction class
        private DbTransaction dbTransaction;
        // bool flag if begin, commit or rollback transaction required.
        private bool isTransactionReqd;
        // Represents name of SP's  default return parameter
        private const string RETVAL = "@RETURN_VALUE";
        // Exception message for invalid command type
        private const string INVCMDTYPE = "Invalid SQL command type '{0}'.";
        // Exception message for invalid data type
        private const string INVDATATYPE = "Dot Net datatype mapping for SQL type '{0}' could not found.";
        // Selection expression for SP's
        private const string SPINFO = "spname='{0}'";
        // Selection expression for SP output parameters
        private const string SPOUTPARAMINFO = "spname='{0}' AND is_output=true";
        // Recored sorting order expression
        private const string SORTEXP = "pos ASC";
        // Command timeout
        private int commandTimeout = 30;

        #endregion

        #region Properties

        /// <summary>
        /// Get or Set connection name
        /// </summary>
        public string ConnectionName
        {
            get
            {
                return connectionName;
            }
            set
            {
                connectionName = value;
            }
        }


        /// <summary>
        /// Set SQL command Type
        /// </summary>
        /// <param name="CommandType">value to be set</param>
        public void SetCommandType(CommandType cmdType)
        {
            this.commandType = cmdType;
        }

        /// <summary>
        /// Set SQL command text
        /// </summary>
        /// <param name="cmdTxt">value to be set</param>
        public void SetCommandText(string cmdTxt)
        {
            this.commandText = cmdTxt;
        }

        ///// <summary>
        ///// Set stored procedur's parameters
        ///// </summary>
        //public Collection<StoreProcedureParameters> SPParameters
        //{
        //    set
        //    {
        //        spParams = value;

        //    }
        //}

        public void SetSPParameters(params StoreProcedureParameter[] spParameters)
        {

            if (spParams != null && spParameters != null)
            {
                if (spParams.Count > 0)
                {
                    spParams.Clear();
                }
                for (int cnt = 0; cnt < spParameters.Length; cnt++)
                {
                    spParams.Add(spParameters[cnt]);
                }
            }
        }

        /// <summary>
        /// Set time out for command object
        /// </summary>
        /// <param name="cmdTimeout">timeout value to be set</param>
        public void SetCommandTimeout(int cmdTimeout)
        {
            this.commandTimeout = cmdTimeout;
        }
        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public DBOperations()
        {
            spParams = new Collection<StoreProcedureParameter>();
        }

        /// <summary>
        /// To execute command string and stored procedure from DB
        /// </summary>
        /// <param name="connectionName">Name of connection from web.config file</param>
        /// <param name="cmdText">SQL Command text</param>
        /// <param name="cmdType">Type of SQL command</param>
        public DBOperations(string connectionName, string cmdText, CommandType cmdType)
        {
            // Initialize Web.Config connection name
            this.connectionName = connectionName;

            // Initialize class member cmdText
            this.commandText = cmdText;

            // Initialize class member cmdType
            this.commandType = cmdType;
        }

        /// <summary>
        /// To execute parameterized stored procedure from DB
        /// </summary>
        /// <param name="connectionName">Name of connection from web.config file</param>
        /// <param name="cmdText">SQL Command text</param>
        /// <param name="spParams">Parameter list for stored procedure</param>
        public DBOperations(string connectionName, string cmdText, Collection<StoreProcedureParameter> spParameters)
            : this(connectionName, cmdText, CommandType.StoredProcedure)
        {
            // Initialize class member spParams
            if (spParameters.Count != 0)
            {
                spParams = spParameters;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initialize databse instance and its members
        /// </summary>
        private void Init()
        {
            // Initialize DataBase instance
            InitDB();

            // Initialize database command 
            InitDBCmd();

            // Check for command type storedprocedure
            if (commandType == CommandType.StoredProcedure)
            {
                // Initialize stored procedure parameres 
                InitSPParams();
            }
        }

        private void InitDB()
        {
            // Initialize Database object dbInstance by invoking CreateDatabase method of DatabaseFactory class 
            // by passing parameter value of connection string in Web.Config
            dbInstance = DatabaseFactory.CreateDatabase(connectionName);
        }

        /// <summary>
        /// Initialize database command
        /// </summary>
        private void InitDBCmd()
        {
            // Process DB operation request as per the request type
            switch (commandType)
            {
                case CommandType.StoredProcedure: // Is of type store procedure
                    {
                        // Initialize dbcommand for stored procedure
                        dbCommand = dbInstance.GetStoredProcCommand(commandText);
                        dbCommand.CommandTimeout = commandTimeout;
                        break;
                    }
                case CommandType.TableDirect:
                    break;
                case CommandType.Text: // Is of type text command
                    {
                        // Initialize dbcommand for text command
                        dbCommand = dbInstance.GetSqlStringCommand(commandText);
                        dbCommand.CommandTimeout = commandTimeout;
                        break;
                    }
                default:
                    // If wrong request type is passed throw exception
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, INVCMDTYPE, commandType));
            }
            // Initialize dbCommand's connection
            if (isTransactionReqd)
            {
                dbCommand.Connection = dbInstance.CreateConnection();
            }
        }

        /// <summary>
        /// Initialize database command
        /// </summary>
        private void InitDBCmd(DbCommand command)
        {
            if (null == command)
            {
                return;
            }

            // Process DB operation request as per the request type
            switch (commandType)
            {
                case CommandType.StoredProcedure: // Is of type store procedure
                    {
                        command.CommandTimeout = commandTimeout;
                        break;
                    }
                case CommandType.TableDirect:
                    break;
                case CommandType.Text: // Is of type text command
                    {
                        command.CommandTimeout = commandTimeout;
                        break;
                    }
                default:
                    // If wrong request type is passed throw exception
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, INVCMDTYPE, commandType));
            }
            // Initialize dbCommand's connection
            if (isTransactionReqd)
            {
                command.Connection = dbInstance.CreateConnection();
            }
        }

        /// <summary>
        /// Adds store procedure parameters to command 
        /// </summary>
        private void InitSPParams()
        {

            try
            {
                dbCommand.Parameters.Clear();

                // Set SP output parameters list to null
                spOutParams = null;

                // Iterate through each parameter information from DB 
                // and add parameter to database instace depends on parameter type in, inout or out 

                foreach (StoreProcedureParameter spParameter in spParams)
                {
                    // Check if parameter is in parameter
                    if (spParameter.IsInParameter)
                    {
                        // Add in parameter

                        dbInstance.AddInParameter(dbCommand,
                                            spParameter.ParameterName,
                                            GetDBType(spParameter.DbType),
                                            spParameter.Value);


                    }
                    else // Parameter is out parameter
                    {



                        dbInstance.AddOutParameter(dbCommand,
                                            spParameter.ParameterName,
                                             GetDBType(spParameter.DbType),
                                             spParameter.Size);
                    }

                }

            }
            catch
            {

                throw;
            }

        }

        /// <summary>
        /// Adds store procedure parameters to command 
        /// </summary>
        private void InitSPParams(DbCommand cmd, IEnumerable<StoreProcedureParameter> spParameters)
        {
            if (null == cmd)
            {
                return;
            }

            try
            {
                cmd.Parameters.Clear();

                // Set SP output parameters list to null
                //spOutParams = null;

                // Iterate through each parameter information from DB 
                // and add parameter to database instace depends on parameter type in, inout or out 

                foreach (StoreProcedureParameter spParameter in spParameters)
                {
                    // Check if parameter is in parameter
                    if (spParameter.IsInParameter)
                    {
                        // Add in parameter

                        dbInstance.AddInParameter(cmd,
                                            spParameter.ParameterName,
                                            GetDBType(spParameter.DbType),
                                            spParameter.SourceColumn,
                                            DataRowVersion.Original);


                    }
                    else // Parameter is out parameter
                    {

                        dbInstance.AddParameter(cmd,
                                            spParameter.ParameterName,
                                            GetDBType(spParameter.DbType),
                                            ParameterDirection.Output,
                                            spParameter.SourceColumn,
                                            DataRowVersion.Original,
                                            null);
                    }

                }

            }
            catch
            {

                throw;
            }

        }

        /// <summary>
        /// Adds store procedure parameters to command 
        /// </summary>
        private void InitSPParams(DbCommand cmd, params StoreProcedureParameter[] spParameters)
        {

            try
            {
                cmd.Parameters.Clear();

                // Set SP output parameters list to null
                //spOutParams = null;

                // Iterate through each parameter information from DB 
                // and add parameter to database instace depends on parameter type in, inout or out 

                foreach (StoreProcedureParameter spParameter in spParams)
                {
                    // Check if parameter is in parameter
                    if (spParameter.IsInParameter)
                    {
                        // Add in parameter

                        dbInstance.AddInParameter(dbCommand,
                                            spParameter.ParameterName,
                                            GetDBType(spParameter.DbType),
                                            spParameter.Value);


                    }
                    else // Parameter is out parameter
                    {



                        dbInstance.AddOutParameter(dbCommand,
                                            spParameter.ParameterName,
                                             GetDBType(spParameter.DbType),
                                             spParameter.Size);
                    }

                }

            }
            catch
            {

                throw;
            }

        }

        /// <summary>
        /// Gets the stored procedure output parameters
        /// </summary>
        /// <returns>Output parameters list in terms of hash table</returns>
        private Hashtable GetSPOutParams(string pName)
        {

            try
            {
                // Initialize empty list of sp output parameters
                spOutParams = new Hashtable();
                spOutParams.Add(pName, dbInstance.GetParameterValue(dbCommand, pName));
                return spOutParams;
            }
            catch
            {

                // Re-throw exception
                throw;
            }
        }



        /// <summary>
        /// Finds DBType compatible with SqlDBType types
        /// </summary>
        /// <param name="sqlType">SQL type</param>
        /// <returns>DBType</returns>
        private static DbType GetDBType(string sqlType)
        {
            if (string.IsNullOrEmpty(sqlType) || string.IsNullOrEmpty(sqlType.Trim()))
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, INVDATATYPE, sqlType));
            }
            switch (sqlType.ToUpperInvariant().Trim())
            {
                case "CHAR":
                case "NCHAR":
                case "VARCHAR":
                case "NVARCHAR":
                case "TEXT":
                case "NTEXT":
                case "XML":
                    {
                        return DbType.String;
                    }
                case "DECIMAL":
                case "MONEY":
                case "NUMERIC":
                case "SMALLMONEY":
                    {
                        return DbType.Decimal;
                    }
                case "DATETIME":
                case "SMALLDATETIME":
                    {
                        return DbType.DateTime;
                    }
                case "DATE":
                    {
                        return DbType.Date;
                    }
                case "TINYINT":
                    {
                        return DbType.Byte;
                    }
                case "SMALLINT":
                    {
                        return DbType.Int16;
                    }
                case "INT":
                    {
                        return DbType.Int32;
                    }
                case "BIGINT":
                    {
                        return DbType.Int64;
                    }
                case "FLOAT":
                    {
                        return DbType.Double;
                    }
                case "REAL":
                    {
                        return DbType.Single;
                    }
                case "BIT":
                    {
                        return DbType.Boolean;
                    }
                case "UNIQUEIDENTIFIER":
                    {
                        return DbType.Guid;
                    }
                case "SQL_VARIANT":
                    {
                        return DbType.Object;
                    }
                case "IMAGE":
                case "BINARY":
                case "VARBINARY":
                    {
                        return DbType.Binary;
                    }
                case "STRUCTURED":
                    {
                        return DbType.Object;
                    }
            }

            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, INVDATATYPE, sqlType));
        }

        #endregion

        #region Public Members

        #region IDBOperations Members

        /// <summary>
        /// Process database request
        /// </summary>
        /// <returns>Execution success result</returns>
        public int ExecuteNonQuery()
        {

            try
            {

                if (!isTransactionReqd) // If transaction begin, commit or rollback not required
                {

                    // Initialize database instance

                    Init();

                    // Invoke ExecuteNonQuery method of database instance and return number rows affected

                    return dbInstance.ExecuteNonQuery(dbCommand);
                }
                else
                {


                    if (commandType == CommandType.StoredProcedure)
                    {


                        InitSPParams();
                    }

                    // Set transaction of command instance
                    dbCommand.Transaction = dbTransaction;


                    // Invoke ExecuteNonQuery method of dbcommand instance and return number rows affected

                    return dbCommand.ExecuteNonQuery();
                }
            }
            catch
            {

                throw;
            }
            finally
            {
                if (null != spParams)
                {
                    spParams.Clear();
                }
                //spParams = null;

            }
        }

        /// <summary>
        /// Process database request to return data set
        /// </summary>
        /// <returns>Result in forms of data set</returns>
        public DataSet ExecuteDataSet()
        {

            try
            {
                // Initialize database instance

                Init();


                // Invoke ExecuteDataSet method and return resultant dataset

                return dbInstance.ExecuteDataSet(dbCommand);
            }
            catch
            {

                throw;
            }
            finally
            {
                //spParams = null;
                if (null != spParams)
                {
                    spParams.Clear();
                }
            }
        }

        /// <summary>
        /// Process database reader request
        /// </summary>
        /// <returns>IDataReader</returns>
        public T ExecuteReader<T>()
        {

            try
            {
                // Initialize database instance

                Init();

                //Invoke ExecuteReader method and return resultant datareader

                return (T)dbInstance.ExecuteReader(dbCommand);
            }
            catch
            {

                throw;
            }
            finally
            {
                //spParams = null;
                if (null != spParams)
                {
                    spParams.Clear();
                }
            }
        }

        /// <summary>
        /// Process database scalar request
        /// </summary>
        /// <returns>Scaler request result</returns>
        public T ExecuteScalar<T>()
        {

            try
            {
                // Initialize database instance

                Init();

                // Invoke ExecuteScalar method and return resultant object value

                return (T)dbInstance.ExecuteScalar(dbCommand);
            }
            catch
            {

                throw;
            }
            finally
            {
                //spParams = null;
                if (null != spParams)
                {
                    spParams.Clear();
                }
            }
        }

        /// <summary>
        ///  To define and begin trnsaction
        /// </summary>
        public void BeginTransaction()
        {

            // Set transaction flag to true
            isTransactionReqd = true;


            // Initialize database object

            InitDB();

            // Initialize command object

            InitDBCmd();

            // open connection if close

            if (dbCommand.Connection.State == ConnectionState.Closed)
            {


                dbCommand.Connection.Open();

            }

            dbTransaction = dbCommand.Connection.BeginTransaction();



        }

        /// <summary>
        /// To commit the transaction
        /// </summary>
        public void CommitTransaction()
        {

            // Commit Trnsaction 
            dbTransaction.Commit();
            // Close connection if opened

            if (dbCommand.Connection.State == ConnectionState.Open)
            {

                dbCommand.Connection.Close();

            }

            // Set transaction flag to false
            isTransactionReqd = false;

        }

        /// <summary>
        /// To rollback the transaction
        /// </summary>
        public void RollbackTransaction()
        {


            // Rollback transaction 
            dbTransaction.Rollback();


            // Close connection if opened

            if (dbCommand.Connection.State == ConnectionState.Open)
            {


                dbCommand.Connection.Close();

            }

            // Set transaction flag to false
            isTransactionReqd = false;


        }

        #endregion

        /// <summary>
        /// Gets the value of specific stored procedure output paramete
        /// </summary>
        /// <typeparam name="T">Type of parameter</typeparam>
        /// <param name="name">Name of output parameter</param>
        /// <returns>Parameter value</returns>
        public T GetSPOutParameterValue<T>(string parameterName)
        {
            try
            {
                if (spOutParams == null)
                {
                    // Get stored peocedur's output parameter
                    //GetSPOutParams(parameterName);

                    spOutParams = new Hashtable();
                }
                if (!spOutParams.ContainsKey(parameterName))
                {
                    spOutParams.Add(parameterName, dbInstance.GetParameterValue(dbCommand, parameterName));
                }

                // Check that paramter value is not null
                if (spOutParams[parameterName] != DBNull.Value)
                {
                    // return value of output parameter
                    return (T)spOutParams[parameterName];
                }
                else // If null
                {
                    return default(T);
                }
            }
            catch
            {
                //Re-throw exception
                throw;
            }
        }

        public int UpdateDataSet(DataSet dataset, string tableName, string insertSPName, IEnumerable<StoreProcedureParameter> insertSPParameters,
            string updateSPName, IEnumerable<StoreProcedureParameter> updateSPParameters,
            string deleteSPName, IEnumerable<StoreProcedureParameter> deleteSPParameters)
        {
            try
            {
                InitDB();
                DbCommand insertCommand = string.IsNullOrEmpty(insertSPName) ? null : dbInstance.GetStoredProcCommand(insertSPName);
                DbCommand updateCommand = string.IsNullOrEmpty(updateSPName) ? null : dbInstance.GetStoredProcCommand(updateSPName);
                DbCommand deleteCommand = string.IsNullOrEmpty(deleteSPName) ? null : dbInstance.GetStoredProcCommand(deleteSPName);

                InitUpdateCommand(insertCommand, insertSPParameters, updateCommand, updateSPParameters, deleteCommand, deleteSPParameters);

                if (!isTransactionReqd)
                {
                    return dbInstance.UpdateDataSet(dataset, tableName, insertCommand, updateCommand, deleteCommand, UpdateBehavior.Standard);
                }
                else
                {
                    return dbInstance.UpdateDataSet(dataset, tableName, insertCommand, updateCommand, deleteCommand, dbTransaction);
                }
            }
            catch
            {

                throw;
            }
            finally
            {
                //spParams = null;
                if (null != spParams)
                {
                    spParams.Clear();
                }
            }
        }

        private void InitUpdateCommand(DbCommand insertCommand, IEnumerable<StoreProcedureParameter> insertSPParameters,
            DbCommand updateCommand, IEnumerable<StoreProcedureParameter> updateSPParameters,
            DbCommand deleteCommand, IEnumerable<StoreProcedureParameter> deleteSPParameters)
        {
            // Initialize database command 
            InitDBCmd(insertCommand);
            InitDBCmd(updateCommand);
            InitDBCmd(deleteCommand);

            // Check for command type storedprocedure
            if (commandType == CommandType.StoredProcedure)
            {
                // Initialize stored procedure parameres 
                InitSPParams(insertCommand, insertSPParameters);
                InitSPParams(updateCommand, updateSPParameters);
                InitSPParams(deleteCommand, deleteSPParameters);
            }
        }

        #endregion
    }
}
