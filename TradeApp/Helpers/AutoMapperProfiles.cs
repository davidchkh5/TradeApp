using AutoMapper;
using TradeApp.Dtos;
using TradeApp.Entities;

namespace TradeApp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>();
            CreateMap<RegisterDto, AppUser>();
            CreateMap<Item, ItemDto>()
                .ForMember(idt => idt.OwnerUsername, i => i.MapFrom(s => s.Owner.UserName))
                .ForMember(idt => idt.OwnerId, i => i.MapFrom(s => s.Owner.Id));
            CreateMap<ItemPhoto, ItemPhotoDto>();
        }

    }
}
