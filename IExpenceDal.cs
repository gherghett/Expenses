using Expenses.Models;

namespace Expenses;

public interface IExpenseDal
{
    List<ExpenseView> GetAllExpenses();
    List<Category> GetAllCategories();
    List<CategoryView> GetAllCategoryViews();
    bool UpdateExpense(ExpenseView expenseView);
    bool UpdateCategory(Category updatedCategory);
    bool DeleteExpense(long expenseId);
    bool DeleteCategory(long categoryId);
    bool AddExpense(Expense expense, List<Category>? categories);
    bool AddCategory(string name);
}