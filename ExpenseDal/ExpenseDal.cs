using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Data;
using Expenses.Models;

namespace Expenses;
public partial class ExpenseDal
{
    //private SqliteConnection? _sqlConnection = null;
    private readonly string _connectionString;
    public ExpenseDal(string connectionString)
    {
        _connectionString = connectionString;
    }

    private long GetLastInsertedRow(SqliteConnection connection, SqliteTransaction transaction)
    {
        // Retrieve the last inserted expense ID
        string getLastIdQuery = "SELECT last_insert_rowid()";
        using var lastIdCommand = new SqliteCommand(getLastIdQuery, connection, transaction);
        return (long)(lastIdCommand.ExecuteScalar() ?? throw new Exception()); 

    }

}