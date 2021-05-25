using MeterReadingImport.Repository.DbContexts;
using MeterReadingImport.Service.Interfaces.MeterReadingImport;
using MeterReadingImport.Service.MeterReadingImport;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MeterReadingImport
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MeterReadingImportDbContext>(options =>
                            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly("MeterReadingImport.Repository")));
            //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            //,x => x.MigrationsAssembly("MeterReadingImport.Repository")));

            services.AddTransient<IUploadService, UploadService>();

            services.AddControllers();

            services.AddAuthorization();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Meter Reading Upload Test API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();

                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meter Reading Upload Test API");
                    c.RoutePrefix = string.Empty;
                });

                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
