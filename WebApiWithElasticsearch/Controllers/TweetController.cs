using Microsoft.AspNetCore.Mvc;
using WebApiWithElasticsearch.Data.Model;
using WebApiWithElasticsearch.Data.Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiWithElasticsearch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetController : ControllerBase
    {
        private TweetService _TweetService;
        public TweetController(TweetService tweetService)
        {
            _TweetService = tweetService;
        }

        // POST api/<TweetController>
        [HttpPost("AddTweet")]
        public ActionResult AddTweet([FromBody] TweetModel model)
        {
            bool result = _TweetService.AddTweet(model);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

       
        // GET api/<TweetController>/5
        [HttpGet("GetTweet/{id}")]
        public ActionResult GetTweet(string id)
        {
           var model =  _TweetService.GetTweet(id);
            return Ok(model);
        }

        [HttpGet("GetAllTweet")]
        public ActionResult GetAllTweet()
        {
            var model = _TweetService.GetAllTweet();
            return Ok(model);

        }

       

        // PUT api/<TweetController>/5
        [HttpPut("UpdateTweet/{id}")]
        public ActionResult UpdateTweet(int id,[FromBody] TweetModel model)
        {
            bool result = _TweetService.UpdateTweet(id,model);
            if (result)
            {
                return Ok(new { message = "Data successfully updated" });
            }
            return BadRequest();
        }

        // DELETE api/<TweetController>/5
        [HttpDelete("DeleteTweet/{id}")]
        public ActionResult DeleteTweet(int id) 
        {
            bool result = _TweetService.DeleteTweet(id);
            if (result)
            {
                return Ok(new { message = "Data successfully deleted" });
            }
            return BadRequest();
        }


        [HttpGet("SearchByTweetName")]
        public ActionResult SearchByTweetName([FromQuery] string tweetUsername) 
        {
            var model = _TweetService.SearchByName(tweetUsername); 
            return Ok(model);

        }

        [HttpGet("SearchAllfield")]
        public ActionResult SearchByTweetAllfield([FromQuery] string keyword) 
        {
            var model = _TweetService.SearchThroughAllField(keyword); 
            return Ok(model);

        }

        [HttpPost("InsertBulkData")]
        public ActionResult InsertBulkData() 
        {
            var result = _TweetService.InsertBulkData();
            if (result)
            {
                return Ok(new { message = "Data successfully inserted" });
            }
            return BadRequest();
        }

        [HttpGet("MultipleFieldSearch")]
        public ActionResult MultipleFieldSearch([FromQuery] string keyword)
        {
            var model = _TweetService.MultipleFieldSearch(keyword);
            return Ok(model);
             
        }

         [HttpGet("PaginateTweets")]
        public ActionResult PaginateTweets([FromQuery] int hitsToSkip)
        {
            //hitsToSkip starts from 5,10,15,20 etc. depend on the number of documents
            var model = _TweetService.Pagination(hitsToSkip,5);
            return Ok(model);

        }



    }
}
