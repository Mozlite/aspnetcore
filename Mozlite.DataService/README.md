# 数据库ADO操作库

> 可以直接使用基础数据库操作ADO接口`IDatabase`，包含了ADO基础操作方法。

## 基础操作方法

```csharp
	/// <summary>
    /// 执行没有返回值的查询实例对象。
    /// </summary>
    /// <param name="commandText">SQL字符串。</param>
    /// <param name="parameters">参数实例对象。</param>
    /// <param name="commandType">命令类型。</param>
    /// <returns>返回是否有执行影响到数据行。</returns>
    bool ExecuteNonQuery(
        string commandText,
        object parameters = null,
        CommandType commandType = CommandType.Text);

    /// <summary>
    /// 查询实例对象。
    /// </summary>
    /// <param name="commandText">SQL字符串。</param>
    /// <param name="parameters">参数实例对象。</param>
    /// <param name="commandType">命令类型。</param>
    /// <returns>返回数据库读取实例接口。</returns>
    DbDataReader ExecuteReader(
        string commandText,
        object parameters = null,
        CommandType commandType = CommandType.Text);

    /// <summary>
    /// 查询数据库聚合值。
    /// </summary>
    /// <param name="commandText">SQL字符串。</param>
    /// <param name="parameters">参数实例对象。</param>
    /// <param name="commandType">命令类型。</param>
    /// <returns>返回聚合值实例对象。</returns>
    object ExecuteScalar(
        string commandText,
        object parameters = null,
        CommandType commandType = CommandType.Text);

    /// <summary>
    /// 执行SQL语句。
    /// </summary>
    /// <param name="commandText">SQL字符串。</param>
    /// <param name="parameters">参数匿名类型。</param>
    /// <param name="commandType">SQL类型。</param>
    /// <param name="cancellationToken">取消标记。</param>
    /// <returns>返回影响的行数。</returns>
    Task<bool> ExecuteNonQueryAsync(
        string commandText,
        object parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行SQL语句。
    /// </summary>
    /// <param name="commandText">SQL字符串。</param>
    /// <param name="commandType">SQL类型。</param>
    /// <param name="parameters">参数匿名类型。</param>
    /// <param name="cancellationToken">取消标记。</param>
    /// <returns>返回数据库读取器实例对象。</returns>
    Task<DbDataReader> ExecuteReaderAsync(
        string commandText,
        object parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 执行SQL语句。
    /// </summary>
    /// <param name="commandText">SQL字符串。</param>
    /// <param name="commandType">SQL类型。</param>
    /// <param name="parameters">参数匿名类型。</param>
    /// <param name="cancellationToken">取消标记。</param>
    /// <returns>返回单一结果实例对象。</returns>
    Task<object> ExecuteScalarAsync(string commandText,
        object parameters = null,
        CommandType commandType = CommandType.Text,
        CancellationToken cancellationToken = default);
```

## 事务方法

```csharp
    /// <summary>
    /// 开启一个事务并执行。
    /// </summary>
    /// <param name="executor">事务执行的方法。</param>
    /// <param name="timeout">等待命令执行所需的时间（以秒为单位）。默认值为 30 秒。</param>
    /// <param name="cancellationToken">取消标识。</param>
    /// <returns>返回事务实例对象。</returns>
    Task<bool> BeginTransactionAsync(Func<IDbTransaction, Task<bool>> executor, int timeout = 30, CancellationToken cancellationToken = default);

    /// <summary>
    /// 开启一个事务并执行。
    /// </summary>
    /// <param name="executor">事务执行的方法。</param>
    /// <param name="timeout">等待命令执行所需的时间（以秒为单位）。默认值为 30 秒。</param>
    /// <returns>返回事务实例对象。</returns>
    bool BeginTransaction(Func<IDbTransaction, bool> executor, int timeout = 30);
```


## 辅助方法

```csharp
    /// <summary>
    /// 批量插入数据。
    /// </summary>
    /// <param name="table">模型列表。</param>
    Task ImportAsync(DataTable table);

    /// <summary>
    /// 获取数据库版本信息。
    /// </summary>
    /// <returns>返回数据库版本信息。</returns>
    string GetVersion();
```