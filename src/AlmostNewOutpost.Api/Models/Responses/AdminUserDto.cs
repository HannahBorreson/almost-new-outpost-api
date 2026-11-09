namespace AlmostNewOutpost.Api.Models.Responses
{
    /// <summary> This is the data we send for an admin user.
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime? ArchivedAtUtc { get; set; }

    }
}
