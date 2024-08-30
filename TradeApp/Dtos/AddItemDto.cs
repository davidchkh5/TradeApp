using Microsoft.AspNetCore.Mvc;
using TradeApp.Entities;

namespace TradeApp.Dtos
{
    public class AddItemDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string TradeFor { get; set; }
        public string Type { get; set; }


        [FromForm(Name = "files")]
        public List<IFormFile> Files { get; set; }

    }
}
