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
        _database = new LiteDatabase(dbPath, RegisterMappings());
    }

    private static BsonMapper RegisterMappings()
    {
        var mapper = new BsonMapper();

        mapper.RegisterType<VariableBase>(
            serialize: variable =>
            {
                // Serialize the object to a BsonDocument using LiteDB's global mapper
                var doc = BsonMapper.Global.ToDocument(variable);

                // Add type information to the document for future deserialization
                doc["_type"] = $"{variable.GetType().FullName}, {variable.GetType().Assembly.GetName().Name}";

                return doc;
            },
            deserialize: bson =>
            {
                // Extract the type information
                string typeName = bson["_type"].AsString;

                // Resolve the correct type from the type name
                Type type = Type.GetType(typeName) ?? throw new InvalidOperationException($"Cannot find the type: {typeName}");

                // Deserialize using the global mapper to ensure all properties are handled
                return (VariableBase)BsonMapper.Global.ToObject(type, (BsonDocument)bson);
            }
        );

        return mapper;
    }

    public ILiteCollection<RequestNode> RequestNodes => _database.GetCollection<RequestNode>("request_nodes");
    public ILiteCollection<VariableBase> GlobalVariables => _database.GetCollection<VariableBase>("global_variables");
    public ILiteCollection<Environment> Environments => _database.GetCollection<Environment>("environments");
    public ILiteCollection<Settings> Settings => _database.GetCollection<Settings>("settings");
}
