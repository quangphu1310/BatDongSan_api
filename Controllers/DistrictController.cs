using AutoMapper;
using BatDongSan_api.Models;
using BatDongSan_api.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BatDongSan_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistrictController : ControllerBase
    {
        private readonly IDistrictRepository _districtRepository;
        private readonly APIResponse _response;

        public DistrictController(IDistrictRepository districtRepository)
        {
            _response = new APIResponse();
            _districtRepository = districtRepository;
        }
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> GetDistrictByProvinceId(int id)
        {
            try
            {
                if(id <= 0)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors = new List<string> { "Invalid province id" };
                    return BadRequest(_response);
                }
                var district = await _districtRepository.GetAllAsync(x=>x.ProvinceId == id,includeProperties:"Province");
                if (district == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors = new List<string>() { "Districts not found" };
                    return NotFound(_response);
                }
                _response.Result = district;
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
