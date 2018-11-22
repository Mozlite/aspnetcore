using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mozlite.Data
{
    /// <summary>
    /// 数据库基类。
    /// </summary>
    public abstract class Database : IDatabase
    {
        private readonly DbProviderFactory _factory;
        private readonly ISqlHelper _sqlHelper;
        /// <summary>
        /// 数据库选项。
        /// </summary>
        protected DatabaseOptions Options { get; }

        /// <summary>
        /// 初始化 <see cref="Database"/> 类的新实例。
        /// </summary>
        /// <param name="logger">日志接口。</param>
        /// <param name="factory">数据库提供者工厂类。</param>
        /// <param name="options">配置选项。</param>
        /// <param name="sqlHelper">SQL辅助接口。</param>
        protected Database(ILogger logger, DbProviderFactory factory, IOptions<DatabaseOptions> options, ISqlHelper sqlHelper)
        {
            _factory = factory;
            _sqlHelper = sqlHelper;
            Options = options.Value;
            Logger = logger;
        }

        /// <summary>
        /// 日志接口。
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// 获取链接实例。
        /// </summary>
        /// <returns>返回数据库连接实例对象。</returns>
        protected DbConnection GetConnection()
        {
            var connection = _factory.CreateConnection();
            connection.ConnectionString = Options.ConnectionString;
            return connection;
        }

        /// <summary>
        /// 创建参数表达式。
        /// </summary>
        /// <param name="name">参数名称。</param>
        /// <param name="value">参数值。</param>
        /// <returns>返回数据库参数实例对象。</returns>
        protected virtual DbParameter CreateParameter(string name, object value)
        {
            var p = _factory.CreateParameter();
            p.ParameterName = name;
            if (value == null)
                p.Value = DBNull.Value;
            else
            {
                var type = Nullable.GetUnderlyingType(value.GetType());
                if (type?.GetTypeInfo().IsEnum == true)
                {
                    type = Enum.GetUnderlyingType(type);
                    if (type == typeof(int))
                        value = (int)value;
                    else if (type == typeof(short))
                        value = (short)value;
                    else if (type == typeof(long))
                        value = (long)value;
                }
                p.Value = value;
            }
            return p;
        }

        private void AttachParameters(DbParameterCollection dbParameters, object parameters)
        {
            var dic = parameters.ToDictionary();
            foreach (var parameter in dic)
            {
                dbParameters.Add(CreateParameter(parameter.Key, parameter.Value));
            }
        }

        /// <summary>
        /// 替换表格前缀。
        /// </summary>
        /// <param name="commandText">当前SQL语句。</param>
        /// <returns>返回替换后的语句。</returns>
        protected string ReplacePrefixed(string commandText)
        {
            return commandText.Replace("$pre:$", string.Empty).Replace("$pre:", Options.Prefix);
        }

        private DbCommand GetCommand(DbConnection connection,
            CommandType commandType,
            string commandText,
            object parameters = null)
        {
            var command = _factory.CreateCommand();
            command.Connection = connection;
            command.CommandText = ReplacePrefixed(commandText);
            command.CommandType = commandType;
            command.Parameters.Clear();
            if (parameters != null)
                AttachParameters(command.Parameters, parameters);
            if (connection.State != ConnectionState.Open)
                connection.Open();
            return command;
        }

        private TResult ExecuteCommand<TResult>(DbCommand command, Func<DbCommand, TResult> execute)
        {
            try
            {
                return execute(command);
            }
            catch (Exception exception)
            {
                LogError(command, exception);
                throw;
            }
        }
        private async Task<TResult> ExecuteCommandAsync<TResult>(DbCommand command, Func<DbCommand, Task<TResult>> execute)
        {
            try
            {
                return await execute(command);
            }
            catch (Exception exception)
            {
                LogError(command, exception);
                throw;
            }
        }

        private void LogError(DbCommand command, Exception exception)
        {
            var commandText = command.CommandText;
            foreach (var parameter in command.Parameters.OfType<DbParameter>()
                .OrderByDescending(p => p.ParameterName.Length))
            {
                commandText = commandText.Replace(_sqlHelper.Parameterized(parameter.ParameterName),
                    _sqlHelper.EscapeLiteral(parameter.Value));
            }
            var error = new StringBuilder();
            error.Append("[数据库]执行SQL错误：").AppendLine(exception.Message);
            error.AppendLine("==================================================");
            error.AppendLine(commandText);
            error.AppendLine("==================================================");
            error.AppendLine(exception.StackTrace);
            Logger.LogError(2, exception, error.ToString());
        }

        /// <summary>
        /// 执行没有返回值的查询实例对象。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数实例对象。</param>
        /// <param name="commandType">命令类型。</param>
        /// <returns>返回是否有执行影响到数据行。</returns>
        public virtual bool ExecuteNonQuery(string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text)
        {
            using (var connection = GetConnection())
            {
                var command = GetCommand(connection, commandType, commandText, parameters);
                var affectedRows = ExecuteCommand(command, cmd => cmd.ExecuteNonQuery());
                command.Parameters.Clear();
                return affectedRows > 0;
            }
        }

        /// <summary>
        /// 查询实例对象。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数实例对象。</param>
        /// <param name="commandType">命令类型。</param>
        /// <returns>返回数据库读取实例接口。</returns>
        public virtual DbDataReader ExecuteReader(string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text)
        {
            var connection = GetConnection();
            var command = GetCommand(connection, commandType, commandText, parameters);
            var reader = ExecuteCommand(command, cmd => cmd.ExecuteReader(CommandBehavior.CloseConnection));
            command.Parameters.Clear();
            return reader;
        }

        /// <summary>
        /// 查询数据库聚合值。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数实例对象。</param>
        /// <param name="commandType">命令类型。</param>
        /// <returns>返回聚合值实例对象。</returns>
        public virtual object ExecuteScalar(string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text)
        {
            using (var connection = GetConnection())
            {
                var command = GetCommand(connection, commandType, commandText, parameters);
                var scalar = ExecuteCommand(command, cmd => cmd.ExecuteScalar());
                command.Parameters.Clear();
                return scalar;
            }
        }

        /// <summary>
        /// 执行SQL语句。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="parameters">参数匿名类型。</param>
        /// <param name="commandType">SQL类型。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回影响的行数。</returns>
        public virtual async Task<bool> ExecuteNonQueryAsync(string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = GetConnection())
            {
                var command = GetCommand(connection, commandType, commandText, parameters);
                var affectedRows = await ExecuteCommandAsync(command, cmd => cmd.ExecuteNonQueryAsync(cancellationToken));
                command.Parameters.Clear();
                return affectedRows > 0;
            }
        }

        /// <summary>
        /// 执行SQL语句。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="commandType">SQL类型。</param>
        /// <param name="parameters">参数匿名类型。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回数据库读取器实例对象。</returns>
        public virtual async Task<DbDataReader> ExecuteReaderAsync(string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var connection = GetConnection();
            var command = GetCommand(connection, commandType, commandText, parameters);
            var reader = await ExecuteCommandAsync(command, cmd => cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection, cancellationToken));
            command.Parameters.Clear();
            return reader;
        }

        /// <summary>
        /// 执行SQL语句。
        /// </summary>
        /// <param name="commandText">SQL字符串。</param>
        /// <param name="commandType">SQL类型。</param>
        /// <param name="parameters">参数匿名类型。</param>
        /// <param name="cancellationToken">取消标记。</param>
        /// <returns>返回单一结果实例对象。</returns>
        public virtual async Task<object> ExecuteScalarAsync(string commandText,
            object parameters = null,
            CommandType commandType = CommandType.Text,
            CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using (var connection = GetConnection())
            {
                var command = GetCommand(connection, commandType, commandText, parameters);
                var scalar = await ExecuteCommandAsync(command, cmd => cmd.ExecuteScalarAsync(cancellationToken));
                command.Parameters.Clear();
                return scalar;
            }
        }

        /// <summary>
        /// 开启一个事务并执行。
        /// </summary>
        /// <param name="executor">事务执行的方法。</param>
        /// <param name="timeout">等待命令执行所需的时间（以秒为单位）。默认值为 30 秒。</param>
        /// <returns>返回事务实例对象。</returns>
        public virtual bool BeginTransaction(Func<Internal.IDbTransaction, bool> executor, int timeout = 30)
        {
            using (var connection = GetConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var command = _factory.CreateCommand();
                    try
                    {
                        command.CommandTimeout = timeout;
                        command.Connection = connection;
                        command.Transaction = transaction;
                        var current = new Transaction(command, ReplacePrefixed, AttachParameters, LogError);
                        if (executor(current))
                        {
                            transaction.Commit();
                            return true;
                        }
                        transaction.Rollback();
                        return false;
                    }
                    catch (Exception exception)
                    {
                        LogError(command, exception);
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// 批量插入数据。
        /// </summary>
        /// <param name="table">模型列表。</param>
        public abstract Task ImportAsync(DataTable table);

        /// <summary>
        /// 获取数据库版本信息。
        /// </summary>
        /// <returns>返回数据库版本信息。</returns>
        public abstract string GetVersion();

        /// <summary>
        /// 开启一个事务并执行。
        /// </summary>
        /// <param name="executor">事务执行的方法。</param>
        /// <param name="timeout">等待命令执行所需的时间（以秒为单位）。默认值为 30 秒。</param>
        /// <param name="cancellationToken">取消标识。</param>
        /// <returns>返回事务实例对象。</returns>
        public virtual async Task<bool> BeginTransactionAsync(Func<Internal.IDbTransaction, Task<bool>> executor, int timeout = 30, CancellationToken cancellationToken = default)
        {
            using (var connection = GetConnection())
            {
                await connection.OpenAsync(cancellationToken);
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    var command = _factory.CreateCommand();
                    try
                    {
                        command.CommandTimeout = timeout;
                        command.Connection = connection;
                        command.Transaction = transaction;
                        var current = new Transaction(command, ReplacePrefixed, AttachParameters, LogError);
                        if (await executor(current))
                        {
                            transaction.Commit();
                            return true;
                        }
                        transaction.Rollback();
                        return false;
                    }
                    catch (Exception exception)
                    {
                        LogError(command, exception);
                        transaction.Rollback();
                        return false;
                    }
                }
            }
        }

        class Transaction : Internal.IDbTransaction
        {
            private readonly DbCommand _command;
            private readonly Func<string, string> _replacePrefixed;
            private readonly Action<DbParameterCollection, object> _attachParameters;
            private readonly Action<DbCommand, Exception> _logError;

            public Transaction(DbCommand command, Func<string, string> replacePrefixed, Action<DbParameterCollection, object> attachParameters, Action<DbCommand, Exception> logError)
            {
                _command = command;
                _replacePrefixed = replacePrefixed;
                _attachParameters = attachParameters;
                _logError = logError;
            }

            public bool ExecuteNonQuery(string commandText, object parameters = null, CommandType commandType = CommandType.Text)
            {
                SetCommand(commandType, commandText, parameters);
                return ExecuteCommand(_command, cmd => cmd.ExecuteNonQuery()) > 0;
            }

            private void SetCommand(CommandType commandType, string commandText, object parameters)
            {
                _command.CommandText = _replacePrefixed(commandText);
                _command.CommandType = commandType;
                _command.Parameters.Clear();
                if (parameters != null)
                    _attachParameters(_command.Parameters, parameters);
                if (_command.Connection.State != ConnectionState.Open)
                    _command.Connection.Open();
            }

            public DbDataReader ExecuteReader(string commandText, object parameters = null, CommandType commandType = CommandType.Text)
            {
                SetCommand(commandType, commandText, parameters);
                return ExecuteCommand(_command, cmd => cmd.ExecuteReader());
            }

            public object ExecuteScalar(string commandText, object parameters = null, CommandType commandType = CommandType.Text)
            {
                SetCommand(commandType, commandText, parameters);
                return ExecuteCommand(_command, cmd => cmd.ExecuteScalar());
            }

            public async Task<bool> ExecuteNonQueryAsync(string commandText, object parameters = null, CommandType commandType = CommandType.Text,
                CancellationToken cancellationToken = default)
            {
                SetCommand(commandType, commandText, parameters);
                return await ExecuteCommandAsync(_command, cmd => cmd.ExecuteNonQueryAsync(cancellationToken)) > 0;
            }

            public async Task<DbDataReader> ExecuteReaderAsync(string commandText, object parameters = null, CommandType commandType = CommandType.Text,
                CancellationToken cancellationToken = default)
            {
                SetCommand(commandType, commandText, parameters);
                return await ExecuteCommandAsync(_command, cmd => cmd.ExecuteReaderAsync(cancellationToken));
            }

            public async Task<object> ExecuteScalarAsync(string commandText, object parameters = null, CommandType commandType = CommandType.Text,
                CancellationToken cancellationToken = default)
            {
                SetCommand(commandType, commandText, parameters);
                return await ExecuteCommandAsync(_command, cmd => cmd.ExecuteScalarAsync(cancellationToken));
            }

            private TResult ExecuteCommand<TResult>(DbCommand command, Func<DbCommand, TResult> execute)
            {
                try
                {
                    return execute(command);
                }
                catch (Exception exception)
                {
                    _logError(command, exception);
                    throw;
                }
            }

            private Task<TResult> ExecuteCommandAsync<TResult>(DbCommand command, Func<DbCommand, Task<TResult>> execute)
            {
                try
                {
                    return execute(command);
                }
                catch (Exception exception)
                {
                    _logError(command, exception);
                    throw;
                }
            }
        }
    }
}