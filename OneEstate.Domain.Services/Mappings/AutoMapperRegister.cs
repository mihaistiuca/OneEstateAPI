using AutoMapper;
using OneEstate.Application.Dtos;
using OneEstate.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OneEstate.Domain.Services.Mappings
{
    public static class AutoMapperRegister
    {
        public static void RegisterMappings(IMapperConfigurationExpression cfg)
        {
            cfg.CreateMap<User, UserBasicInfoDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            //cfg.CreateMap<Series, ProducerSeriesDto>();

            cfg.CreateMap<Project, ProjectViewDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));
            cfg.CreateMap<Project, ProjectListViewDto>().ForMember(a => a.Id, x => x.MapFrom(s => s.Id.ToString()));

        }
    }
}
