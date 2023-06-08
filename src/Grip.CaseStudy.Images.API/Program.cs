using Grip.CaseStudy.Images.Persistence;
using Grip.CaseStudy.Images.Persistence.Configuration;
using Grip.CaseStudy.Images.Persistence.Interfaces;
using Grip.CaseStudy.Images.Services;
using Grip.CaseStudy.Images.Services.Interfaces;
using System.Reflection;

namespace Grip.CaseStudy.Images.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services
                .AddSingleton(new StorageConfiguration(builder.Configuration.GetConnectionString("Storage")))
                .AddSingleton<IImageRepository, ImageRepository>()
                .AddSingleton<IImageScalingService, ImageScalingService>()
                .AddSingleton<IImageIngestionService, ImageIngestionService>()
                .AddSingleton<IImageEgressService, ImageEgressService>();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options => {
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // TODO: disable on production
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            // TODO: setup proper OAauth2-based authorization.
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}