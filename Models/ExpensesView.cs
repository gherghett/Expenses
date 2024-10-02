namespace Expenses.Models;

public class ExpenseView : Expense
{
    required public List<Category> Categories { get; set; }
}