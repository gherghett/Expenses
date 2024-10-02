namespace Expenses.Models;
public class CategoryView : Category
{
    public required List<ExpenseView> Expenses { get; set; }
}

