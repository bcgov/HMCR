using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.ServiceArea;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IServiceAreaService
    {
        Task<IEnumerable<ServiceAreaDto>> GetServiceAreaBySystemUserIdAsync(long systemUserId);
        Task<IEnumerable<ServiceAreaNumberDto>> GetAllServiceAreasAsync();
        Task<int> CountServiceAreaNumbersAsync(IEnumerable<decimal> serviceAreaNumbers);
    }

    public class ServiceAreaService : IServiceAreaService
    {
        private IServiceAreaRepository _svcAreaRepo;
        private IUnitOfWork _unitOfWork;

        public ServiceAreaService(IServiceAreaRepository svcAreaRepo, IUnitOfWork unitOfWork)
        {
            _svcAreaRepo = svcAreaRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ServiceAreaNumberDto>> GetAllServiceAreasAsync()
        {
            return await _svcAreaRepo.GetAllServiceAreasAsync();
        }

        public async Task<IEnumerable<ServiceAreaDto>> GetServiceAreaBySystemUserIdAsync(long systemUserId)
        {
            return await _svcAreaRepo.GetServiceAreaBySystemUserIdAsync(systemUserId);
        }

        public async Task<int> CountServiceAreaNumbersAsync(IEnumerable<decimal> serviceAreaNumbers)
        {
            return await _svcAreaRepo.CountServiceAreaNumbersAsync(serviceAreaNumbers);
        }
    }
}
