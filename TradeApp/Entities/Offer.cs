using System.ComponentModel.DataAnnotations.Schema;

namespace TradeApp.Entities
{
    [Table("Offer")]
    public class Offer
    {
        public int Id { get; set; }
        public string PosterUsername { get; set; }
        public DateTime Created { get; set; } 
        public Item Item { get; set; }
        public int ItemId { get; set; }
        public string Comment { get; set; }

    }
}
