namespace Lost_Found.Models
{
    public class StandardUser : User
    {
        public ICollection<Listing> Listings { get; set; } = new List<Listing>();
        public ICollection<Claim> Claims { get; set; } = new List<Claim>();
    }
}
