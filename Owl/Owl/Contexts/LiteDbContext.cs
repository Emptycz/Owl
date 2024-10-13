using System;
using System.IO;
using System.Security.Cryptography;
using LiteDB;
using Owl.Models;
using Owl.Models.Requests;
using Owl.Models.Variables;
using Environment = Owl.Models.Environment;

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

        mapper.RegisterType<IRequest>(
            serialize: request =>
            {
                var doc = BsonMapper.Global.ToDocument(request);
                doc["_type"] = $"{request.GetType().FullName}, {request.GetType().Assembly.GetName().Name}";
                return doc;
            },
            deserialize: bson =>
            {
                var typeName = bson["_type"].AsString;
                Type type = Type.GetType(typeName) ?? throw new InvalidOperationException($"Cannot find type {typeName}");
                return (IRequest)BsonMapper.Global.ToObject(type, (BsonDocument)bson);
            });

        mapper.RegisterType<IVariable>(
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
                var typeName = bson["_type"].AsString;

                // TODO: Instead of throwing, maybe use some generic default type
                // Resolve the correct type from the type name
                Type type = Type.GetType(typeName) ?? throw new InvalidOperationException($"Cannot find the type: {typeName}");

                // Deserialize using the global mapper to ensure all properties are handled
                return (IVariable)BsonMapper.Global.ToObject(type, (BsonDocument)bson);
            }
        );

        return mapper;
    }

    public ILiteCollection<IRequest> RequestNodes => _database.GetCollection<IRequest>("request_nodes");
    public ILiteCollection<IVariable> GlobalVariables => _database.GetCollection<IVariable>("global_variables");
    public ILiteCollection<Environment> Environments => _database.GetCollection<Environment>("environments");
    public ILiteCollection<Settings> Settings => _database.GetCollection<Settings>("settings");
}
