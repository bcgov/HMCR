using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.User;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IUserService
    {
        Task<UserCurrentDto> GetCurrentUser();
    }
    public class UserService : IUserService
    {
        private IUserRepository _userRepo;
        private IUnitOfWork _unitOfWork;

        public UserService(IUserRepository userRepo, IUnitOfWork unitOfWork)
        {
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<UserCurrentDto> GetCurrentUser()
        {
            return await _userRepo.GetCurrentUser();
        }
    }
}
