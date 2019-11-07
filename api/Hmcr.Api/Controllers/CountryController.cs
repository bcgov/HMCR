using Hmcr.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Hmcr.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/countries")]
    public class CountryController : Controller
    {
        private ICountryService _countrService;

        public CountryController(ICountryService countryService)
        {
            _countrService = countryService;
        }

        [Route("{id}")]
        [HttpGet]
        public async Task<IActionResult> GetCountryAsync(int id)
        {
            var country = await _countrService.GetCountryByIdAsync(id);

            if (country == null)
                return NotFound();

            return Ok(country);
        }

        [Route("")]
        [HttpGet]
        public async Task<IActionResult> GetCountriesAsync()
        {
            return Ok(await _countrService.GetCountriesAsync());
        }
    }
}
