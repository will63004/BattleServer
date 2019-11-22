using Game.Service.Chat;
using Game.Service.HeartBeat;
using GlobalDefine;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson;
using MongoDB.Driver;
using ProtoService.Service;
using System.Threading;
using System.Threading.Tasks;
using TcpService.Service;

namespace Game
{
    public class GameHost : IHostedService
    {
        private readonly IServerDefine serverDefine;
        private readonly ITcpServer server;
        private readonly IMongoClient mongoClient;

        private readonly GrpcSetUp grpcSetUp;

        public GameHost(IServerDefine serverDefine, 
                        ITcpServer server, 
                        IMongoClient mongoClient,
                        GrpcSetUp grpcSetUp)
        {
            this.serverDefine = serverDefine;
            this.server = server;
            this.mongoClient = mongoClient;
            this.grpcSetUp = grpcSetUp;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            server.Start(serverDefine.IP, serverDefine.Port);

            grpcSetUp.SetUp(serverDefine.IP, serverDefine.GrpcPort);

            var database = mongoClient.GetDatabase("game");
            var collection = database.GetCollection<BsonDocument>("player");
            var document = new BsonDocument("name", "bbb");
            collection.InsertOne(document);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
