using System;
using System.Web.Http;
using Projektr.WebApi;

namespace Projektr.Samples.WebApiSample
{
    public class FooController : ApiController
    {
        [Route("")]
        [HttpGet]
        public IHttpActionResult Get([FromUri,ProjektrFilter] string fields = null)
        {
            return Ok(new
            {
                FirstName = "Rick",
                LastName = "Astley",
                BirthPlace = "Newton-le-Willows, England, United Kingdom",
                BirthDate = "1966-02-06",
                TopHits = new[]
                {
                    new
                    {
                        Title = "Never Gonna Give You Up",
                        ReleaseYear = 1987,
                        Length = TimeSpan.Parse("0:3:31")
                    }
                }
            });
        }
    }
}