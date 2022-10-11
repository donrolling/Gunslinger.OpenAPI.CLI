using AutoFixture;
using Microservice_API1.Models;
using Microsoft.AspNetCore.Mvc;

namespace Microservice_API1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CourseController : ControllerBase
	{
		private readonly Fixture _fixture;

		public CourseController()
		{
			_fixture = new Fixture();
		}

		[HttpGet]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Course> Get(int id)
		{
			var Course = _fixture.Build<Course>().Without(a => a.Id).Create();
			Course.Id = id;
			return Course;
		}

		[HttpPost]
		[ProducesResponseType(StatusCodes.Status201Created)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Course> Create(Course Course)
		{
			return Ok(Course);
		}

		[HttpPut]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult<Course> Update(Course Course)
		{
			return Ok(Course);
		}

		[HttpDelete]
		[ProducesResponseType(StatusCodes.Status200OK)]
		[ProducesResponseType(StatusCodes.Status400BadRequest)]
		public ActionResult Delete(int id)
		{
			return Ok();
		}
	}
}