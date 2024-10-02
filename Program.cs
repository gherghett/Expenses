using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Expenses.Models;

namespace Expenses;

class Program
{
    private static IExpenseDal _expenseDal;

    static void Main(string[] args)
    {
        // Initialize your DAL here. You can switch between ExpenseDapperDal and ExpenseDal
        // _expenseDal = new ExpenseDapperDal("your_connection_string");
        _expenseDal = new ExpenseDapperDal("Data Source=Expenses.db");

        while (true)
        {
            Console.WriteLine("\nExpense Management System");
            Console.WriteLine("1. Add Expense");
            Console.WriteLine("2. Add Category");
            Console.WriteLine("3. View All Expenses");
            Console.WriteLine("4. View All Categories");
            Console.WriteLine("5. View Expenses in a Category");
            Console.WriteLine("6. View Categories for an Expense");
            Console.WriteLine("7. Demo: Update Expense");
            Console.WriteLine("8. Demo: Delete Expense");
            
            Console.WriteLine("9. Exit");
            Console.Write("Choose an option: ");

            string choice = Console.ReadLine()!;

            switch (choice)
            {
                case "1":
                    AddExpense();
                    break;
                case "2":
                    AddCategory();
                    break;
                case "3":
                    ViewAllExpenses();
                    break;
                case "4":
                    ViewAllCategories();
                    break;
                case "5":
                    ViewExpensesInCategory();
                    break;
                case "6":
                    ViewCategoriesForExpense();
                    break;
                case "7":
                    DemoUpdateExpense();
                    break;
                case "8":
                    DemoDeleteExpense();
                    break;
                case "9":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    private static void AddCategory()
    {
        Console.Write("Enter category name: ");
        string name = Console.ReadLine()!;
        _expenseDal.AddCategory(name);
        Console.WriteLine("Added category");
        ViewAllCategories();
    }

    static void AddExpense()
    {
        Console.Write("Enter expense name: ");
        string name = Console.ReadLine()!;
        Console.Write("Enter expense description: ");
        string description = Console.ReadLine()!;
        Console.Write("Enter expense date (yyyy-MM-dd): ");
        DateTime date = DateTime.Parse(Console.ReadLine()!);

        Expense expense = new Expense { Name = name, Description = description, DateOfP = date };

        Console.Write("Enter category IDs (comma-separated) or press Enter for no categories: ");
        string categoryInput = Console.ReadLine() ?? "";
        var allCategories = _expenseDal.GetAllCategories();
        List<Category> categories = new List<Category>();

        if (!string.IsNullOrWhiteSpace(categoryInput))
        {
            try
            {
                var categoryIds = categoryInput.Split(',').Select(id => long.Parse(id)).ToList();
                categories = allCategories.Where(c => categoryIds.Contains(c.Id)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("invalid id format");
            }
        }

        bool success = _expenseDal.AddExpense(expense, categories);
        Console.WriteLine(success ? "Expense added successfully." : "Failed to add expense.");
    }

    static void ViewAllExpenses()
    {
        List<ExpenseView> expenses = _expenseDal.GetAllExpenses();
        foreach (var expense in expenses)
        {
            PrintExpense(expense);
        }
    }

    static void PrintExpense(ExpenseView expense) =>
            Console.WriteLine($"ID: {expense.Id}, Name: {expense.Name}, Date: {expense.DateOfP}, Description: {expense.Description} " 
                            + "Categories: " + string.Join(", ", expense.Categories.Select(c => c.Name)));
    

    static void ViewAllCategories()
    {
        List<Category> categories = _expenseDal.GetAllCategories();
        foreach (var category in categories)
        {
            Console.WriteLine($"ID: {category.Id}, Name: {category.Name}");
        }
    }

    static void ViewExpensesInCategory()
    {
        Console.Write("Enter category ID: ");
        long categoryId = long.Parse(Console.ReadLine());

        List<CategoryView> categoryViews = _expenseDal.GetAllCategoryViews();
        var category = categoryViews.FirstOrDefault(c => c.Id == categoryId);

        if (category != null)
        {
            Console.WriteLine($"Expenses in category '{category.Name}':");
            foreach (var expense in category.Expenses)
            {
                Console.WriteLine($"ID: {expense.Id}, Name: {expense.Name}, Date: {expense.DateOfP}, Description: {expense.Description}");
            }
        }
        else
        {
            Console.WriteLine("Category not found.");
        }
    }

    static void ViewCategoriesForExpense()
    {
        Console.Write("Enter expense ID: ");
        long expenseId = long.Parse(Console.ReadLine());

        List<ExpenseView> expenses = _expenseDal.GetAllExpenses();
        var expense = expenses.FirstOrDefault(e => e.Id == expenseId);

        if (expense != null)
        {
            Console.WriteLine($"Categories for expense '{expense.Name}':");
            foreach (var category in expense.Categories)
            {
                Console.WriteLine($"ID: {category.Id}, Name: {category.Name}");
            }
        }
        else
        {
            Console.WriteLine("Expense not found.");
        }
    }

    static void DemoUpdateExpense()
    {
        Console.WriteLine("Demonstrating expense update...");

        var allExpenses = _expenseDal.GetAllExpenses();
        var allCategories = _expenseDal.GetAllCategories();

        if (allExpenses.Count > 0)
        {
            var expense = allExpenses[0];
            PrintExpense(expense);
            Console.WriteLine($"Updating first expense: ID {expense.Id}, Name: {expense.Name}");
            expense.Name += " (Updated)";
            Console.WriteLine($"Removing a category, and adding another");
            if(expense.Categories.Count > 0)
            {
                // We remove the category from the list in the object
                expense.Categories.RemoveAt(0);
            }
            if(allCategories.Count > 0)
            {
                // And we add a category to the list in the object
                expense.Categories.Add(allCategories[0]);
            }
            // And this update-method will remove the category in the database aswell.
            // (a nano micro mini ORM)
            bool success = _expenseDal.UpdateExpense(expense);
            Console.WriteLine(success ? "Expense updated successfully." : "Failed to update expense.");
            PrintExpense(expense);
        }
        else
        {
            Console.WriteLine("No expenses to update.");
        }
    }

    static void DemoDeleteExpense()
    {
        Console.WriteLine("Demonstrating expense deletion...");
        List<ExpenseView> expenses = _expenseDal.GetAllExpenses();
        if (expenses.Count > 0)
        {
            var expense = expenses[expenses.Count - 1];
            Console.WriteLine($"Deleting last expense: ID {expense.Id}, Name: {expense.Name}");
            // This deletes every row in CategoryOfExpense aswell
            bool success = _expenseDal.DeleteExpense(expense.Id);
            Console.WriteLine(success ? "Expense deleted successfully." : "Failed to delete expense.");
        }
        else
        {
            Console.WriteLine("No expenses to delete.");
        }
    }
}
