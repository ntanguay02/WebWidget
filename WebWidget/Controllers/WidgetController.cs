using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebWidget.Controllers
{
    [Route("api/v1/Widget")]
    [ApiController]
    public class WidgetController : ControllerBase
    {
        // GET: api/<WidgetController>
        [HttpGet()]
        public ActionResult<string> Get()
        {
            DataLayer dl = new();
            List<Widget> widgets = dl.GetWidgets();
            if(widgets == null)
            {
                return NotFound($"Widgets not found");
            }
            return Ok(widgets);
        }

        // GET api/<WidgetController>/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            DataLayer dl = new(); 
            Widget? widget = dl.GetWidgetById(id);
            if (widget == null)
            {
                return NotFound($"Widget {id} not found."); //status 404
            }

            //all is good, return the article
            return Ok(widget);
        }

        // POST api/<WidgetController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<WidgetController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<WidgetController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
