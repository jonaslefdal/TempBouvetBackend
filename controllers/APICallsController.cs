using Microsoft.AspNetCore.Mvc;
using BouvetBackend.Models.ApiModel;
using BouvetBackend.Entities;
using BouvetBackend.Repositories;


namespace BouvetBackend.Controllers 
{
[Route("api/[controller]")]
[ApiController]
public class ApiController : Controller
{
    private readonly IApiRepository _apiRepository;

    public ApiController(IApiRepository apiRepository)
    {
        _apiRepository = apiRepository;
    }

    [HttpPost("upsert")]
    public IActionResult Post([FromBody] ApiFullModel apiModel)
    {
        if (apiModel == null || apiModel.UpsertModel == null)
        {
            return BadRequest("Invalid data.");
        }

        var entity = new API
        {
            apiId = apiModel.UpsertModel.apiId,
            value1 = apiModel.UpsertModel.value1,
            value2 = apiModel.UpsertModel.value2
        };
        

        _apiRepository.Upsert(entity);

        return Ok(new { message = "Data received successfully." });
    }
}
}