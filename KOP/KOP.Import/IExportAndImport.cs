namespace KOP.Import
{
    public interface IExportAndImport
    {
        Task CheckUsersForNotifications();
        Task TransferDataFromExcelToDatabase();
        Task CheckUsersForGradeProcess();
    }
}