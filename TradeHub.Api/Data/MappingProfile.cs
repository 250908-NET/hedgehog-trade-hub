using AutoMapper;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;

namespace TradeHub.Api.Data;

public class MappingProfile : Profile
{
    /// <remark>
    /// Parameters are assigned as from -> to.
    /// </remark>
    public MappingProfile()
    {
        #region Item

        CreateMap<Item, ItemDTO>();

        CreateMap<CreateItemDTO, Item>()
            .ForMember(dest => dest.Value, opt => opt.Ignore()) // set by service logic
            .ForMember(dest => dest.IsValueEstimated, opt => opt.Ignore()); // set by service logic

        CreateMap<UpdateItemDTO, Item>()
            .ForMember(dest => dest.Value, opt => opt.Ignore()) // set by service logic
            .ForMember(dest => dest.IsValueEstimated, opt => opt.Ignore()) // set by service logic
                                                                           // ignore null values for patch updates
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        #endregion

        #region User
           // User -> UserDto
        CreateMap<User, UserDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // UserDto -> User
        CreateMap<UserDto, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.OwnedItems, opt => opt.Ignore())
            .ForMember(dest => dest.InitiatedTrades, opt => opt.Ignore())
            .ForMember(dest => dest.ReceivedTrades, opt => opt.Ignore())
            .ForMember(dest => dest.Offers, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<RegisterUserDTO, User>();
        CreateMap<LoginDTO, User>();
        #endregion
    }
}
