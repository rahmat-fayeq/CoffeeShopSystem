using AutoMapper;
using CoffeShopSystem.Models;
using CoffeShopSystem.ViewModels;

namespace CoffeShopSystem.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Table, TableVM>().ReverseMap();
            CreateMap<Menu, MenuVM>().ReverseMap();
        }
    }
}
