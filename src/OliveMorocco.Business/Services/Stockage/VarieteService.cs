using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using OliveMorocco.Business.DTOs.Stockage;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Entities.Operationnel;

namespace OliveMorocco.Business.Services.Stockage;

public sealed class VarieteService : IVarieteService
{
    private readonly IRepository<Variete> _varietes;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateVarieteDto>? _createValidator;

    public VarieteService(
        IRepository<Variete> varietes,
        IMapper mapper,
        IEnumerable<IValidator<CreateVarieteDto>> createValidators)
    {
        _varietes = varietes;
        _mapper = mapper;
        _createValidator = createValidators.FirstOrDefault();
    }

    public async Task<VarieteCreatedDto> CreateVarieteAsync(
        CreateVarieteDto dto,
        CancellationToken cancellationToken = default)
    {
        await ValidateAsync(_createValidator, dto, cancellationToken);
        await EnsureNomUniqueAsync(dto.Nom, cancellationToken);
        await EnsureCodeUniqueAsync(dto.Code, cancellationToken);

        var entity = _mapper.Map<Variete>(dto);
        await _varietes.AddAsync(entity, cancellationToken);

        return new VarieteCreatedDto(entity.Id, entity.Nom);
    }

    private async Task EnsureNomUniqueAsync(string nom, CancellationToken cancellationToken)
    {
        var normalized = nom.Trim();
        if (await _varietes.AnyAsync(v => v.Nom == normalized, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateVarieteDto.Nom),
                    "Une variété avec ce nom existe déjà."),
            ]);
        }
    }

    private async Task EnsureCodeUniqueAsync(string? code, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(code))
            return;

        var normalized = code.Trim();
        if (await _varietes.AnyAsync(v => v.Code == normalized, cancellationToken))
        {
            throw new ValidationException([
                new ValidationFailure(
                    nameof(CreateVarieteDto.Code),
                    "Une variété avec ce code existe déjà."),
            ]);
        }
    }

    private static async Task ValidateAsync<T>(
        IValidator<T>? validator,
        T instance,
        CancellationToken cancellationToken)
    {
        if (validator is null)
            return;

        var result = await validator.ValidateAsync(instance, cancellationToken);
        if (!result.IsValid)
            throw new ValidationException(result.Errors);
    }
}
