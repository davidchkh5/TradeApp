using TradeApp.Entities;

namespace TradeApp.Dtos
{
    public class OfferDto
    {
        public int Id { get; set; }
        public string PosterUsername { get; set; }
        public string Comment { get; set; }
        public DateTime Created { get; set; }
    }
}
