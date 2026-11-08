namespace AlmostNewOutpost.Api.Models.Responses
{
    /// <summary>
    /// A role DTO that contains RoleId and RoleName
    /// </summary>
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }
}
