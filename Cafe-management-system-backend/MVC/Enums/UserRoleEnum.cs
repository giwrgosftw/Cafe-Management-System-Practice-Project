public enum UserRoleEnum
{
    Admin,
    User
}

/*
public enum UserRoleEnum
{
    [Description("Admin")]
    Admin,

    [Description("User")]
    User
}

public static class UserRoleEnumExtensions
{
    public static string ToDescriptionString(this UserRoleEnum role)
    {
        var fieldInfo = role.GetType().GetField(role.ToString());

        var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute));

        return attribute == null ? role.ToString() : attribute.Description;
    }
}
*/