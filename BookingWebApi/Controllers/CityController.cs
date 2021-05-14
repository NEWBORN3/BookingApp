using System.Text.RegularExpressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Interfaces;
using WebApi.Dtos;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [Authorize]
    public class CityController : BaseController
    {
        private readonly IUnitOfWork uwo;
        private readonly IMapper mapper;
        public CityController(IUnitOfWork uwo, IMapper mapper)
        {
            this.mapper = mapper;
            this.uwo = uwo;
        }
        [HttpGet("GetAllCities")]
        public async Task<IActionResult> GetCities()
        {
            
            var cities = await uwo.CityRepository.GetCitiesAsync();
            var citiesDto = mapper.Map<IEnumerable<CityDto>>(cities);
            return Ok(citiesDto);
        }
        [HttpPost("AddCity")]
        public async Task<IActionResult> AddCity(CityDto cityDto)
        {
            var city = mapper.Map<City>(cityDto);
            city.LastUpdated = DateTime.Now;
            city.LastUpdatedBy = 6;
            uwo.CityRepository.AddCity(city);
            await uwo.SaveAsync();
            return StatusCode(201);
        }
        [AllowAnonymous]
        [HttpGet("getCity/{id}")]
        public async Task<IActionResult> GetAcity(int id)
        {
            var city = await uwo.CityRepository.FindCity(id);
            var cityDto = mapper.Map<CityDto>(city);
            return Ok(city);
        }


        [HttpPut("updateCity/{id}")]
        public async Task<IActionResult> UpdateCity(int id, CityDto cityDto)
        {

            var city = await uwo.CityRepository.FindCity(id);
            city.LastUpdated = DateTime.Now;
            city.LastUpdatedBy = 6;
            mapper.Map(cityDto,city);
            await uwo.SaveAsync();      
            return StatusCode(200);
        }

        [HttpPut("updateCityName/{id}")]
        public async Task<IActionResult> UpdateCity(int id, CityNameDto cityNameDto)
        {
            var city = await uwo.CityRepository.FindCity(id);
            city.LastUpdated = DateTime.Now;
            city.LastUpdatedBy = 6;
            mapper.Map(cityNameDto,city);
            await uwo.SaveAsync();      
            return StatusCode(200);
        }

        [HttpDelete("deleteCity/{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            uwo.CityRepository.DeleteCity(id);
            await uwo.SaveAsync();
            return Ok(id);
        }

    }
}