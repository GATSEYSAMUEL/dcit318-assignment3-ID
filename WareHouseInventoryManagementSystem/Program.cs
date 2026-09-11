using System;
using System.Collections.Generic;
using System.Linq;

// a. Marker interface
public interface IInventoryItem
{
    int Id { get; }
    string Name { get; }
    int Quantity { get; set; }
}

// b. ElectronicItem
public class ElectronicItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public string Brand { get; }
    public int WarrantyMonths { get; }

    public ElectronicItem(int id, string name, int quantity, string brand, int warrantyMonths)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        Brand = brand;
        WarrantyMonths = warrantyMonths;
    }
}

// c. GroceryItem
public class GroceryItem : IInventoryItem
{
    public int Id { get; }
    public string Name { get; }
    public int Quantity { get; set; }
    public DateTime ExpiryDate { get; }

    public GroceryItem(int id, string name, int quantity, DateTime expiryDate)
    {
        Id = id;
        Name = name;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }
}

// e. Custom Exceptions
public class DuplicateItemException : Exception
{
    public DuplicateItemException(string message) : base(message) { }
}

public class ItemNotFoundException : Exception
{
    public ItemNotFoundException(string message) : base(message) { }
}

public class InvalidQuantityException : Exception
{
    public InvalidQuantityException(string message) : base(message) { }
}

// d. Generic Inventory Repository
public class InventoryRepository<T> where T : IInventoryItem
{
    private Dictionary<int, T> _items = new Dictionary<int, T>();

    public void AddItem(T item)
    {
        if (_items.ContainsKey(item.Id))
        {
            throw new DuplicateItemException($"Item with ID {item.Id} already exists.");
        }
        _items[item.Id] = item;
    }

    public T GetItemById(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
        return _items[id];
    }

    public void RemoveItem(int id)
    {
        if (!_items.ContainsKey(id))
        {
            throw new ItemNotFoundException($"Item with ID {id} not found.");
        }
        _items.Remove(id);
    }

    public List<T> GetAllItems()
    {
        return _items.Values.ToList();
    }

    public void UpdateQuantity(int id, int newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new InvalidQuantityException("Quantity cannot be negative.");
        }
        var item = GetItemById(id);
        item.Quantity = newQuantity;
    }
}

// f. WareHouseManager
public class WareHouseManager
{
    private InventoryRepository<ElectronicItem> _electronics = new InventoryRepository<ElectronicItem>();
    private InventoryRepository<GroceryItem> _groceries = new InventoryRepository<GroceryItem>();

    public void SeedData()
    {
        _electronics.AddItem(new ElectronicItem(1, "Laptop", 10, "Dell", 24));
        _electronics.AddItem(new ElectronicItem(2, "Smartphone", 15, "Samsung", 12));
        _electronics.AddItem(new ElectronicItem(3, "Headphones", 20, "Sony", 6));

        _groceries.AddItem(new GroceryItem(1, "Rice", 50, DateTime.Now.AddMonths(6)));
        _groceries.AddItem(new GroceryItem(2, "Milk", 30, DateTime.Now.AddDays(10)));
        _groceries.AddItem(new GroceryItem(3, "Bread", 25, DateTime.Now.AddDays(5)));
    }

    public void PrintAllItems<T>(InventoryRepository<T> repo) where T : IInventoryItem
    {
        foreach (var item in repo.GetAllItems())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
        }
    }

    public void IncreaseStock<T>(InventoryRepository<T> repo, int id, int quantity) where T : IInventoryItem
    {
        try
        {
            var item = repo.GetItemById(id);
            repo.UpdateQuantity(id, item.Quantity + quantity);
            Console.WriteLine($"Stock increased for item {id}. New quantity: {item.Quantity + quantity}");
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void RemoveItemById<T>(InventoryRepository<T> repo, int id) where T : IInventoryItem
    {
        try
        {
            repo.RemoveItem(id);
            Console.WriteLine($"Item {id} removed successfully.");
        }
        catch (ItemNotFoundException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public void RunDemo()
    {
        SeedData();

        Console.WriteLine("\n--- Grocery Items ---");
        PrintAllItems(_groceries);

        Console.WriteLine("\n--- Electronic Items ---");
        PrintAllItems(_electronics);

        Console.WriteLine("\n--- Testing Exceptions ---");

        try
        {
            _electronics.AddItem(new ElectronicItem(1, "Tablet", 5, "Apple", 12));
        }
        catch (DuplicateItemException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }

        RemoveItemById(_groceries, 99);

        try
        {
            _electronics.UpdateQuantity(1, -5);
        }
        catch (InvalidQuantityException ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}

public class Program
{
    public static void Main()
    {
        var manager = new WareHouseManager();
        manager.RunDemo();
    }
}