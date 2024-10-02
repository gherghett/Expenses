using System.Data;
using Expenses.Models;

namespace Expenses;
public partial class ExpenseDal
{
    public bool UpdateExpense(ExpenseView expenseView)
    {
        var catsOfExps = GetAllCategoryOfExpense(); // Existing category associations
        var existingCategoryIds = catsOfExps.Where(coe => coe.ExpenseId == expenseView.Id)
                                            .Select(coe => coe.CategoryId)
                                            .ToHashSet(); // Faster lookup

        var newCategoryIds = expenseView.Categories.Select(c => c.Id).ToHashSet();

        using var sqlConnection = new SqliteConnection(_connectionString);
        sqlConnection.Open();

        using var transaction = sqlConnection.BeginTransaction(); // Start a transaction

        try
        {
            // 1. Update the expense data
            UpdateExpenseData(expenseView, sqlConnection, transaction);

            // 2. Remove categories that no longer exist
            RemoveOldCategories(expenseView.Id, existingCategoryIds, newCategoryIds, sqlConnection, transaction);

            // 3. Add new categories that are not already associated with the expense
            AddNewCategories(expenseView.Id, existingCategoryIds, newCategoryIds, sqlConnection, transaction);

            // Commit transaction
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

    private void UpdateExpenseData(ExpenseView expenseView, SqliteConnection sqlConnection, SqliteTransaction transaction)
    {
        string expenseQuery = @"UPDATE Expense 
                                SET 
                                    Name = $Name,
                                    Description = $Desc,
                                    Date = $Date
                                WHERE
                                    Id = $Id";
        using var updateExpense = new SqliteCommand(expenseQuery, sqlConnection, transaction);
        updateExpense.Parameters.AddWithValue("$Id", expenseView.Id);
        updateExpense.Parameters.AddWithValue("$Name", expenseView.Name);
        updateExpense.Parameters.AddWithValue("$Desc", expenseView.Description);
        updateExpense.Parameters.AddWithValue("$Date", expenseView.DateOfP.ToString("yyyy-MM-dd HH:mm:ss"));
        updateExpense.ExecuteNonQuery();
    }

    private void RemoveOldCategories(long expenseId, HashSet<long> existingCategoryIds, HashSet<long> newCategoryIds, SqliteConnection sqlConnection, SqliteTransaction transaction)
    {
        var categoriesToRemove = existingCategoryIds.Except(newCategoryIds).ToList();
        if (categoriesToRemove.Count > 0)
        {
            string removeCategoryQuery = "DELETE FROM CategoryOfExpense WHERE ExpenseId = $expenseId AND CategoryId = $catId";
            using var removeCatOfExp = new SqliteCommand(removeCategoryQuery, sqlConnection, transaction);
            removeCatOfExp.Parameters.AddWithValue("$expenseId", expenseId);

            foreach (var catId in categoriesToRemove)
            {
                removeCatOfExp.Parameters.Clear();
                removeCatOfExp.Parameters.AddWithValue("$expenseId", expenseId);
                removeCatOfExp.Parameters.AddWithValue("$catId", catId);
                removeCatOfExp.ExecuteNonQuery();
            }
        }
    }

    private void AddNewCategories(long expenseId, HashSet<long> existingCategoryIds, HashSet<long> newCategoryIds, SqliteConnection sqlConnection, SqliteTransaction transaction)
    {
        var categoriesToAdd = newCategoryIds.Except(existingCategoryIds).ToList();
        if (categoriesToAdd.Count > 0)
        {
            string insertCategoryQuery = "INSERT INTO CategoryOfExpense (ExpenseId, CategoryId) VALUES ($expenseId, $catId)";
            using var insertCatOfExp = new SqliteCommand(insertCategoryQuery, sqlConnection, transaction);
            insertCatOfExp.Parameters.AddWithValue("$expenseId", expenseId);

            foreach (var catId in categoriesToAdd)
            {
                insertCatOfExp.Parameters.Clear();
                insertCatOfExp.Parameters.AddWithValue("$expenseId", expenseId);
                insertCatOfExp.Parameters.AddWithValue("$catId", catId);
                insertCatOfExp.ExecuteNonQuery();
            }
        }
    }

    public bool UpdateCategory(Category updatedCategory)
    {
        using var sqlConnection = new SqliteConnection(_connectionString);
        sqlConnection.Open();
        string expenseQuery = @"UPDATE Category SET Name = $Name WHERE Id = $Id";
        using var command = new SqliteCommand(expenseQuery, sqlConnection);
        command.Parameters.AddWithValue("$Name", updatedCategory.Name);
        command.Parameters.AddWithValue("$Id", updatedCategory.Id);


        return command.ExecuteNonQuery() == 1;
    }

}