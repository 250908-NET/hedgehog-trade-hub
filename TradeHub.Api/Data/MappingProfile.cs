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
    }
}
