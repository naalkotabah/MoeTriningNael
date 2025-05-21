using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moe.Core.Data;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.DTOs.User;
using Moe.Core.Models.Entities;
using Moe.Core.Extensions;
using Moe.Core.Models.DTOs.LocalizedContent;
using Moe.Core.Translations;
using Moe.Core.Models.DTOs.Warehouse;
using Moe.Core.Models.DTOs.Items;

namespace Moe.Core.Helpers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserDTO>()
     .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.StaticRole));





        CreateMap<UserFormDTO, User>();
        CreateMap<UserUpdateDTO, User>()
            .IgnoreNullAndEmptyGuids();

        CreateMap<Country, CountryDTO>();
        CreateMap<CountryFormDTO, Country>();
        CreateMap<CountryUpdateDTO, Country>()
            .IgnoreNullAndEmptyGuids();

        CreateMap<City, CityDTO>();
        CreateMap<CityFormDTO, City>();
        CreateMap<CityUpdateDTO, City>()
            .IgnoreNullAndEmptyGuids();

        CreateMap<LocalizedContent, LocalizedContentDTO>();
        CreateMap<LocalizedContentFormDTO, LocalizedContent>();
        CreateMap<LocalizedContentUpdateDTO, LocalizedContent>()
            .IgnoreNullAndEmptyGuids();

        CreateMap<Notification, NotificationDTO>();
        CreateMap<Notification, NotificationDTOSimplified>();
        CreateMap<NotificationForm, Notification>();
        
        
        //{{INSERTION_POINT}}
        
        CreateMap<SystemSettings,SystemSettingsDTO>();
        CreateMap<SystemSettingsFormDTO,SystemSettings>();
        CreateMap<SystemSettingsUpdateDTO,SystemSettings>()
            .IgnoreNullAndEmptyGuids();
        CreateMap<Permission,PermissionDTO>();
        
        CreateMap<Role,RoleDTO>();
        CreateMap<RoleFormDTO,Role>();
        CreateMap<RoleUpdateDTO,Role>()
            .IgnoreNullAndEmptyGuids();




        CreateMap<WarehouseFormDTO, Warehouse>()
       .ForMember(dest => dest.Admins, opt => opt.Ignore()); 

        CreateMap<Warehouse, WarehouseDTO>()
            .ForMember(dest => dest.AdminNames, opt => opt.MapFrom(src => src.Admins.Select(a => a.Name).ToList()));


        CreateMap<ItemFormDTO, Item>();
        CreateMap<Item, ItemDTO>();


        CreateMap<WarehouseItemFormDTO, WarehouseItem>()
      .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));



        // موجود مسبقًا
        CreateMap<InventoryMovementFormDTO, InventoryMovement>()
            .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.FromWarehouseId, opt => opt.MapFrom(src => src.FromWarehouseId))
            .ForMember(dest => dest.ToWarehouseId, opt => opt.MapFrom(src => src.ToWarehouseId))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

        // هذا هو المطلوب الآن
        CreateMap<InventoryMovementFormDTO, WarehouseItem>()
            .ForMember(dest => dest.ItemId, opt => opt.MapFrom(src => src.ItemId))
            .ForMember(dest => dest.WarehouseId, opt => opt.MapFrom(src => src.ToWarehouseId)) // لأنه المخزن الهدف
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));



    }
}