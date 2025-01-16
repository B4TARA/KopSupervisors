using KOP.DAL;
using KOP.Import;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NLog;

internal class Program
{
    private static void Main(string[] args)
    {
        var _config = new ConfigurationBuilder().AddJsonFile("C:\\PROJECTS\\KopSupervisors\\Configuration\\appsettings.json", optional: false, reloadOnChange: true).Build();

        var _nlogConfigPath = _config["FilePaths:NLogConfigPath"];
        LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration(_nlogConfigPath);
        var _logger = LogManager.GetCurrentClassLogger();

        var _dbConnectionString = _config["ConnectionStrings:WebApiDatabase"];
        var _optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        var _options = _optionsBuilder.UseNpgsql(_dbConnectionString).Options;

        var importer = new ExportAndImport(_options, _config);
        importer.TransferDataFromExcelToDatabase().Wait();
    }
}