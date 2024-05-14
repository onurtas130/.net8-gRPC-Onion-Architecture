using Application.Services;
using Domain.Entities.Identity;
using Grpc;
using Grpc.Core;

namespace Grpc.Services
{
    public class UserService : User.UserBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly IAppUserService _appUserService;
        public UserService(ILogger<GreeterService> logger, IAppUserService appUserService)
        {
            _logger = logger;
            _appUserService = appUserService;
        }

        public override async Task<ServiceResultExt> Login(LoginRequest request, ServerCallContext context)
        {
            var serviceResult = await _appUserService.LoginAsync(request.Email, request.Password);

            if(serviceResult is not null)
            {
                var result = new Grpc.ServiceResultExt
                {
                    Explanation = serviceResult.Explanation,
                    TotalCount = serviceResult.TotalCount ?? 0,
                    Status = serviceResult.Status,
                    Expire = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(serviceResult.ResultObject.Expire),
                    Token = serviceResult.ResultObject.Token
                };

                return result;
            }

            throw new Exception("some error occured");

            //return new Grpc.ServiceResultExt
            //{
            //    Explanation = "some error occured",
            //};
        }

        public override async Task<ServiceResult> Register(RegisterRequest request, ServerCallContext context)
        {
            var user = new AppUser()
            {
                UserName = request.UserName,
                Email = request.Email,
            };

            var serviceResult = await _appUserService.RegisterAsync(user, request.Password);

            if(serviceResult is not null && serviceResult.Status)
            {
                return new ServiceResult
                {
                    Explanation = serviceResult.Explanation,
                    Status = serviceResult.Status
                };
            }

            return new ServiceResult { Explanation = "some error occured" };
        }
    }
}
