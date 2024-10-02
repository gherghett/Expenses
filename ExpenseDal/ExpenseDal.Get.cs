using System.Collections.Immutable;
using System.Data;
using Expenses.Models;

namespace Expenses;
public partial class ExpenseDal
{
    public List<ExpenseView> GetAllExpenses()
    {
        var catsOfExps = GetAllCategoryOfExpense();
        var categories = GetAllCategories();
        List<ExpenseView> expenses = new List<ExpenseView>();
        using var sqlConnection = new SqliteConnection(_connectionString);
        sqlConnection.Open();
        string query = "SELECT * FROM Expense";
        using var command = new SqliteCommand(query, sqlConnection);
        using var reader = command.ExecuteReader();
        while(reader.Read())
        {
            long id = reader.GetInt64("Id");
            expenses.Add(
                new ExpenseView{
                    Id = id,
                    Name = reader.GetString("Name"),
                    Description = reader.GetString("Description"),
                    DateOfP = reader.GetDateTime("Date"),
                    Categories = categories
                        .Where(cat => catsOfExps
                            .Where(coe => coe.ExpenseId == id)
                            .Select(coe => coe.CategoryId)
                            .Contains(cat.Id)
                        )
                        .ToList()
                }
            );
        }
        reader.Close();
        return expenses;
    }

    private List<CategoryOfExpense> GetAllCategoryOfExpense()
    {
        List<CategoryOfExpense> catsOfExps = new List<CategoryOfExpense>();
        using var sqlConnection = new SqliteConnection(_connectionString);
        sqlConnection.Open();
        string query = "SELECT * FROM CategoryOfExpense";
        using var command = new SqliteCommand(query, sqlConnection);
        using var reader = command.ExecuteReader();
        while(reader.Read())
        {
            catsOfExps.Add(
                new CategoryOfExpense{
                    Id = reader.GetInt32("Id"),
                    CategoryId = reader.GetInt64("CategoryId"),
                    ExpenseId = reader.GetInt64("ExpenseId")
                }
            );
        }
        reader.Close();
        return catsOfExps;
    }
    public List<Category> GetAllCategories()
    {
        List<Category> categories = new List<Category>();
        using var sqlConnection = new SqliteConnection(_connectionString);
        sqlConnection.Open();
        string query = "SELECT * FROM Category";
        using var command = new SqliteCommand(query, sqlConnection);
        using var reader = command.ExecuteReader();
        while(reader.Read())
        {
            categories.Add(
                new Category{
                    Id = reader.GetInt64("Id"),
                    Name = reader.GetString("Name")
                }
            );
        }
        reader.Close();
        return categories;
    }

    public List<CategoryView> GetAllCategoryViews()
    {
        List<CategoryView> categoryViews = new List<CategoryView>();
        List<Category> categories = GetAllCategories();
        List<ExpenseView> expenseViews = GetAllExpenses();
        var catsOfExps = GetAllCategoryOfExpense();

        foreach(var category in categories)
        {
            categoryViews.Add(
                new CategoryView{
                    Id = category.Id,
                    Name = category.Name,
                    Expenses = expenseViews
                            .Where(exp => catsOfExps
                                .Where(coe => coe.CategoryId == category.Id)
                                .Select(coe => coe.ExpenseId)
                                .Contains(exp.Id)
                            )
                            .ToList()
                }
            );
        }

        return categoryViews;
    }
}
