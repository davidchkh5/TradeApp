using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TradeApp.Dtos;
using TradeApp.Interfaces;

namespace TradeApp.Entities
{
    [Table("Items")]
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<ItemPhoto> Photos { get; set; } = new();
        public string MainPhotoUrl { get; set; }
        public int OwnerId { get; set; }
        public AppUser Owner { get; set; }
        public string Description { get; set; }
        public string TradeFor { get; set; }
        public string Type { get; set; }
        public List<Offer> Offers { get; set; } = new();

    }
}
