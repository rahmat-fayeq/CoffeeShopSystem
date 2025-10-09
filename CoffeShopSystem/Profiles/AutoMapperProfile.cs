using AutoMapper;
using CoffeShopSystem.Models;
using CoffeShopSystem.ViewModels;
using static CoffeShopSystem.ViewModels.OrderVM;

namespace CoffeShopSystem.Profiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Table, TableVM>().ReverseMap();

            CreateMap<Menu, MenuVM>().ReverseMap();
            
            CreateMap<Order, OrderDetailsVM>()
                .ForMember(dest => dest.TableName, opt => opt.MapFrom(src => $"Table {src.Table.TableNumber}"))
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));

            CreateMap<OrderItem, OrderItemVM>()
                .ForMember(dest => dest.ItemName, opt => opt.MapFrom(src => src.Menu.Item))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.PriceAtOrder));
        }
    }
}
