using FluentValidation;
using FluentValidation.AspNetCore;
using Heinekamp.CodingChallenge.FileApi.Authentication;
using Heinekamp.CodingChallenge.FileApi.Common.Extensions;
using Heinekamp.CodingChallenge.FileApi.Extensions;
using Heinekamp.CodingChallenge.FileApi.Handlers;
using Heinekamp.CodingChallenge.FileApi.Persistence.Extensions;
using Heinekamp.CodingChallenge.FileApi.Services.Extensions;
using Heinekamp.CodingChallenge.FileApi.Validation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(UploadFileRequestValidator).Assembly);

builder.Services.AddFileInformationServices(builder.Configuration);

builder.Services.AddImageThumbnailServices(builder.Configuration);
builder.Services.AddArchiveServices();

builder.Services.AddS3Services(builder.Configuration);

builder.Services.AddRetryPolicies(builder.Configuration);

builder.Services.AddApiKeyAuthentication(builder.Configuration);

builder.Services.AddApiExplorerServices();

builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(GetFileHandler).Assembly);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", cpb =>
    {
        cpb.AllowAnyHeader()
            .AllowAnyOrigin()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

app.UseCors("default");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//disabled to run in Docker without additional setup 
//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

//make visible for Integration Tests setup
public partial class Program { }
