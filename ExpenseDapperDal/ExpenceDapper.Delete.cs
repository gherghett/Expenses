using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Expenses.Models;

namespace Expenses;

public partial class ExpenseDapperDal
{
    public bool DeleteExpense(long expenseId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Delete associated categories from CategoryOfExpense
            DeleteAssociatedCategories(expenseId, connection, transaction);

            // 2. Delete the expense itself
            DeleteExpenseRow(expenseId, connection, transaction);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }

    private void DeleteExpenseRow(long expenseId, IDbConnection connection, IDbTransaction transaction)
    {
        const string deleteExpenseQuery = "DELETE FROM Expense WHERE Id = @Id";
        connection.Execute(deleteExpenseQuery, new { Id = expenseId }, transaction);
    }

    private void DeleteAssociatedCategories(long expenseId, IDbConnection connection, IDbTransaction transaction)
    {
        const string deleteCategoryQuery = "DELETE FROM CategoryOfExpense WHERE ExpenseId = @ExpenseId";
        connection.Execute(deleteCategoryQuery, new { ExpenseId = expenseId }, transaction);
    }

    public bool DeleteCategory(long categoryId)
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            // 1. Delete associated categories from CategoryOfExpense
            DeleteAssociatedCategoryOfExpenses(categoryId, connection, transaction);

            // 2. Delete the category itself
            DeleteCategoryRow(categoryId, connection, transaction);

            transaction.Commit();
            return true;
        }
        catch
        {
            transaction.Rollback();
            return false;
        }
    }

    private void DeleteAssociatedCategoryOfExpenses(long categoryId, IDbConnection connection, IDbTransaction transaction)
    {
        const string deleteCategoryQuery = "DELETE FROM CategoryOfExpense WHERE CategoryId = @CategoryId";
        connection.Execute(deleteCategoryQuery, new { CategoryId = categoryId }, transaction);
    }

    private void DeleteCategoryRow(long categoryId, IDbConnection connection, IDbTransaction transaction)
    {
        const string deleteCategoryQuery = "DELETE FROM Category WHERE Id = @Id";
        connection.Execute(deleteCategoryQuery, new { Id = categoryId }, transaction);
    }
}