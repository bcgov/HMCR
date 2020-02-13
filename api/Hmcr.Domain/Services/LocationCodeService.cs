using Hmcr.Data.Database;
using Hmcr.Data.Repositories;
using Hmcr.Model.Dtos.LocationCode;
using Hmcr.Model.Dtos.ServiceArea;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface ILocationCodeService
    {
        Task<IEnumerable<LocationCodeDropDownDto>> GetLocationCodesAsync();
        
    }

    public class LocationCodeService : ILocationCodeService
    {
        private ILocationCodeRepository _locationCodeRepo;

        public LocationCodeService(ILocationCodeRepository locationCodeRepo)
        {
            _locationCodeRepo = locationCodeRepo;
        }

        public async Task<IEnumerable<LocationCodeDropDownDto>> GetLocationCodesAsync()
        {
            return await _locationCodeRepo.GetLocationCodes();
        }
    }
}
