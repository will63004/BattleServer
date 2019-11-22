using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.DependencyInjection;
using Game.Service.Chat;
using ProtoService.Service;
using Game.Service.Login;
using InterfaceAdapters.Service.Login;
using InterfaceAdapters.Service.Chat;
using Game.Service.HeartBeat;
using TcpService.Service;
using ServerNormal.Users;
using GlobalDefine;
using MongoDB.Driver;
using Game;
using System.Threading.Tasks;

namespace BattleServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();

            while (true) { }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton<GrpcSetUp>();

                services.AddSingleton<IChat>(new Chat(100));
                services.AddSingleton<ILoginMember, LoginMember>();

                services.AddSingleton<LoginMemberAdapter>();
                services.AddSingleton<ChatAdapter>();

                services.AddSingleton<IUsers, Users>();

                services.AddSingleton<HeartBeatScanContext>();
                services.AddSingleton<HeartBeatScan>();




                //===================================

                services.AddSingleton<ITcpDefine, TcpDefine>();
                services.AddSingleton<ITcpServer, TcpServer>();
                //services.AddSingleton<IProtocolContainer, ProtocolContainer>();
                //services.AddSingleton<IProtocolParser, ProtocolParser>();

                services.AddSingleton<IServerDefine, ServerDefine>();
                IMongoClient client = new MongoClient("mongodb+srv://admin:admin123456@clustertangyu-rpvpx.gcp.mongodb.net/test?retryWrites=true&w=majority");
                services.AddSingleton<IMongoClient>(client);
                services.AddHostedService<GameHost>();
                //services.AddHostedService<CommandSystem>();
            });
    }
}
