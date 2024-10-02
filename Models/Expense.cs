namespace Expenses.Models;
public class Expense
{
    public long Id { get; set;}
    required public string Name { get; set;}
    required public string Description { get; set;}
    public DateTime DateOfP { get; set;}

}