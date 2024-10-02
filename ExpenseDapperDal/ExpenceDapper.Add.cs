using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Expenses.Models;

namespace Expenses;

public partial class ExpenseDapperDal
{
    public bool AddExpense(Expense expense, List<Category>? categories)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();
        
        using var transaction = connection.BeginTransaction();
        try
        {
            const string expenseQuery = @"
                INSERT INTO Expense (Name, Description, Date) 
                VALUES (@Name, @Description, @Date);
                SELECT last_insert_rowid();";

            long expenseId = connection.ExecuteScalar<long>(expenseQuery, new
            {
                expense.Name,
                expense.Description,
                Date = expense.DateOfP
            }, transaction);

            if (categories?.Count > 0)
            {
                const string categoriesQuery = @"
                    INSERT INTO CategoryOfExpense (ExpenseId, CategoryId) 
                    VALUES (@ExpenseId, @CategoryId)";

                var categoryExpenses = categories.Select(c => new 
                { 
                    ExpenseId = expenseId, 
                    CategoryId = c.Id 
                });

                connection.Execute(categoriesQuery, categoryExpenses, transaction);
            }

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }

    public bool AddCategory(string name)
    {
        using var connection = new SqliteConnection(_connectionString);
        const string categoryQuery = "INSERT INTO Category (Name) VALUES (@Name)";
        int rowsAffected = connection.Execute(categoryQuery, new { Name = name });
        return rowsAffected == 1;
    }

    // Helper method to get the last inserted row ID
    private long GetLastInsertedRow(IDbConnection connection, IDbTransaction transaction)
    {
        return connection.ExecuteScalar<long>("SELECT last_insert_rowid()", transaction: transaction);
    }
}