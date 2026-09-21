using System.Text.Json;
using System.Text.Json.Serialization;

namespace TianzigeGenerator.Models;

/// <summary>
/// Represents the full document project with multiple pages.
/// </summary>
public class Project
{
    public string Name { get; set; } = "New Tianzige Project";
    public List<PageSettings> Pages { get; set; } = new();

    public static Project CreateDefault()
    {
        var project = new Project { Name = "My Practice Book" };
        // Add a tianzige grid page
        project.Pages.Add(new PageSettings
        {
            Type = PageType.TianzigeGrid,
            CellSizeMm = 15f
        });
        return project;
    }

    public string ToJson()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };
        return JsonSerializer.Serialize(this, options);
    }

    public static Project FromJson(string json)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() }
        };
        return JsonSerializer.Deserialize<Project>(json, options) ?? CreateDefault();
    }

    public void SaveToFile(string path)
    {
        File.WriteAllText(path, ToJson());
    }

    public static Project LoadFromFile(string path)
    {
        var json = File.ReadAllText(path);
        return FromJson(json);
    }
}
