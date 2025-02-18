using AutoMapper;
using Azure;
using BatDongSan_api.Models;
using BatDongSan_api.Models.DTO;
using BatDongSan_api.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace BatDongSan_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly APIResponse _response;
        private readonly IPropertyRepository _proRepo;
        private readonly IMapper _mapper;

        public PropertyController(IPropertyRepository proRepo, IMapper mapper)
        {
            _response = new APIResponse();
            _proRepo = proRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetProperties(
            [FromQuery] string? title,
            [FromQuery] int? room,
            [FromQuery] decimal? fromSquare,
            [FromQuery] decimal? toSquare,
            [FromQuery] string? province,
            [FromQuery] string? district,
            [FromQuery] decimal? fromPrice,
            [FromQuery] decimal? toPrice,
            int pageSize = 2, int pageNumber = 1
        )
        {
            try
            {
                IQueryable<Property> query = _proRepo.GetAllAsQueryable();

                if (!string.IsNullOrEmpty(title))
                {
                    query = query.Where(x => x.Title.Contains(title));
                }
                if (room.HasValue)
                {
                    query = query.Where(x => x.Rooms == room);
                }
                if (!string.IsNullOrEmpty(province))
                {
                    query = query.Include(x => x.District).ThenInclude(x=>x.Province).Where(x => x.District.Province.Name.Contains(province));
                }
                if (!string.IsNullOrEmpty(district))
                {
                    if (string.IsNullOrEmpty(province))
                    {
                        query = query.Include(x => x.District).Where(x => x.District.Name.Contains(district));
                    }
                    else
                    {
                        query = query.Include(x => x.District).Where(x => x.District.Name.Contains(district) && x.District.Province.Name.Contains(province));
                    }
                }
                if (fromSquare.HasValue)
                {
                    query = query.Where(x => x.Square >= fromSquare);
                }
                if (toSquare.HasValue)
                {
                    query = query.Where(x => x.Square <= toSquare);
                }
                if (fromPrice.HasValue)
                {
                    query = query.Where(x => x.Price >= fromPrice);
                }
                if (toPrice.HasValue)
                {
                    query = query.Where(x => x.Price <= toPrice);
                }

                int totalRecords = await query.CountAsync();

                var properties = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .Include(x => x.District.Province)
                    .ToListAsync();

                _response.Result = new
                {
                    TotalRecords = totalRecords,
                    PageSize = pageSize,
                    PageNumber = pageNumber,
                    Data = properties
                };
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

        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetPropertyById(int id)
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
                var property = await _proRepo.GetAsync(x => x.Id == id, includeProperties:"District.Province");
                if (property == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors = new List<string>() { "Property not found" };
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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> CreateProperty([FromBody] PropertyCreateDTO propertyDto)
        {
            try
            {
                if (propertyDto == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors = new List<string> { "Property is null" };
                    return BadRequest(_response);
                }

                var property = _mapper.Map<Property>(propertyDto);

                await _proRepo.CreateAsync(property);
                _response.Result = propertyDto;
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

                return CreatedAtAction(nameof(GetPropertyById), new { id = property.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.Message };
                return BadRequest(_response);
            }
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> UpdateProperty(int id, [FromBody] PropertyUpdateDTO propertyDto)
        {
            try
            {
                if (id < 0 || propertyDto == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors = new List<string> { "Invalid data" };
                    return BadRequest(_response);
                }
                var existingProperty = await _proRepo.GetAsync(u => u.Id == id);
                if (existingProperty == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors = new List<string> { "Property not found" };
                    return NotFound(_response);
                }
                _mapper.Map(propertyDto, existingProperty);

                await _proRepo.UpdateAsync(existingProperty);

                _response.Result = propertyDto;
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
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteUser(int id)
        {
            try
            {
                if (id < 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors = new List<string> { "Invalid property ID" };
                    return BadRequest(_response);
                }

                var user = await _proRepo.GetAsync(u => u.Id == id);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors = new List<string> { "Property not found" };
                    return NotFound(_response);
                }

                await _proRepo.RemoveAsync(user);

                _response.StatusCode = HttpStatusCode.OK;
                _response.IsSuccess = true;
                _response.Result = $"Property with ID {id} has been successfully deleted.";
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
