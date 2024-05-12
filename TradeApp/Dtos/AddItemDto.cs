using TradeApp.Entities;

namespace TradeApp.Dtos
{
    public class AddItemDto
    {
        public string Name { get; set; }
        public string ItemPhotoUrl { get; set; }
        public string Description { get; set; }
        public string TradeFor { get; set; }

    }
}
