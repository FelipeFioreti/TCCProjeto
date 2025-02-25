using Microsoft.Data.SqlClient;

namespace TCCProjeto.DataBase
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            var json = File.ReadAllText("appsettings.json");
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json)!;
            ConnectionString = jsonObj!["ConnectionStrings"]["DefaultConnection"];
        }
        private string ConnectionString;
        public string GetConnectionString()
        {
            return ConnectionString;
        }

    }
}
