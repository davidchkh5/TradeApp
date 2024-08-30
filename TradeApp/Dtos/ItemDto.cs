using TradeApp.Entities;

namespace TradeApp.Dtos
{
    public class ItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ItemPhotoDto> ItemPhotos { get; set; }
        public string MainPhotoUrl { get; set; }
        public int OwnerId { get; set; }
        public string OwnerUsername { get; set; }
        public string Description { get; set; }
        public string TradeFor { get; set; }
        public string Type { get; set; }
        
        public List<OfferDto> Offers { get; set; }

    }
}
