using AutoMapper;
using OliveMorocco.Business.DTOs.Operationnel;
using OliveMorocco.Domain.Entities.Achat;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Business.Mapping;

public class OperationnelProfile : Profile
{
    public OperationnelProfile()
    {
        CreateMap<CreateInterventionDto, Intervention>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Secteur, o => o.Ignore())
            .ForMember(d => d.Charges, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));

        CreateMap<CreateInterventionLigneDto, InterventionLigne>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.InterventionId, o => o.Ignore())
            .ForMember(d => d.Intervention, o => o.Ignore())
            .ForMember(d => d.Intrant, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

        CreateMap<UpdateInterventionDto, Intervention>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.Secteur, o => o.Ignore())
            .ForMember(d => d.Charges, o => o.Ignore())
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore())
            .ForMember(d => d.Lignes, o => o.MapFrom(s => s.Lignes));

        CreateMap<CreateInterventionChargeDto, Charge>()
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.InterventionId, o => o.Ignore())
            .ForMember(d => d.Intervention, o => o.Ignore())
            .ForMember(d => d.TypeCharge, o => o.Ignore())
            .ForMember(d => d.Libelle, o => o.MapFrom(s => s.Libelle.Trim()))
            .ForMember(d => d.Note, o => o.MapFrom(s => s.Note ?? string.Empty))
            .ForMember(d => d.CreatedAt, o => o.Ignore())
            .ForMember(d => d.UpdatedAt, o => o.Ignore())
            .ForMember(d => d.CreatedByUserId, o => o.Ignore());

    }
}
