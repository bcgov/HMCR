using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.CodeLookup;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Hmcr.Data.Repositories
{
    public interface ICodeLookupRepository
    {
        IEnumerable<CodeLookupCache> LoadCodeLookupCache();
    }

    public class CodeLookupRepository : HmcrRepositoryBase<HmrCodeLookup>, ICodeLookupRepository
    {
        public CodeLookupRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public IEnumerable<CodeLookupCache> LoadCodeLookupCache()
        {
            return DbSet.AsNoTracking()
                .Where(x => x.EndDate == null || DateTime.Today < x.EndDate)
                .Select(x =>
                    new CodeLookupCache
                    {
                        CodeSet = x.CodeSet,
                        CodeValue = x.CodeValueFormat == "NUMBER" ? x.CodeValueNum.ToString() : x.CodeValueText,
                        CodeName = x.CodeName,
                        CodeValueNum = x.CodeValueNum
                    }
                )
                .ToArray();
        }
    }
}