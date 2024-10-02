
using System.Collections.Immutable;
using System.Data;
using Expenses.Models;

namespace Expenses;
public partial class ExpenseDal
{    
    public bool AddExpense(Expense expense, List<Category>? categories)
    {
        using var sqlConnection = new SqliteConnection(_connectionString);
        sqlConnection.Open();
        
        using var transaction = sqlConnection.BeginTransaction(); // Start a transaction

        try
        {
            string expenseQuery = "INSERT INTO Expense (Name, Description, Date) VALUES ($Name, $Desc, $Date)";
            using var insertExpense = new SqliteCommand(expenseQuery, sqlConnection, transaction);
            insertExpense.Parameters.AddWithValue("$Name", expense.Name);
            insertExpense.Parameters.AddWithValue("$Desc", expense.Description);
            insertExpense.Parameters.AddWithValue("$Date", expense.DateOfP.ToString("yyyy-MM-dd HH:mm:ss"));
            insertExpense.ExecuteNonQuery();

            // Get the last inserted expense ID
            long expenseId = GetLastInsertedRow(sqlConnection, transaction);

            // insert categories
            if (categories?.Count > 0)
            {
                string categoriesQuery = "INSERT INTO CategoryOfExpense (ExpenseId, CategoryId) VALUES ($expenseId, $catId)";
                using var insertCatOfExp = new SqliteCommand(categoriesQuery, sqlConnection, transaction);
                
                foreach (var category in categories)
                {
                    insertCatOfExp.Parameters.Clear(); // Clear parameters for each iteration
                    insertCatOfExp.Parameters.AddWithValue("$expenseId", expenseId);
                    insertCatOfExp.Parameters.AddWithValue("$catId", category.Id);
                    insertCatOfExp.ExecuteNonQuery(); // Execute each category insertion
                }
            }

            // complete the transaction if everything is successful
            transaction.Commit();
            return true;
        }
        catch
        {
            //take back the transaction in case of any error
            transaction.Rollback();
            return false; 
        }
    }

    public bool AddCategory(string name)
    {
        using var sqlConnection = new SqliteConnection(_connectionString);
        sqlConnection.Open();
        string expenseQuery = @"INSERT INTO Category (Name) VALUES ($Name)";
        using var command = new SqliteCommand(expenseQuery, sqlConnection);
        command.Parameters.AddWithValue("$Name", name);

        return command.ExecuteNonQuery() == 1;
    }
}