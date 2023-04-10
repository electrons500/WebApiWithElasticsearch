using Nest;
using WebApiWithElasticsearch.Data.Model;
using static System.Net.Mime.MediaTypeNames;

namespace WebApiWithElasticsearch.Data.Service
{
    public class TweetService
    {
        private ElasticClient _Client;
        private readonly ILogger<TweetService> _logger;

        public TweetService(ElasticClient client, ILogger<TweetService> logger)
        {
            _Client = client;
            _logger = logger;
        }

        //Insert data
        public bool AddTweet(TweetModel model)
        {
            //check if index exist
            string indexName = "tweetidx";
            var checkIfIndexExist = _Client.Indices.Exists(indexName);
            if (!checkIfIndexExist.Exists)
            {
                //create new index
                var createIndexResponse = _Client.Indices.Create(indexName, index => index.Map<TweetModel>(x => x.AutoMap()));
                if (createIndexResponse.IsValid)
                {
                    //log it
                    _logger.LogInformation("TweetIndex successfully created");
                }

            }

            var InsertData = _Client.Index(model, i => i.Index(indexName));//insert document with it index
                                                                           //or
                                                                           //use can use the code below to insert document into index tweetidx
                                                                           // var InsertData = _Client.Index(new IndexRequest<TweetModel>(model, indexName));
            if (InsertData.IsValid)
            {
                _logger.LogInformation("Data successfully saved");
                return true;
            }

            return false;
        }

        //Get tweets
        public TweetModel GetTweet(string id)
        {
            string indexName = "tweetidx";

            var response = _Client.Get<TweetModel>(id, x => x.Index(indexName));
            if (!response.IsValid)
            {
                return null;
            }
            return response.Source;
        }

        public List<TweetModel> GetAllTweet()
        {

            string indexName = "tweetidx";
            List<TweetModel> model = new();
            var response = _Client.Search<TweetModel>(s => s.Index(indexName)
                                                            .Query(q => q.MatchAll()));

            if (response.IsValid)
            {
                model = response.Documents.ToList();
            }
            return model;
        }


        //Update tweet
        public bool UpdateTweet(int id, TweetModel model)
        {
            string indexName = "tweetidx";
            var response = _Client.Update<TweetModel>(id, x => x.Index(indexName).Doc(model));
            if (response.IsValid)
            {
                return true;
            }

            return false;
        }

        //Delete tweet
        public bool DeleteTweet(int id)
        {
            string indexName = "tweetidx";
            var response = _Client.Delete<TweetModel>(id, x => x.Index(indexName));
            if (response.IsValid)
            {
                return true;
            }

            return false;
        }

        //Search by name
        public List<TweetModel> SearchByName(string Name)
        {
            string indexName = "tweetidx";
            List<TweetModel> model = new();
            
            //Do your search on one field
            var response = _Client.Search<TweetModel>(s => s.Index(indexName)
                                                           .From(0)
                                                           .Size(10)
                                                           .Query(q => q.Match(x => x.Field(f => f.User).Query(Name))));

            if (response.IsValid) 
            {
                model = response.Documents.ToList();
            }

            return model;
        }


         public List<TweetModel> MultipleFieldSearch(string keyText) 
        {
            string indexName = "tweetidx";
            List<TweetModel> model = new();
            
            //Do your search on more one field
            var response = _Client.Search<TweetModel>(s => s.Index(indexName)
                                                           .From(0)
                                                           .Size(10)
                                                           .Query(q => q.MultiMatch(mp => mp.Fields(f => f.Fields(f1 => f1.User,f2 => f2.Message)).Query(keyText))));

            if (response.IsValid) 
            {
                model = response.Documents.ToList();
            }

            return model;
        }





         //Search by name
        public List<TweetModel> SearchThroughAllField(string keyword)  
        { 
            string indexName = "tweetidx";
            List<TweetModel> model = new();
            var response = _Client.Search<TweetModel>(s => s.Index(indexName)
                                                            .From(0)
                                                            .Size(10)
                                                            .Query(q => q.QueryString(x => x.Query('*' + keyword + '*'))));


            if (response.IsValid) 
            {
                model = response.Documents.ToList();
            }

            return model;
        }


        //Index bulk tweets
        public bool InsertBulkData()
        {
            List<TweetModel> model = new();
            string indexName = "tweetidx";

            for(int i=4;i<20; i++)
            {
                TweetModel tweet = new()
                {
                    Id= i,
                    User = $"User {i}",
                    PostDate = Convert.ToDateTime(DateTime.Now.ToLongDateString()),
                    Message = $"This is a message for user {i}"
                };
                model.Add(tweet);
            }

            var response = _Client.IndexMany<TweetModel>(model, indexName);
           // //OR
            //var response = _Client.Bulk(x => x.Index(indexName).IndexMany(model));

            if (response.IsValid)
            {
                return true;
            }

            return false;
        }

        //Pagination

        public List<TweetModel> Pagination(int hitsToSkip,int pageSize) 
        {

            string indexName = "tweetidx";
            List<TweetModel> model = new();
            var response = _Client.Search<TweetModel>(s => s.Index(indexName)
                                                            .Query(q => q.MatchAll())
                                                            .From(hitsToSkip)
                                                            .Size(pageSize));
                                                            

            if (response.IsValid)
            {
                model = response.Documents.ToList();
            }
            return model;
        }


    }
}
