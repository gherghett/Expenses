using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;
using Expenses.Models;

namespace Expenses;

public partial class ExpenseDapperDal
{
    public List<ExpenseView> GetAllExpenses()
    {
        var catsOfExps = GetAllCategoryOfExpense();
        var categories = GetAllCategories();

        using var connection = new SqliteConnection(_connectionString);
        var expenses = connection.Query<ExpenseView>("SELECT * FROM Expense").ToList();

        foreach (var expense in expenses)
        {
            expense.Categories = categories
                .Where(cat => catsOfExps
                    .Where(coe => coe.ExpenseId == expense.Id)
                    .Select(coe => coe.CategoryId)
                    .Contains(cat.Id)
                )
                .ToList();
        }

        return expenses;
    }

    private List<CategoryOfExpense> GetAllCategoryOfExpense()
    {
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<CategoryOfExpense>("SELECT * FROM CategoryOfExpense").ToList();
    }

    public List<Category> GetAllCategories()
    {
        using var connection = new SqliteConnection(_connectionString);
        return connection.Query<Category>("SELECT * FROM Category").ToList();
    }

    public List<CategoryView> GetAllCategoryViews()
    {
        var categories = GetAllCategories();
        var expenseViews = GetAllExpenses();
        var catsOfExps = GetAllCategoryOfExpense();

        return categories.Select(category => new CategoryView
        {
            Id = category.Id,
            Name = category.Name,
            Expenses = expenseViews
                .Where(exp => catsOfExps
                    .Where(coe => coe.CategoryId == category.Id)
                    .Select(coe => coe.ExpenseId)
                    .Contains(exp.Id)
                )
                .ToList()
        }).ToList();
    }
}