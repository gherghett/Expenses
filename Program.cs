using System.Diagnostics.Contracts;
using Expenses.Models;

namespace Expenses;

class Program
{
    static void Main(string[] args)
    {
        var dal = new ExpenseDal("Data Source=Expenses.db");
        // List<ExpenseView> expenses = dal.GetAllExpenses();
        // dal.DeleteExpense(expenses[4].Id);
        var cats = dal.GetAllCategories();
        var expenses = dal.GetAllExpenses();
        Print(expenses);
        Console.WriteLine("---then we delete a category---");

        dal.DeleteCategory(expenses[0].Categories[0].Id);
        expenses = dal.GetAllExpenses();
        Print(expenses);

    }
    static void Print(List<ExpenseView> expenseViews) =>
    expenseViews.ForEach(expensesItem =>
        Console.WriteLine(expensesItem.Name + "\n\t" +
                        string.Join("\n\t", expensesItem.Categories
                                            .Select(c => c.Name).ToArray()
                        )
        )
    );

    static void DoSomeStuff(ExpenseDal dal)
    {
        List<ExpenseView> expenses = dal.GetAllExpenses();
        Print(expenses);

        Console.WriteLine("---then we add---");
        List<Category> categories = dal.GetAllCategories();
        dal.AddExpense(
            new Expense{
                Name = "Filter",
                Description = "Expensive, but oh so nice",
                DateOfP = DateTime.Now,
            },
            categories[0..2]
        );
        expenses = dal.GetAllExpenses();
        Print(expenses);

        Console.WriteLine($"---then we delete {expenses[0].Name}---");
        var deleted = expenses[0];
        dal.DeleteExpense(expenses[0].Id);
        expenses = dal.GetAllExpenses();
        Print(expenses);

        // Console.WriteLine($"----with{categories[0].Name}--");
        // var some = expenses.Where(expense => expense.Categories.Contains(categories[0])).ToList();
        // Print(some);

        Console.WriteLine($"----then we add back {deleted.Name}--");
        dal.AddExpense(deleted, deleted.Categories);
        expenses = dal.GetAllExpenses();
        Print(expenses);
    }
}
