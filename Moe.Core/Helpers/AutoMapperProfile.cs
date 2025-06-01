using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moe.Core.Data;
using Moe.Core.Models.DTOs;
using Moe.Core.Models.DTOs.User;
using Moe.Core.Models.Entities;
using Moe.Core.Extensions;
using Moe.Core.Models.DTOs.LocalizedContent;
using Moe.Core.Translations;

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

        CreateMap<WarehouseItemTransaction, WarehouseItemTransactionDTO>();
        CreateMap<WarehouseItemTransactionFormDTO, WarehouseItemTransaction>()
            .IgnoreNullAndEmptyGuids();




        CreateMap<Item,ItemDTO>();
        CreateMap<ItemFormDTO,Item>();
        CreateMap<ItemUpdateDTO,Item>()
            .IgnoreNullAndEmptyGuids();
        
        CreateMap<Warehouse,WarehouseDTO>();
        CreateMap<WarehouseFormDTO,Warehouse>();
        CreateMap<WarehouseUpdateDTO,Warehouse>()
            .IgnoreNullAndEmptyGuids();
        
        CreateMap<SystemSettings,SystemSettingsDTO>();
        CreateMap<SystemSettingsFormDTO,SystemSettings>();
        CreateMap<SystemSettingsUpdateDTO,SystemSettings>()
            .IgnoreNullAndEmptyGuids();
        CreateMap<Permission,PermissionDTO>();
        
        CreateMap<Role,RoleDTO>();
        CreateMap<RoleFormDTO,Role>();
        CreateMap<RoleUpdateDTO,Role>()
            .IgnoreNullAndEmptyGuids();
    }
}