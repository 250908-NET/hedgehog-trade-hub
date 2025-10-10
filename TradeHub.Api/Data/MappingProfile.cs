using AutoMapper;
using TradeHub.API.Models;
using TradeHub.API.Models.DTOs;

namespace TradeHub.API.Data;

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
        // User -> UserDTO
        CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        // UserDTO -> User
        CreateMap<UserDTO, User>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Username))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.OwnedItems, opt => opt.Ignore())
            .ForMember(dest => dest.InitiatedTrades, opt => opt.Ignore())
            .ForMember(dest => dest.ReceivedTrades, opt => opt.Ignore())
            .ForMember(dest => dest.Offers, opt => opt.Ignore())
            .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<RegisterUserDTO, User>();
        CreateMap<LoginDto, User>();
        #endregion
    }
}
