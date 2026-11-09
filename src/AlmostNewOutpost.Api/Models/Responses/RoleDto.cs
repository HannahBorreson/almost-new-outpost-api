namespace AlmostNewOutpost.Api.Models.Responses
{ 
    /// <summary> This is the data we send for when someone logs in as a normal role like medic or citizen.   
    
    public class RoleDto
    {
        public int RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;

    }
}
