using Application.Consts;
using Grpc;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;

namespace Grpc.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        [Authorize(Roles = UserRoles.Member)]
        public override async Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return new HelloReply
            {
                Message = "Hello " + request.Name
            };
        }
    }
}
