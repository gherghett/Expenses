namespace Expenses.Models;

public class CategoryOfExpense
{
    public int Id { get; set;}
    public long CategoryId { get; set; }
    public long ExpenseId { get; set;}

    public override bool Equals(object? other)
    {
        if (other is CategoryOfExpense otherCategoryOfExpense)
        {
            return Id == otherCategoryOfExpense.Id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        string combined = "CategoryOfExpense" + this.Id.ToString();
        return combined.GetHashCode();
    }
}