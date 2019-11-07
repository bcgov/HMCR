using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface ICountryService
    {
        Task<CountryDto> GetCountryByIdAsync(int countryId);

        Task<IEnumerable<CountryDto>> GetCountriesAsync();
    }
    public class CountryService : ICountryService
    {
        private ICountryRepository _countryRepo;
        private IUnitOfWork _unitOfWork;

        public CountryService(ICountryRepository countryRepo, IUnitOfWork unitOfWork)
        {
            _countryRepo = countryRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<CountryDto> GetCountryByIdAsync(int countryId)
        {
            return await _countryRepo.GetCountryByIdAsync(countryId);
        }

        public async Task<IEnumerable<CountryDto>> GetCountriesAsync()
        {
            return await _countryRepo.GetAllAsync();
        }
    }
}
