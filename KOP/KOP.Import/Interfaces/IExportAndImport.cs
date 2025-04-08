namespace KOP.Import.Interfaces
{
    public interface IExportAndImport
    {
        Task TransferDataFromExcelToDatabase();
        Task CheckUsersForGradeProcess();
    }
}