using AutoMapper;
using Azure;
using BatDongSan_api.Models;
using BatDongSan_api.Models.DTO;
using BatDongSan_api.Repository.IRepository;
using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;
        private readonly IPropertyImageRepository _proImageRepo;

        public PropertyController(IPropertyRepository proRepo, IMapper mapper, IConfiguration configuration, IPropertyImageRepository proImageRepo)
        {
            _response = new APIResponse();
            _proRepo = proRepo;
            _configuration = configuration;
            _mapper = mapper;
            _proImageRepo = proImageRepo;
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
                    .Include(x => x.PropertyImages)
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
        //[Consumes("multipart/form-data")]
        public async Task<ActionResult<APIResponse>> CreateProperty([FromForm] PropertyCreateDTO propertyDto)
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
                if (propertyDto.Files != null)
                {
                    foreach (var file in propertyDto.Files)
                    {
                        var propertyImage = new PropertyImage
                        {
                            PropertyId = property.Id
                        };
                        var cloudinary = new Cloudinary(new Account(
                        cloud: _configuration.GetSection("Cloudinary:CloudName").Value,
                        apiKey: _configuration.GetSection("Cloudinary:ApiKey").Value,
                        apiSecret: _configuration.GetSection("Cloudinary:ApiSecret").Value
                        ));
                        var uploadParams = new ImageUploadParams()
                        {
                            File = new FileDescription(file.FileName, file.OpenReadStream())
                        };
                        var uploadResult = cloudinary.Upload(uploadParams);
                        propertyImage.ImageUrl = uploadResult.Url.ToString();
                        propertyImage.ImagePublicId = uploadResult.PublicId;
                        await _proImageRepo.CreateAsync(propertyImage);
                    }
                }
                _response.Result = property;
                _response.StatusCode = HttpStatusCode.Created;
                _response.IsSuccess = true;

                return CreatedAtAction(nameof(GetPropertyById), new { id = property.Id }, _response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors = new List<string> { ex.InnerException?.Message ?? ex.Message };
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
        public async Task<ActionResult<APIResponse>> DeleteProperty(int id)
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
                var cloudinary = new Cloudinary(new Account(
                        cloud: _configuration.GetSection("Cloudinary:CloudName").Value,
                        apiKey: _configuration.GetSection("Cloudinary:ApiKey").Value,
                        apiSecret: _configuration.GetSection("Cloudinary:ApiSecret").Value
                        ));
                var oldImages = await _proImageRepo.GetAllAsync(x => x.PropertyId == id);
                foreach (var oldImage in oldImages)
                {
                    if (!string.IsNullOrEmpty(oldImage.ImagePublicId))
                    {
                        var deletionParams = new DeletionParams(oldImage.ImagePublicId);
                        var deletionResult = cloudinary.Destroy(deletionParams);
                        if (deletionResult.Result != "ok")
                        {
                            _response.Errors.Add($"Failed to delete image {oldImage.ImagePublicId} on Cloudinary.");
                        }
                    }

                    await _proImageRepo.RemoveAsync(oldImage);
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
