namespace Tweetero.API.DbContexts
{
    public static class ConnectionString
    {
        public static string GetConnectionString(WebApplicationBuilder builder)
        {
            string server = builder.Configuration["ConnectionStrings:TweeteroDBConnectionString:Server"];
            string dataBase = builder.Configuration["ConnectionStrings:TweeteroDBConnectionString:DataBase"];
            string user = builder.Configuration["ConnectionStrings:TweeteroDBConnectionString:User"];
            string password = builder.Configuration["ConnectionStrings:TweeteroDBConnectionString:Password"];

            string connectionString = $"Server={server};" +
                                      $"DataBase={dataBase};" +
                                      "Trusted_Connection=true;" +
                                      $"UserId={user};" +
                                      $"Password={password};";

            return connectionString;
        }
    }
}
