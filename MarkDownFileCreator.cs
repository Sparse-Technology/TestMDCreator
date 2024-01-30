namespace TestMDCreator;

using Microsoft.Extensions.Logging;

internal class MarkDownFileCreator
{
    private readonly ILogger? log;
    private readonly string docsFolder;
    private readonly string outputFileName;
    private XlsxFileReader xlsxFileReader;

    public MarkDownFileCreator(ILogger? log, string docsFolder, string outputFileName)
    {
        this.log = log;
        this.docsFolder = docsFolder;
        this.outputFileName = outputFileName;

        xlsxFileReader = new XlsxFileReader(log);
    }

    public void Start()
    {
        string document = "";
        log?.LogInformation($"Starting MD creator for folder '{docsFolder}'");
        int index = 0;
        foreach (var file in Directory.GetFiles(docsFolder, "*.xlsx"))
        {
            log?.LogInformation($"Creating MD for file [{++index}] '{file}'");
            var test = xlsxFileReader.GetTestModel(file);
            var md = CreateTestMD(test);
            document += md + "\n\n";
        }

        log?.LogInformation($"Writing MD to file '{outputFileName}'");
        File.WriteAllText(outputFileName, document);
    }

    private string CreateTestMD(TestModel test)
    {
        return $"# {test.Name}\n\n{test.Description}\n\n## Type\n\n{test.Type}\n\n" +
            $"## Tags\n\n{string.Join(", ", test.Tags ?? [])}\n\n" +
            $"## Steps\n\n{string.Join("\n", test.Steps ?? [])}\n\n";
    }
}
