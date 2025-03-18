using AutoMapper;
using MeterReadingApp.Models;
using MeterReadingApp.Models.DTO;

namespace MeterReadingApp
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<MeterReading, MeterReadingDTO>();
            CreateMap<MeterReadingDTO, MeterReading>();
        }
    }
}
