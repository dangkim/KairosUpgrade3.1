namespace Slot.BackOffice.Data.Authentication
{
    public static class BackOfficeClaimTypes
    {
        public static string UserId = "id";
        public static string Username = "username";
        public static string RoleId = "roleid";
        /// <summary>
        /// This is used for identifying the authorization of the user against the backend.
        /// Retrieval of this claim type value is crossed against the enums in the backend.
        /// </summary>
        public static string Role = "role";
        /// <summary>
        /// This is used for front-end display purposes.
        /// </summary>
        public static string RoleName = "rolename";
        public static string OperatorId = "operatorid";
        public static string Operator = "operatortag";
    }
}
