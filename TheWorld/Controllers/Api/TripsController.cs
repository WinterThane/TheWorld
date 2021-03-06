﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheWorld.Models;
using TheWorld.ViewModels;

namespace TheWorld.Controllers.Api
{
    [Route("api/trips")]
    public class TripsController : Controller
    {
        private IWorldRepository _repository;
        private ILogger<TripsController> _logger;

        public TripsController(IWorldRepository repository, ILogger<TripsController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Get()
        {
            try
            {
                var results = _repository.GetAllTrips();

                return Ok(Mapper.Map<IEnumerable<Trip>>(results));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to get trips: {ex}");

                return BadRequest("Error occured.");
            }           
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]TripViewModel theTrip)
        {
            if(ModelState.IsValid)
            {
                var newTrip = Mapper.Map<Trip>(theTrip);
                _repository.AddTrip(newTrip);

                if(await _repository.SaveChangesAsync())
                {
                    return Created($"api/trips/{theTrip.Name}", Mapper.Map<Trip>(newTrip));
                }
            }

            return BadRequest("Failed to save changes to DB.");
        }
    }
}
