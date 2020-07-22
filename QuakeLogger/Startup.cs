using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuakeLogger.AutoMapper;
using QuakeLogger.Data.Context;
using QuakeLogger.Data.Repositories;
using QuakeLogger.Domain.Interfaces.Repositories;
using QuakeLogger.Services;

namespace QuakeLogger
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().
                AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            services.AddScoped<IQuakeGameRepo, QuakeGameRepo>(); // injeção de dependência
            services.AddScoped<IQuakePlayerRepo, QuakePlayerRepo>(); // injeção de dependência
            services.AddScoped<IQuakeKillMethodRepo, QuakeKillMethodRepo>(); // injeção de dependência

            services.AddScoped<Parser>(); // Faz a leitura do arquivo .txt
            services.AddScoped<ReportPrinter>(); // Imprime o relatório de ranking geral


            

            services.AddAutoMapper(typeof(AutoMapperConfig)); // Adicionar o serviço do AutoMapper


            services.AddControllersWithViews();

            services.AddDbContext<QuakeLoggerContext>(options =>
                                                      options
                                                      .UseInMemoryDatabase("InMemoryProvider")); // configuração do DB em memória


            services.AddSwaggerGen(x =>
            x.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "QuakeLogger", Version = "v1" })
            );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, Parser parser, ReportPrinter reportPrinter)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                parser.Reader(@"C:\Users\andre\source\repos\QuakeLoggerAPI\raw.txt");
                reportPrinter.Print();
            }

            else
            {
                app.UseHsts();
            }



            app.UseSwagger();

            app.UseSwaggerUI(option =>
            {
                option.SwaggerEndpoint("/swagger/v1/swagger.json", "QuakeLogger V1");
            });


            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();



            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
