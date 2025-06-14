﻿using KOP.DAL;
using KOP.DAL.Interfaces;
using KOP.DAL.Repositories;
using KOP.EmailService;
using KOP.Import.Interfaces;
using KOP.Import.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace KOP.Import
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            // Регистрация провайдера кодировок
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            Console.InputEncoding = System.Text.Encoding.GetEncoding(1251); // Для CP1251
            Console.OutputEncoding = System.Text.Encoding.GetEncoding(1251); // Для CP1251

            // Настройка Serilog
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug() // Уровень логирования
                .WriteTo.Console()    // Логирование в консоль
                .WriteTo.File("C:/PROJECTS/KopSupervisors/logs/Import/log-.txt", rollingInterval: RollingInterval.Day) // Логирование в файл
                .CreateLogger();

            Log.Information("Импорт запущен");

            try
            {
                // Подключение файла конфигурации
                var _config = new ConfigurationBuilder()
                    .AddJsonFile("C:\\PROJECTS\\KopSupervisors\\Configuration\\appsettings.json", false, true)
                    .Build();

                var _dbConnectionString = _config["ConnectionStrings:WebApiDatabase"];

                if (string.IsNullOrEmpty(_dbConnectionString))
                {
                    throw new InvalidOperationException("Строка подключения к базе данных не найдена в конфигурации.");
                }

                // Настройка DI
                var serviceProvider = new ServiceCollection()
                    .AddSingleton(Log.Logger) // Регистрация логгера
                    .AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(_dbConnectionString)) // Регистрация DbContext
                    .AddScoped<IEmailSender, EmailSender>() // Регистрация почтового сервиса
                    .AddScoped<IExportAndImport, ExportAndImport>() // Регистрация сервиса импорта и экспорта
                    .AddScoped<INotificationService, NotificationService>() // Регистрация сервиса импорта и экспорта
                    .AddScoped<IUnitOfWork, UnitOfWork>() // Регистрация UnitOfWork
                    .AddSingleton<IConfiguration>(_config)
                    .BuildServiceProvider();

                var exportAndImport = serviceProvider.GetService<IExportAndImport>();

                if (exportAndImport == null)
                {
                    throw new InvalidOperationException("Сервис IExportAndImport не зарегистрирован в контейнере зависимостей.");
                }

                var notificationService = serviceProvider.GetService<INotificationService>();

                if (notificationService == null)
                {
                    throw new InvalidOperationException("Сервис INotificationService не зарегистрирован в контейнере зависимостей.");
                }

                //await exportAndImport.TransferDataFromExcelToDatabase();
                //await exportAndImport.CheckUsersForGradeProcess();
                //await notificationService.CheckUsersForNotifications();

                Log.Information("Импорт завершен");
            }
            catch (InvalidOperationException ex)
            {
                Log.Error(ex, "Ошибка конфигурации: {Message}", ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Произошла ошибка: {Message}", ex.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}