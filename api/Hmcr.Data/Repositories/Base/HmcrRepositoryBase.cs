using AutoMapper;
using Hmcr.Model.Dtos;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Hmcr.Data.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using NetTopologySuite.Geometries;

namespace Hmcr.Data.Repositories.Base
{
    public interface IHmcrRepositoryBase<TEntity>
        where TEntity : class
    {
        TEntity Add<TDto>(TDto dto);
        Task<TEntity> AddAsync<TDto>(TDto dto);
        void Update<TDto>(long id, TDto dto);
        void Update<TDto>(string id, TDto dto);
        void Delete<TDto>(TDto dto);
        void Delete(Expression<Func<TEntity, bool>> where);
        TDto GetById<TDto>(long id);
        TDto GetById<TDto>(string id);
        Task<TDto> GetByIdAsync<TDto>(object id);
        IEnumerable<TDto> GetAll<TDto>();
        IEnumerable<TDto> GetAll<TDto>(Expression<Func<TEntity, bool>> where);
        Task<IEnumerable<TDto>> GetAllAsync<TDto>();
        Task<IEnumerable<TDto>> GetAllAsync<TDto>(Expression<Func<TEntity, bool>> where);
        Task<TDto> GetFirstOrDefaultAsync<TDto>(Expression<Func<TEntity, bool>> where);
        Task<PagedDto<TOutput>> Page<TInput, TOutput>(IQueryable<TInput> list, int pageSize, int pageNumber, string orderBy,string orderDir);
        Task<bool> ExistsAsync(object id);
    }

    public class HmcrRepositoryBase<TEntity> : IHmcrRepositoryBase<TEntity>
        where TEntity : class
    {
        #region Properties

        protected DbSet<TEntity> DbSet { get; private set; }

        protected AppDbContext DbContext { get; private set; }

        protected IMapper Mapper { get; private set; }

        #endregion

        public HmcrRepositoryBase(AppDbContext dbContext, IMapper mapper)
        {
            Mapper = mapper;
            DbContext = dbContext;
            DbSet = DbContext.Set<TEntity>();
        }

        #region Implementation
        public virtual TEntity Add<TDto>(TDto dto)
        {
            var result = Mapper.Map<TEntity>(dto);
            DbSet.Add(result);

            return result;
        }

        public async virtual Task<TEntity> AddAsync<TDto>(TDto dto)
        {
            var result = Mapper.Map<TEntity>(dto);
            await DbSet.AddAsync(result);

            return result;
        }

        public virtual void Update<TDto>(long id, TDto dto)
        {
            var entity = DbSet.Find(id);
            Mapper.Map(dto, entity);
        }

        public virtual void Update<TDto>(string id, TDto dto)
        {
            var entity = DbSet.Find(id);
            Mapper.Map(dto, entity);
        }

        public virtual void Delete<TDto>(TDto dto)
        {
            var result = Mapper.Map<TEntity>(dto);
            DbSet.Remove(result);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> where)
        {
            IEnumerable<TEntity> objects = DbSet.Where<TEntity>(where).AsEnumerable();
            foreach (TEntity obj in objects)
                DbSet.Remove(obj);
        }

        public virtual TDto GetById<TDto>(long id)
        {
            return Mapper.Map<TDto>(DbSet.Find(id));
        }

        public virtual TDto GetById<TDto>(string id)
        {
            return Mapper.Map<TDto>(DbSet.Find(id));
        }

        public virtual async Task<TDto> GetByIdAsync<TDto>(object id)
        {
            return Mapper.Map<TDto>(await DbSet.FindAsync(id));
        }
        public virtual IEnumerable<TDto> GetAll<TDto>()
        {
            return Mapper.Map<IEnumerable<TDto>>(DbSet.ToList());
        }

        public virtual IEnumerable<TDto> GetAll<TDto>(Expression<Func<TEntity, bool>> where)
        {
            return Mapper.Map<IEnumerable<TDto>>(DbSet.Where(where).ToList());
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync<TDto>()
        {
            return Mapper.Map<IEnumerable<TDto>>(await DbSet.ToListAsync());
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync<TDto>(Expression<Func<TEntity, bool>> where)
        {
            return Mapper.Map<IEnumerable<TDto>>(await DbSet.Where(where).ToListAsync());
        }

        public virtual async Task<IEnumerable<TDto>> GetAllNoTrackAsync<TDto>(Expression<Func<TEntity, bool>> where)
        {
            return Mapper.Map<IEnumerable<TDto>>(await DbSet.AsNoTracking().Where(where).ToListAsync());
        }

        public async Task<TDto> GetFirstOrDefaultAsync<TDto>(Expression<Func<TEntity, bool>> where)
        {
            return Mapper.Map<TDto>(await DbSet.Where(where).FirstOrDefaultAsync<TEntity>());
        }

        public async Task<bool> ExistsAsync(object id)
        {
            return await DbSet.FindAsync(id) != null;
        }

        public async Task<PagedDto<TOutput>> Page<TInput, TOutput>(IQueryable<TInput> list, int pageSize, int pageNumber, string orderBy, string direction = "")
        {
            var totalRecords = list.Count();

            if (pageNumber <= 0) pageNumber = 1;

            var pagedList = list.DynamicOrderBy($"{orderBy} {direction}") as IQueryable<TInput>;

            if (pageSize > 0)
            {
                var skipRecordCount = (pageNumber - 1) * pageSize;
                pagedList = pagedList.Skip(skipRecordCount)
                    .Take(pageSize);
            }

            var result = await pagedList.ToListAsync();

            IEnumerable<TOutput> outputList;

            if (typeof(TOutput) != typeof(TInput))
                outputList = Mapper.Map<IEnumerable<TInput>, IEnumerable<TOutput>>(result);
            else
                outputList = (IEnumerable<TOutput>) result;

            var pagedDTO = new PagedDto<TOutput>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalRecords,
                SourceList = outputList,
                OrderBy = orderBy,
                Direction = direction
            };

            return pagedDTO;
        }

        #endregion
    }
}
