namespace Lost_Found.Models
{
    public class Admin : User
    {
        public ICollection<Listing> SupervisedListings { get; set; } = new List<Listing>();
    }
}
