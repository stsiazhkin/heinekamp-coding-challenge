using MediatR;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Requests;

public interface IAuthorizedRequest<out T> : IRequest<T>
{
    public string CurrentUser { get; }
}