using System;
using System.Configuration;
using System.Data;
using Npgsql;

public class DataAccess
{
    private readonly string _connString;

    public DataAccess()
    {
        _connString = ConfigurationManager.ConnectionStrings["SitePlannerDB"].ConnectionString;
    }

    public DataSet GetAllProjectsDataSet()
    {
        var ds = new DataSet();
        using (var conn = new NpgsqlConnection(_connString))
        {
            var adapter = new NpgsqlDataAdapter("SELECT * FROM projects ORDER BY id", conn);
            adapter.Fill(ds, "projects");
        }
        return ds;
    }

    public void UpdateProjectsFromDataSet(DataSet ds)
    {
        using (var conn = new NpgsqlConnection(_connString))
        {
            var adapter = new NpgsqlDataAdapter("SELECT * FROM projects", conn);
            var builder = new NpgsqlCommandBuilder(adapter);
            adapter.Update(ds, "projects");
        }
    }

    public DataSet GetTasksByProjectDataSet(int projectId)
    {
        var ds = new DataSet();
        using (var conn = new NpgsqlConnection(_connString))
        {
            var adapter = new NpgsqlDataAdapter("SELECT * FROM tasks WHERE project_id = @pid ORDER BY id", conn);
            adapter.SelectCommand.Parameters.AddWithValue("@pid", projectId);
            adapter.Fill(ds, "tasks");
        }
        return ds;
    }

    public void UpdateTasksFromDataSet(DataSet ds)
    {
        using (var conn = new NpgsqlConnection(_connString))
        {
            var adapter = new NpgsqlDataAdapter("SELECT * FROM tasks", conn);
            var builder = new NpgsqlCommandBuilder(adapter);
            adapter.Update(ds, "tasks");
        }
    }

    public DataTable GetProjectsByDeadline(DateTime from, DateTime to)
    {
        var dt = new DataTable();
        using (var conn = new NpgsqlConnection(_connString))
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM projects WHERE deadline BETWEEN @from AND @to ORDER BY deadline";
            cmd.Parameters.AddWithValue("@from", from);
            cmd.Parameters.AddWithValue("@to", to);
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                dt.Load(reader);
            }
        }
        return dt;
    }

    public DataTable GetProjectSummary(int projectId)
    {
        var dt = new DataTable();
        using (var conn = new NpgsqlConnection(_connString))
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM get_project_summary(@p)";
            cmd.Parameters.AddWithValue("@p", projectId);
            conn.Open();
            using (var reader = cmd.ExecuteReader())
            {
                dt.Load(reader);
            }
        }
        return dt;
    }

    public void InsertProject_Connected(string name, string client, decimal budget, DateTime? deadline, string status)
    {
        using (var conn = new NpgsqlConnection(_connString))
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "INSERT INTO projects (name, client, budget, deadline, status) VALUES (@name, @client, @budget, @deadline, @status)";
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@client", (object)client ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@budget", budget);
            cmd.Parameters.AddWithValue("@deadline", (object)deadline ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@status", status);
            conn.Open();
            cmd.ExecuteNonQuery();
        }
    }
}
