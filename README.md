# Expenses
Detta projekt är främst för personligt bruk och lärande.

## Uppgift
Vi behöver bygga ett system som håller reda på utgifter! När en utgift läggs till ska vi kunna skriva in kostnad, datum, kategori och en beskrivning. Vi vill sen kunna visa alla utgifter, visa alla utgifter för en viss kategori, total kostnad, total kostnad per kategori.

## Lösning
Datan sparas med sqlite i Expenses.db, och läses av `ExpenseDal` klassen, som öven kallas om man ska göra någon CRUD. Databasen har tre tabeller, en för expenses en för categories och en som länkar samman de två, så att en expense kan ha fler än en kategori.

## Projektstruktur

### Models
i Models finns "entity" klasser som är baserade på tabellerna i databasen:
 - Category.cs
 - Expense.cs
 - CategoryOfExpense.cs

Och "Views" som ärver av entity-klasserna
- ExpenseView.cs
- CategoryView.cs
  
Enda skillnaden är att Viewsen har en lista som motsvarar;
 - för en utgift de associerade kategorierna, och
 - för en kategori de associerade utgifterna.

### IExpenseDal
Ett interface som håller reda på tre tabeller och översätter till vår app. I de konkreta implementationerna finns alla CRUD operationer för de två tabellerna, detta objektet föjer repository-mönstret.

Det finns två konkreta versioner:

## ExpenseDal
Stor klass för att hålla reda på tre tabeller, skriven med ADO.NET. 

## ExpenseDapperDal
Göra samma sak fast med dapper

## Använding

I Program.cs finns lite kod för att testa ändra på databasen.

# Expenses

[Previous content remains unchanged]

## Kodexempel (Code Examples)

Här är några exempel på hur du kan använda ExpenseDal-klassen för att interagera med databasen:

### Initiera ExpenseDal

```csharp
var dal = new ExpenseDal("Data Source=Expenses.db");
```

### Hämta alla utgifter (Expenses)

```csharp
List<ExpenseView> expenses = dal.GetAllExpenses();
```

### Hämta alla kategorier (Categories)

```csharp
List<Category> categories = dal.GetAllCategories();
```

### Hämta alla CategoryViews

```csharp
var catViews = dal.GetAllCategoryViews();
```

Notera skillnaden: `GetAllCategories()` returnerar en enkel lista med kategorier, medan `GetAllCategoryViews()` returnerar kategorier med tillhörande utgifter.

### Visa utgifter för varje kategori

```csharp
catViews.ForEach(category =>
    Console.WriteLine(
        category.Name + "\n\t" +
        string.Join("\n\t", category.Expenses
                                .Select(e => e.Name).ToArray()
        )
    )
);
```

### Lägga till en ny utgift

```csharp
List<Category> categories = dal.GetAllCategories();
dal.AddExpense(
    new Expense{
        Name = "Shotgun",
        Description = "Expensive, but oh so nice",
        DateOfP = DateTime.Now,
    },
    [categories[5], categories[2]]  // Lägg till i en specifik kategori
);
```

### Ta bort en utgift

```csharp
dal.DeleteExpense(expenses[0].Id);
```

### Ta bort en kategori

```csharp
dal.DeleteCategory(categories[2].Id);
```

### Visa alla utgifter med tillhörande kategorier

```csharp
static void Print(List<ExpenseView> expenseViews) =>
    expenseViews.ForEach(expensesItem =>
        Console.WriteLine(expensesItem.Name + "\n\t" +
                        string.Join("\n\t", expensesItem.Categories
                                            .Select(c => c.Name).ToArray()
                        )
        )
    );

// Använd funktionen så här:
List<ExpenseView> expenses = dal.GetAllExpenses();
Print(expenses);
```
```
Salt
        Groceries
Stuffff
        Groceries
        Expensive
Sugar
        Groceries
Large Computer
        Computer
        Large things
```

Detta projekt är främst för personligt bruk och lärande.
