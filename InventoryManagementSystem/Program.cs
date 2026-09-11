using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

// b. Marker interface
public interface IInventoryEntity
{
    int Id { get; }
}

// a. Immutable Inventory Record
public record InventoryItem(int Id, string Name, int Quantity, DateTime DateAdded) : IInventoryEntity;

// c. Generic Inventory Logger
public class InventoryLogger<T> where T : IInventoryEntity
{
    private List<T> _log = new List<T>();
    private string _filePath;

    public InventoryLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Add(T item)
    {
        _log.Add(item);
    }

    public List<T> GetAll()
    {
        return _log;
    }

    public void SaveToFile()
    {
        try
        {
            using (var writer = new StreamWriter(_filePath))
            {
                string json = JsonSerializer.Serialize(_log);
                writer.Write(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving to file: {ex.Message}");
        }
    }

    public void LoadFromFile()
    {
        try
        {
            using (var reader = new StreamReader(_filePath))
            {
                string json = reader.ReadToEnd();
                var items = JsonSerializer.Deserialize<List<T>>(json);
                if (items != null)
                {
                    _log = items;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading from file: {ex.Message}");
        }
    }
}

// f. InventoryApp
public class InventoryApp
{
    private InventoryLogger<InventoryItem> _logger = new InventoryLogger<InventoryItem>("inventory.json");

    public void SeedSampleData()
    {
        _logger.Add(new InventoryItem(1, "Laptop", 10, DateTime.Now));
        _logger.Add(new InventoryItem(2, "Monitor", 15, DateTime.Now));
        _logger.Add(new InventoryItem(3, "Keyboard", 30, DateTime.Now));
        _logger.Add(new InventoryItem(4, "Mouse", 25, DateTime.Now));
        _logger.Add(new InventoryItem(5, "Printer", 5, DateTime.Now));
    }

    public void SaveData()
    {
        _logger.SaveToFile();
    }

    public void LoadData()
    {
        _logger.LoadFromFile();
    }

    public void PrintAllItems()
    {
        foreach (var item in _logger.GetAll())
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}, Date Added: {item.DateAdded}");
        }
    }
}

public class Program
{
    public static void Main()
    {
        var app = new InventoryApp();

        app.SeedSampleData();
        app.SaveData();

        // Simulate a new session
        app = new InventoryApp();

        app.LoadData();
        app.PrintAllItems();
    }
}
