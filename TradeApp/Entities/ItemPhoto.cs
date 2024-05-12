using System.ComponentModel.DataAnnotations.Schema;

namespace TradeApp.Entities
{
    [Table("ItemPhotos")]
    public class ItemPhoto
    {
        public int Id { get; set; }
        public string PhotoUrl { get; set; }
        public bool IsMain { get; set; }
        public Item Item { get; set; }
        public int ItemId { get; set; }
    }
}
