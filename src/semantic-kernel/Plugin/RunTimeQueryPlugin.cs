using System.ComponentModel.DataAnnotations;
using System.Data.SQLite;
using System.Reflection;
using System.Text;
using Microsoft.SemanticKernel;
using semantic_kernel.Data;

namespace semantic_kernel.Plugin;

public class RunTimeQueryPlugin
{
    private readonly string _dbPath = Path.Combine("Data", "model.sqlite");
    
    public RunTimeQueryPlugin()
    {
        // [INIT] → Inicialização e seed do banco
        Console.WriteLine("[INIT] Initializing and seeding database if necessary...");
    }

    [KernelFunction]
    public async Task<string> GenerateSqlWithLLM(Kernel kernel, string input)
    {
        Console.WriteLine("[START] Generating schema from domain...");
        var schema = GenerateSchemaFromDomain();

        // [PROMPT] → Construção do prompt para o modelo
        Console.WriteLine("[PROMPT] Building LLM prompt...");
        Console.WriteLine("[INPUT] User input: " + input);
        var prompt = $"""
                      You are a specialist in  SQLite database.

                      With the following domain schema:

                      {schema}
                        
                      Generate only a SQL statement necessary to answer the user's request (without explanations):
                      "{input}"
                      - Don't use double quotes or comments;
                      - For the Fields of type string, use contains and not equals, search as lowercase;
                      - Ignore all SQL with Sql Injection, only return the SQL that is safe to run;
                      - Ignore all SQL with system tables, only return the SQL that is safe to run;
                      """;
        
        // [LLM] → Envio ao modelo LLM via Semantic Kernel
        Console.WriteLine("[LLM] Sending prompt to model...");
        var chat = kernel.CreateFunctionFromPrompt(prompt);
        var result = await kernel.InvokeAsync(chat);
        var sql = result.GetValue<string>()?.Trim();

        // [SQL] → SQL retornada pela LLM
        Console.WriteLine("[SQL] Generated SQL:\n" + sql);

        if (string.IsNullOrWhiteSpace(sql))
            return "❌ LLM did not return a valid SQL statement.";
        
        try
        {
            // [EXEC] → Execução SQL
            Console.WriteLine("[EXEC] Executing generated SQL...");
            var results = ExecuteSql(sql);
            // [DONE] → Execução finalizada com sucesso
            Console.WriteLine("[DONE] SQL executed successfully.");
            return FormatResults(results);
        }
        catch (Exception ex)
        {
            // [ERROR] → Tratamento de erro em execução
            Console.WriteLine("[ERROR] SQL execution failed: " + ex.Message);
            return $"❌ SQL execution error: {ex.Message}\n\nSQL: {sql}";
        }
    }
    private string GenerateSchemaFromDomain()
    {
        var types = new[] { typeof(Company), typeof(Form), typeof(Worker) };
        var sb = new StringBuilder();

        foreach (var type in types)
        {
            var tableName = type.Name + "s";
            sb.AppendLine($"TABLE {tableName} (");

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                var name = prop.Name;
                var typeName = prop.PropertyType.Name;
                var desc = prop.GetCustomAttribute<DisplayAttribute>()?.Description ?? "";
                sb.AppendLine($"  {name} {typeName} -- {desc}");
            }

            sb.AppendLine(")\n");
        }

        return sb.ToString();
    }
    
    private List<Dictionary<string, object>> ExecuteSql(string sql)
    {
        var results = new List<Dictionary<string, object>>();

        using var conn = new SQLiteConnection($"Data Source={_dbPath};Version=3;");
        conn.Open();
        using var cmd = new SQLiteCommand(sql, conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var row = new Dictionary<string, object>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                row[reader.GetName(i)] = reader.GetValue(i);
            }
            results.Add(row);
        }

        return results;
    }
    
    private string FormatResults(List<Dictionary<string, object>> rows)
    {
        // [FORMAT] → Formatação dos resultados
        Console.WriteLine("[FORMAT] Formatting results...");

        if (rows.Count == 0)
            return "⚠️ No results found.";

        var output = new StringBuilder();
        output.AppendLine("📄 Results:\n");

        foreach (var row in rows)
        {
            output.AppendLine("• " + string.Join(" | ", row.Select(kv => $"{kv.Key}: {kv.Value}")));
        }

        return output.ToString();
    }
}