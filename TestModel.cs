namespace TestMDCreator;

internal class TestModel
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Type { get; set; }
    public List<string>? Tags { get; set; }
    public List<string>? Steps { get; set; }

    public override string ToString()
    {
        return $"Name: {Name}, Description: {Description}, Type: {Type}, Tags: {Tags}, Steps: {Steps}";
    }
}