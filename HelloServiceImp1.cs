using Grpc.Core;

namespace GrpcServicePlayground
{
    public class HelloServiceImpl : HelloService.HelloServiceBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            var reply = new HelloReply { Message = $"Hello, {request.Name}!" };
            return Task.FromResult(reply);
        }
    }
}
