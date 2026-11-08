namespace AlmostNewOutpost.Api.Models.Responses
{
    /// <summary>
    /// An Admin User DTO that contains identifiers related to the user and their assigned role
    /// </summary>
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime? ArchivedAtUtc { get; set; }
    }
}
