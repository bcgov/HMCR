using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface IProvinceService
    {
        Task<IEnumerable<ProvinceDto>> GetProvincesAsync();
        Task AddProvinceAsync(ProvinceDto province);
        PagedDto<ProvinceDto> GetProvincesByPage(int pageSize, int pageNumber);
    }
    public class ProvinceService : IProvinceService
    {
        private IProvinceRepository _provinceRepo;
        private IUnitOfWork _unitOfWork;

        public ProvinceService(IProvinceRepository provinceRepo, IUnitOfWork unitOfWork)
        {
            _provinceRepo = provinceRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProvinceDto>> GetProvincesAsync()
        {
            return await _provinceRepo.GetAllAsync();
        }

        public async Task AddProvinceAsync(ProvinceDto province)
        {
            _provinceRepo.Add(province);
            await _unitOfWork.CommitAsync();
        }

        public PagedDto<ProvinceDto> GetProvincesByPage(int pageSize, int pageNumber)
        {
            return _provinceRepo.GetProvincesByPage(pageSize, pageNumber);
        }
    }
}
