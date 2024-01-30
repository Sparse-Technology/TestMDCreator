using CommandLine;
using Microsoft.Extensions.Logging;

namespace TestMDCreator
{
    class Program
    {
        public static readonly string OUTPUT_FILE_NAME = "test.md";

        public class Options
        {
            [Option("debug", Required = false, Default = LogLevel.Warning, HelpText = "Set output to debug messages.")]
            public int DebugLevel { get; set; }

            [Option('f', "folder", Required = false, Default = "docs", HelpText = "Input folder to be processed.")]
            public string? DocsFolder { get; set; }

            [Option('n', "name", Required = false, Default = "test.md", HelpText = "Output file name.")]
            public string? OutputFileName { get; set; }

            [Option("force", Required = false, Default = false, HelpText = "Force overwrite of output file.")]
            public bool Force { get; set; }

            [Option("test-xlsx-file", Required = false, Default = null, HelpText = "Test xlsx file reader. Set to file name.")]
            public string? TestXlsxFile { get; set; }
        }

        private static ILogger? log;

        static void Main(string[] args)
        {
            _ = Parser.Default.ParseArguments<Options>(args).WithParsed<Options>(o =>
            {
                // Configure logging
                ConfigureLogging((LogLevel)o.DebugLevel);
                log?.LogInformation("Starting MD creator");

                #region Test xlsx file reader
                if (!string.IsNullOrEmpty(o.TestXlsxFile))
                {
                    try
                    {
                        var xlsxFileReader = new XlsxFileReader(log);
                        var test = xlsxFileReader.GetTestModel(o.TestXlsxFile);
                        log?.LogInformation($"Test Model: --- \n\n{test}\n\n ---");
                    }
                    catch (Exception ex)
                    {
                        log?.LogCritical(ex, "Error: {0}", ex.Message);
                    }
                    return;
                }
                #endregion

                // Create MD creator
                try
                {
                    #region Check arguments
                    if (string.IsNullOrEmpty(o.DocsFolder))
                        throw new ArgumentException("No folder specified");

                    if (!Directory.Exists(o.DocsFolder))
                        throw new ArgumentException($"Folder '{o.DocsFolder}' does not exist");

                    if (!Directory.GetFiles(o.DocsFolder, "*.xlsx").Any())
                        throw new ArgumentException($"Folder '{o.DocsFolder}' does not contain any .xlsx files");

                    if (string.IsNullOrEmpty(o.OutputFileName))
                        o.OutputFileName = Path.Combine(Directory.GetCurrentDirectory(), OUTPUT_FILE_NAME);

                    if (File.Exists(o.OutputFileName))
                    {
                        log?.LogInformation($"Output file '{o.OutputFileName}' already exists, deleting it");
                        if (!o.Force)
                            throw new ArgumentException($"Output file '{o.OutputFileName}' already exists, use --force to overwrite it");
                        File.Delete(o.OutputFileName);
                    }

                    if (!Directory.Exists(Path.GetDirectoryName(o.OutputFileName)))
                    {
                        var outputDirectory = Path.GetDirectoryName(o.OutputFileName);
                        if (!string.IsNullOrEmpty(outputDirectory))
                            Directory.CreateDirectory(outputDirectory);
                    }
                    #endregion

                    // Create MD engine
                    var mdCreator = new MarkDownFileCreator(log, o.DocsFolder, o.OutputFileName);
                    // Start MD engine
                    mdCreator.Start();

                    log?.LogInformation("MD creator finished");
                }
                catch (Exception ex)
                {
                    log?.LogCritical(ex, "Error arguments: {0}", ex.Message);
                }
            });

            log?.LogInformation("Press any key to exit");
            Console.ReadKey();
        }

        private static void ConfigureLogging(LogLevel logLevel = LogLevel.Warning)
        {
            // Create logger factory
            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddSimpleConsole(options =>
                {
                    options.SingleLine = true;
                    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.ff ";
                });
                builder.SetMinimumLevel(logLevel);
            });

            // Create logger
            log = loggerFactory.CreateLogger<Program>();
        }
    }
}