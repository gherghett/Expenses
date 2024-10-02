using System.Collections.Immutable;
using System.Data;
using Expenses.Models;

namespace Expenses;
public partial class ExpenseDal
{
    public bool DeleteExpense(long expenseId)
    {
        using var sqlConnection = new SqliteConnection(_connectionString);
        sqlConnection.Open();

        using var transaction = sqlConnection.BeginTransaction(); // Start a transaction

        try
        {
            // 1. Delete associated categories from CategoryOfExpense
            DeleteAssociatedCategories(expenseId, sqlConnection, transaction);

            // 2. Delete the expense itself
            DeleteExpenseRow(expenseId, sqlConnection, transaction);

            // Commit the transaction
            transaction.Commit();
            return true;
        }
        catch
        {
            // Rollback the transaction in case of any error
            transaction.Rollback();
            return false;
        }
    }

    private void DeleteExpenseRow(long expenseId, SqliteConnection sqlConnection, SqliteTransaction transaction)
    {
        string deleteExpenseQuery = "DELETE FROM Expense WHERE Id = $Id";
        using var deleteExpenseCmd = new SqliteCommand(deleteExpenseQuery, sqlConnection, transaction);
        deleteExpenseCmd.Parameters.AddWithValue("$Id", expenseId);
        deleteExpenseCmd.ExecuteNonQuery();
    }

    private void DeleteAssociatedCategories(long expenseId, SqliteConnection sqlConnection, SqliteTransaction transaction)
    {
        string deleteCategoryQuery = "DELETE FROM CategoryOfExpense WHERE ExpenseId = $expenseId";
        using var deleteCatOfExpCmd = new SqliteCommand(deleteCategoryQuery, sqlConnection, transaction);
        deleteCatOfExpCmd.Parameters.AddWithValue("$expenseId", expenseId);
        deleteCatOfExpCmd.ExecuteNonQuery();
    }

    public bool DeleteCategory(long categoryId)
    {
        using var sqlConnection = new SqliteConnection(_connectionString);
        sqlConnection.Open();

        using var transaction = sqlConnection.BeginTransaction(); // Start a transaction

        try
        {
            // 1. Delete associated categories from CategoryOfExpense
            DeleteAssociatedCategoryOfExpenses(categoryId, sqlConnection, transaction);

            // 2. Delete the category itself
            DeleteCategoryRow(categoryId, sqlConnection, transaction);

            // Commit the transaction
            transaction.Commit();
            return true;
        }
        catch
        {
            // Rollback the transaction in case of any error
            transaction.Rollback();
            return false;
        }
    }

    private void DeleteAssociatedCategoryOfExpenses(long categoryId, SqliteConnection sqlConnection, SqliteTransaction transaction)
    {
        string deleteCategoryQuery = "DELETE FROM CategoryOfExpense WHERE CategoryId = $categoryId";
        using var deleteCatOfExpCmd = new SqliteCommand(deleteCategoryQuery, sqlConnection, transaction);
        deleteCatOfExpCmd.Parameters.AddWithValue("$categoryId", categoryId);
        deleteCatOfExpCmd.ExecuteNonQuery();
    }

    private void DeleteCategoryRow(long categoryId, SqliteConnection sqlConnection, SqliteTransaction transaction)
    {
        string deleteCategoryQuery = "DELETE FROM Category WHERE Id = $Id";
        using var deleteCategoryCmd = new SqliteCommand(deleteCategoryQuery, sqlConnection, transaction);
        deleteCategoryCmd.Parameters.AddWithValue("$Id", categoryId);
        deleteCategoryCmd.ExecuteNonQuery();
    }
}