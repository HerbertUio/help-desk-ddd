namespace Domain.Enums.UserEnums;

[Flags]
public enum UserRoleEnum
{
    None = 0,
    Admin = 1,
    FrontLineAgent = 2,
    BackLineAgent = 3,
    Employee = 4
}

public static class UserRoleExtensions
{
    public static string GetDisplayName(this UserRoleEnum role)
    {
        return role switch
        {
            UserRoleEnum.Admin => "Administrador",
            UserRoleEnum.FrontLineAgent => "Agente de Primera Línea",
            UserRoleEnum.BackLineAgent => "Agente de Segunda Línea",
            UserRoleEnum.Employee => "Empleado",
            _ => role.ToString()
        };
    }
}