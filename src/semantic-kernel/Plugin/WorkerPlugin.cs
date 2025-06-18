using System.ComponentModel;
using System.Data.SQLite;
using System.Text;
using Microsoft.SemanticKernel;
using semantic_kernel.Data;

namespace semantic_kernel.Plugin;

public class WorkerPlugin
{
    private readonly string _dbPath = Path.Combine("Data", "model.sqlite");
    
    public WorkerPlugin()
    {
        Console.WriteLine("[WorkerPlugin] Initializing WorkerPlugin...");
    }

    [KernelFunction("SearchWorkers")]
    [Description("Gets Data From the Workers Table if we don't have any filter, it will return all the workers")]
    [return: Description("The list of workers")]
    public async Task<IList<Worker>> SearchCompaniesAsync(Kernel kernel,
        [Description("The name that will be to search, avoid sql injection, avoid or escape special characters")] string name,
        [Description("The name of the company to search workers, avoid sql injection")] string companyName)
    {  
        
        Console.WriteLine("[WorkerPlugin] Run SearchWorkers");
        var workers = new List<Worker>();
        var query = BuildTheQuery(name,companyName);
        Console.WriteLine("[WorkerPlugin] Generated SQL:\n" + query);

        if (string.IsNullOrWhiteSpace(query))
            return workers;

        using (var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
        {
            await conn.OpenAsync();
            using var cmd = new SQLiteCommand(query, conn);
            if(!string.IsNullOrWhiteSpace(name))
                cmd.Parameters.AddWithValue("@name", name);
            if(!string.IsNullOrWhiteSpace(companyName)) 
                cmd.Parameters.AddWithValue("@companyName", companyName);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (reader.Read())
            {
                var worker = new Worker
                {
                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                    Name = reader["Name"]?.ToString(),
                    CompanyId = reader["CompanyId"] != DBNull.Value ? Convert.ToInt32(reader["CompanyId"]) : 0,
                    // Add other properties of Worker as needed
                };
                workers.Add(worker);
            }

            await conn.CloseAsync();
        }

        return workers;
    }
    
    private string BuildTheQuery(string name , string companyName)
    {
        var query = new StringBuilder("");
        
        if (!string.IsNullOrWhiteSpace(companyName))
        {
            query.Append("SELECT * FROM Workers inner join Companys on Workers.CompanyId = Companys.Id WHERE LOWER(Companys.Name) LIKE '%' || LOWER(@companyName) || '%'");
        }
        else
        {
            query.Append("SELECT * FROM Workers WHERE 1=1");
        }
        
        if (!string.IsNullOrWhiteSpace(name))
        {
            query.Append(" AND LOWER(Name) LIKE '%' || LOWER(@name) || '%'");
        }
        
       
        
        return query.ToString();
    }
}