using AutoMapper;
using BatDongSan_api.Models;
using BatDongSan_api.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

using System.Net;

namespace BatDongSan_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyImageController : ControllerBase
    {
        private readonly APIResponse _response;
        private readonly IPropertyImageRepository _proImageRepo;
        private readonly IMapper _mapper;

        public PropertyImageController(IPropertyImageRepository proImageRepo, IMapper mapper)
        {
            _response = new APIResponse();
            _proImageRepo = proImageRepo;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetPropertyImageByPropertyId(int id)
        {
            try
            {
                if(id <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors = new List<string> { "Invalid property id" };
                    return BadRequest(_response);
                }
                var property = await _proImageRepo.GetAllAsync(x => x.PropertyId == id);
                if (property == null || property.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors = new List<string>() { "Property Image not found" };
                    return NotFound(_response);
                }
                _response.Result = property;
                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
                return BadRequest(_response);
            }
        }
        
    }
}
