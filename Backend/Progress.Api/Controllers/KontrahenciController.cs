using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Progress.Api.Controllers
{
	[Route("api/customer")]
	[ApiController]
	public class KontrahenciController : ControllerBase
	{
		[HttpGet("{id}")]
		public string Get(long id)
		{
			return "OK";
		}

		[HttpGet("search/{pattern}")]
		public string Search(string pattern) 
		{
			return "OK";

		}

		[HttpPost]
		public string Create() 
		{
			return "OK";
		}

		[HttpPut]
		public string Update()
		{
			return "OK";
		}

	}
}
