using AutoMapper;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Business.Mapping;

public class OperationnelProfile : Profile
{
    public OperationnelProfile()
    {
        CreateMap<CreateInterventionDto, Intervention>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Secteur, o => o.Ignore())
            .ForMember(d => d.Intrant, o => o.Ignore())
            .ForMember(d => d.Charges, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

        CreateMap<UpdateInterventionDto, Intervention>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Secteur, o => o.Ignore())
            .ForMember(d => d.Intrant, o => o.Ignore())
            .ForMember(d => d.Charges, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());
    }
}
