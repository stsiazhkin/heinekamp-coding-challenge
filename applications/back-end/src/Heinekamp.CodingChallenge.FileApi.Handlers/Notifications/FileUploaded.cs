using MediatR;
using Microsoft.AspNetCore.Http;

namespace Heinekamp.CodingChallenge.FileApi.Handlers.Notifications;

public record FileUploaded(IFormFile File, Guid FileId, string CurrentUser) : INotification;