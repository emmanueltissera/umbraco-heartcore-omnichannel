namespace SocialMediaWebHookHandler.Models
{
    public class Product
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public bool IsOnSpecialToday { get; set; }
    }
}
