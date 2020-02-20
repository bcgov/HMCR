using Hmcr.Chris;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Hmcr.Domain.Services
{
    public interface ISpatialService
    {

    }
    
    public class SpatialService : ISpatialService
    {
        private IMapsApi _mapsApi;
        private IOasApi _oasApi;

        public SpatialService(IMapsApi mapsApi, IOasApi oasApi)
        {
            _mapsApi = mapsApi;
            _oasApi = oasApi;
        }

        public async Task<(bool success, decimal offset)> ValidatePointAsync(decimal latitude, decimal longitude, string highwayUnique)
        {
            await Task.CompletedTask;
            
            return (true, 0);
        }
    }
}
