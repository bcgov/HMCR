using Hmcr.Domain.Services;
using Hmcr.Model.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/countries")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private ICountryService _countrService;

        public CountryController(ICountryService countryService)
        {
            _countrService = countryService;
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<ActionResult<CountryDto>> GetCountryAsync(int id)
        {
            var country = await _countrService.GetCountryByIdAsync(id);

            if (country == null)
                return NotFound();

            return Ok(country);
        }

        [Route("")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CountryDto>>> GetCountriesAsync()
        {
            return Ok(await _countrService.GetCountriesAsync());
        }
    }
}
