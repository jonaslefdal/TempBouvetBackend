using System;
using Microsoft.AspNetCore.Mvc;
using BouvetBackend.Models.TransportEntryModel;
using BouvetBackend.Entities;
using BouvetBackend.Repositories;

namespace BouvetBackend.Controllers 
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransportEntryController : Controller
    {
        private readonly ITransportEntryRepository _transportEntryRepository;
        private readonly IUserRepository _userRepository; 


        public TransportEntryController(ITransportEntryRepository transportEntryRepository, IUserRepository userRepository)
        {
            _transportEntryRepository = transportEntryRepository;
            _userRepository = userRepository;

        }

        [HttpPost("upsert")]
        public IActionResult Post([FromBody] TransportEntryModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid data.");
            }

            var user = _userRepository.GetUserByEmail(model.Email ?? "");


            var entity = new TransportEntry
            {
                UserId = user.UserId, 
                Method = model.Method,
                Points = model.Points,
                CreatedAt = DateTime.UtcNow
            };

            _transportEntryRepository.Upsert(entity);

            return Ok(new { message = "Data received successfully." });
        }
    }
}
