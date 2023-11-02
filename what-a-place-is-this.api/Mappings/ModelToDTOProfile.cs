using AutoMapper;
using what_a_place_is_this.api.DTOs;
using what_a_place_is_this.api.Models;

namespace what_a_place_is_this.api.Mappings
{
    public class ModelToDTOProfile : Profile
    {
        public ModelToDTOProfile()
        {
            CreateMap<PlaceModel, PlaceDTO>().ReverseMap();
        }
    }
}
