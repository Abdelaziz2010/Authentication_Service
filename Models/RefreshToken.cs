namespace Authentication_Service.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public DateTime Expires { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public DateTime? Revoked { get; set; }    // Track WHEN revocation occurred
        public bool IsActive => Revoked == null && !IsExpired;   // Computed property
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
