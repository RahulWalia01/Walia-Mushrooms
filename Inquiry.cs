namespace MushroomApi.Models
{
    public class Inquiry
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? OrderType { get; set; }
        public string? Product { get; set; }
        public string? Quantity { get; set; }
        public string? Message { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsRead { get; set; }
    }
}