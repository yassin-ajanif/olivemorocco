using System.Linq.Expressions;
using AutoMapper;
using FluentValidation;
using OliveMorocco.Business.DTOs;
using OliveMorocco.DataAccess.Repositories;
using OliveMorocco.Domain.Common;

namespace OliveMorocco.Business.Services;

public class GenericService<TEntity, TDto, TCreateDto, TUpdateDto>
    : IGenericService<TEntity, TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
{
    protected readonly IRepository<TEntity> Repo;
    protected readonly IMapper Mapper;
    protected readonly IValidator<TCreateDto>? CreateValidator;
    protected readonly IValidator<TUpdateDto>? UpdateValidator;

    public GenericService(
        IRepository<TEntity> repo,
        IMapper mapper,
        IEnumerable<IValidator<TCreateDto>> createValidators,
        IEnumerable<IValidator<TUpdateDto>> updateValidators)
    {
        Repo = repo;
        Mapper = mapper;
        CreateValidator = createValidators.FirstOrDefault();
        UpdateValidator = updateValidators.FirstOrDefault();
    }

    public async Task<TDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await Repo.GetByIdAsync(id, cancellationToken);
        return entity is null ? default : Mapper.Map<TDto>(entity);
    }

    public async Task<IReadOnlyList<TDto>> GetAllAsync(CancellationToken cancellationToken = default)
        => Mapper.Map<IReadOnlyList<TDto>>(await Repo.GetAllAsync(cancellationToken));

    public async Task<IReadOnlyList<TDto>> FindAsync(
        Expression<Func<TEntity, bool>> predicate,
        CancellationToken cancellationToken = default)
        => Mapper.Map<IReadOnlyList<TDto>>(await Repo.FindAsync(predicate, cancellationToken));

    public async Task<PagedResult<TResult>> QueryPagedAsync<TResult>(
        Expression<Func<TEntity, bool>>? predicate,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
        Expression<Func<TEntity, TResult>> selector,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var (items, totalCount) = await Repo.QueryPagedAsync(
            predicate, orderBy, selector, page, pageSize, cancellationToken);
        return new PagedResult<TResult>(items, totalCount);
    }

    public async Task<TDto> CreateAsync(TCreateDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(CreateValidator, dto, cancellationToken);
        var entity = await Repo.AddAsync(Mapper.Map<TEntity>(dto), cancellationToken);
        return Mapper.Map<TDto>(entity);
    }

    public async Task UpdateAsync(int id, TUpdateDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateAsync(UpdateValidator, dto, cancellationToken);
        var entity = await Repo.GetByIdAsync(id, cancellationToken)
            ?? throw new KeyNotFoundException();
        Mapper.Map(dto, entity);
        await Repo.UpdateAsync(entity, cancellationToken);
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        => Repo.DeleteAsync(id, cancellationToken);

    protected static async Task ValidateAsync<T>(
        IValidator<T>? validator,
        T instance,
        CancellationToken cancellationToken)
    {
        if (validator is null)
            return;

        await validator.ValidateAndThrowAsync(instance, cancellationToken);
    }
}
