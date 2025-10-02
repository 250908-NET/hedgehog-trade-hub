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
        # region Item
        CreateMap<Item, ItemDTO>();
        CreateMap<CreateItemDTO, Item>();
        CreateMap<UpdateItemDTO, Item>()
            // ignore null values for patch updates
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        #endregion
    }
}
