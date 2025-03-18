namespace KOP.Common.Enums
{
    public enum StatusCodes
    {
        OK = 200,
        UserExistsInMultipleDatabases = 210,
        Redirect = 300,
        BadRequest = 400,
        EntityNotFound = 404,
        InternalServerError = 500,
        IncorrectPassword = 401,
        UserIsSuspended = 403,
    }
}