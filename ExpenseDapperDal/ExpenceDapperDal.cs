    using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Expenses.Models;

namespace Expenses;

public partial class ExpenseDapperDal : IExpenseDal
{
    private readonly string _connectionString;

    public ExpenseDapperDal(string connectionString)
    {
        _connectionString = connectionString;
    }
}