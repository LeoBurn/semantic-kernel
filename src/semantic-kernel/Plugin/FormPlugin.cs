using System.ComponentModel;
using System.Data.SQLite;
using System.Text;
using Microsoft.SemanticKernel;
using semantic_kernel.Data;

namespace semantic_kernel.Plugin;

public class FormPlugin
{
    private readonly string _dbPath = Path.Combine("Data", "model.sqlite");
    
    public FormPlugin()
    {
        Console.WriteLine("[FormPlugin] Initializing FormPlugin...");
    }

    [KernelFunction("SearchForms")]
    [Description("Gets data from the Forms table")]
    [return: Description("The list of forms")]
    public async Task<IList<Form>> SearchForms(Kernel kernel,
        [Description("The name of the form to search, avoid SQL injection,avoid or escape special characters")]
        string formName,
        [Description("The company name to filter forms, avoid SQL injection,avoid or escape special characters")]
        string companyName,
        [Description("The worker name to filter forms, avoid SQL injection,avoid or escape special characters")]
        string workerName)
    {
        Console.WriteLine("[FormPlugin] Run SearchForms");
        var forms = new List<Form>();
        var query = BuildTheQuery(formName,companyName,workerName);

        Console.WriteLine("[FormPlugin] Generated SQL:\n" + query);

        if (string.IsNullOrWhiteSpace(query))
            return forms;

        using (var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;"))
        {
            await conn.OpenAsync();
            using var cmd = new SQLiteCommand(query, conn);
            if(!string.IsNullOrWhiteSpace(formName))
                cmd.Parameters.AddWithValue("@formName", formName);
            if(!string.IsNullOrWhiteSpace(companyName)) 
                cmd.Parameters.AddWithValue("@companyName", companyName);
            if(!string.IsNullOrWhiteSpace(workerName)) 
                cmd.Parameters.AddWithValue("@workerName", workerName);
            using var reader = await cmd.ExecuteReaderAsync();
            
            while (reader.Read())
            {
                var form = new Form
                {
                    Id = reader["Id"] != DBNull.Value ? Convert.ToInt32(reader["Id"]) : 0,
                    Name = reader["Name"]?.ToString(),
                    Type = reader["Type"]?.ToString(),
                    CompanyId = reader["CompanyId"] != DBNull.Value ? Convert.ToInt32(reader["CompanyId"]) : 0,
                    WorkerId = reader["WorkerId"] != DBNull.Value ? Convert.ToInt32(reader["WorkerId"]) : 0,
                    // Add other properties of Worker as needed
                };
                forms.Add(form);
            }

            await conn.CloseAsync();
        }

        return forms;
    }

    private string BuildTheQuery(string formName, string companyName,string workerName)
    {
        var whereQuery = new StringBuilder("WHERE 1 =1 ");
        var fromQuery = new StringBuilder(" FROM Forms ");

        if (!string.IsNullOrWhiteSpace(workerName))
        {
            fromQuery.Append(" inner join Workers on Forms.WorkerId = Workers.Id ");
            whereQuery.Append(" AND Workers.Name LIKE '%' || LOWER(@workerName) || '%'");
        }
        if (!string.IsNullOrWhiteSpace(companyName))
        {
            fromQuery.Append(" inner join Companys on Forms.CompanyId = Companys.Id ");
            whereQuery.Append(" AND Companys.Name LIKE '%' || LOWER(@companyName) || '%'");
        }
        if (!string.IsNullOrWhiteSpace(formName))
        {
            whereQuery.Append(" AND Forms.Name LIKE '%' || LOWER(@formName) || '%'");
        }
        
        return "SELECT * " + fromQuery.ToString() + whereQuery.ToString();
    }
}