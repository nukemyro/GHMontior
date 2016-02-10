using GHModel;
using GHData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GHMontior.Controllers
{
    /// <summary>Controller for Health Updates</summary>
    /// <description>Handles all the GET and POST </description>
    public class HealthUpdateController : ApiController
    {
        GHContext _context = new GHContext();        

        /// <summary>Get all the Health Updates from the Database</summary>
        /// <returns>List of HealthUpdates (in JSON)</returns>
        [HttpGet]
        [Route("api/all")]
        public IEnumerable<HealthUpdate> GetAllUpdates()
        {
            return _context.HealthUpdates.ToList();
        }

        /// <summary>POST a Health Update to the Server</summary>
        /// <param name="anUpdate">A HealthUpdate object to save</param>
        /// <returns>OK if successful</returns>
        [HttpPost]
        [Route("api/updates")]
        public IHttpActionResult Post([FromBody]HealthUpdate anUpdate)
        {
            anUpdate.TimeStamp = DateTime.Now;
            //var db = GHModel.HealthUpdate.
            _context.HealthUpdates.Add(anUpdate);

            _context.SaveChanges();

            return Ok(anUpdate);
        }
    }
}
