using MediatR;

namespace CQRS.WebApi;

public class DeleteProductByIdCommand : IRequest<int>
{
    public int Id { get; set; }
    public class DeleteProductByIdCommandHandler : IRequestHandler<DeleteProductByIdCommand, int>
    {
        private readonly IApplicationContext _context;

        public DeleteProductByIdCommandHandler(IApplicationContext context)
        {
            _context = context;
        }
        public Task<int> Handle(DeleteProductByIdCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }

}
