using AutoMapper;
using Hmcr.Data.Database.Entities;
using Hmcr.Data.Repositories.Base;
using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hmcr.Data.Repositories
{
    public interface IUserRepository : IHmcrRepositoryBase<HmrSystemUser>
    {

    }

    public class UserRepository : HmcrRepositoryBase<HmrSystemUser>, IUserRepository
    {
        public UserRepository(AppDbContext dbContext, IMapper mapper)
            : base(dbContext, mapper)
        {

        }
    }
}
