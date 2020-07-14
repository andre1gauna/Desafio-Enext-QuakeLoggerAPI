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
using QuakeLogger.Data.Seeds;
using QuakeLogger.Domain.Interfaces.Repositories;


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

            services.AddScoped<GameSeed>(); // alimenta o banco com dados de teste
            services.AddScoped<PlayerSeed>(); // alimenta o banco com dados de teste

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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, GameSeed gameSeed, PlayerSeed playerSeed)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                playerSeed.Populate();
                gameSeed.Populate();
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
