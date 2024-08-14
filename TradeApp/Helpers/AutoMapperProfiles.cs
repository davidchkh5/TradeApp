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
                .ForMember(idt => idt.OwnerId, i => i.MapFrom(s => s.Owner.Id))
                .ForMember(idt => idt.ItemPhotos, i => i.MapFrom(s => s.Photos));
            CreateMap<ItemPhoto, ItemPhotoDto>();
            CreateMap<UpdateUserDto, AppUser>()
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); //Checks all properties before mapping from UpdateUserDto to AppUser and data with null value will not be mapped
            CreateMap<Offer, OfferDto>() // Add this line for mapping from Offer to OfferDto
              .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<OfferDto, Offer>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }

    }
}
