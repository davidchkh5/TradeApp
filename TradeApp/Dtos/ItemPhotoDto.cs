using TradeApp.Entities;

namespace TradeApp.Dtos
{
    public class ItemPhotoDto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }
    }
}
