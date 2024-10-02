namespace Expenses.Models;
public class Category
{
    public long Id { get; set; }
    required public string Name { get; set; }

    public override bool Equals(object? other)
    {
        if (other is Category otherCategory)
        {
            return Id == otherCategory.Id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        string combined = "Category" + this.Id.ToString();
        return combined.GetHashCode();
    }
}

