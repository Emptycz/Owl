using System.IO;
using LiteDB;
using Owl.Models;

namespace Owl.Contexts;

public class LiteDbContext : IDbContext
{
    private readonly LiteDatabase _database;

    public LiteDbContext(string? databasePath = null)
    {
        string dbPath = databasePath ?? Directory.GetCurrentDirectory() + "/Owl.db";
        _database = new LiteDatabase(dbPath);
    }

    public ILiteCollection<RequestNode> RequestNodes => _database.GetCollection<RequestNode>("request_nodes");
    public ILiteCollection<OwlVariable> GlobalVariables => _database.GetCollection<OwlVariable>("global_variables");
}
