using BTG.Domain.Entities;
using BTG.Infrastructure.Persistence;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

public class MongoContext
{
    private readonly IMongoDatabase _db;

    static MongoContext()
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

        // Registrar mappings solo una vez
        if (!BsonClassMap.IsClassMapRegistered(typeof(Fondo)))
        {
            BsonClassMap.RegisterClassMap<Fondo>(cm =>
            {
                cm.AutoMap();
                cm.MapIdProperty(f => f.Id).SetElementName("_id");
                cm.MapMember(f => f.Nombre).SetElementName("nombre");
                cm.MapMember(f => f.MontoMinimo).SetElementName("montoMinimo");
                cm.MapMember(f => f.Categoria).SetElementName("categoria");
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Cliente)))
        {
            BsonClassMap.RegisterClassMap<Cliente>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdProperty(c => c.Id).SetElementName("_id");
                cm.MapMember(c => c.Nombre).SetElementName("nombre");
                cm.MapMember(c => c.Email).SetElementName("email");
                cm.MapMember(c => c.Telefono).SetElementName("telefono");
                cm.MapMember(c => c.Saldo).SetElementName("saldo");
            });


        }
        if (!BsonClassMap.IsClassMapRegistered(typeof(Transaccion)))
        {
            BsonClassMap.RegisterClassMap<Transaccion>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
                cm.MapIdProperty(t => t.Id).SetElementName("_id");
                cm.MapMember(t => t.ClienteId).SetElementName("clienteId");
                cm.MapMember(t => t.FondoId).SetElementName("fondoId");
                cm.MapMember(t => t.Tipo).SetElementName("tipo");
                cm.MapMember(t => t.Monto).SetElementName("monto");
                cm.MapMember(t => t.Fecha).SetElementName("fecha");
            });
        }
    }

    public MongoContext(IOptions<MongoOptions> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        _db = client.GetDatabase(options.Value.Database);
    }

    public IMongoCollection<Cliente> Clientes => _db.GetCollection<Cliente>("Clientes");
    public IMongoCollection<Fondo> Fondos => _db.GetCollection<Fondo>("fondos");
    public IMongoCollection<Transaccion> Transacciones => _db.GetCollection<Transaccion>("transacciones");
}
