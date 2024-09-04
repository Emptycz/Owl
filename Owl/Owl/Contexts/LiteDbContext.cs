using System;
using System.IO;
using LiteDB;
using Owl.Models;
using Environment = Owl.Models.Environment;
using RequestNode = Owl.Models.RequestNode;

namespace Owl.Contexts;

public class LiteDbContext : IDbContext
{
    private readonly LiteDatabase _database;

    public LiteDbContext(string? databasePath = null)
    {
        string dbPath = databasePath ?? Directory.GetCurrentDirectory() + "/Owl.db";
        _database = new LiteDatabase(dbPath);

        RegisterMappings();
    }

    private static void RegisterMappings()
    {
        var mapper = new BsonMapper();

        // Register custom serialization and deserialization for IVariable
        mapper.RegisterType<IVariable>(
            serialize: variable =>
            {
                // Convert the variable to a BsonDocument with type information
                var doc = mapper.ToDocument(variable);
                doc["_type"] = variable.GetType().FullName; // Store type information
                return doc;
            },
            deserialize: bson =>
            {
                string? typeName = bson["_type"].AsString; // Retrieve type information
                var type = Type.GetType(typeName); // Get the type using the type name
                return (IVariable)mapper.ToObject(type, (BsonDocument)bson);
            }
        );
    }

    public ILiteCollection<RequestNode> RequestNodes => _database.GetCollection<RequestNode>("request_nodes");
    public ILiteCollection<OwlVariable> GlobalVariables => _database.GetCollection<OwlVariable>("global_variables");
    public ILiteCollection<Environment> Environments => _database.GetCollection<Environment>("environments");
    public ILiteCollection<Settings> Settings => _database.GetCollection<Settings>("settings");
}
