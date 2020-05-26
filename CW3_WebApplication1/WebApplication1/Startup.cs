using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using WebApplication1.DAL;
using WebApplication1.Middleware;
using WebApplication1.Services;

namespace WebApplication1
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
    	// HTTP Basic
			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(
				    options => {
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer=true,
						ValidateAudience= true,
						ValidateLifetime=true,
						ValidIssuer="Gakko",
						ValidAudience = "Students",
						IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["SecretKey"]))
					};
				});
		// services.AddAuthentication("AuthenticationBasic")
		// 		.AddScheme<AuthenticationSchemeOptions, BasicAuthHandler>("AuthenticationBasic", null);
				//.AddXmlSerializerFormatters();

            services.AddTransient<IStudentDbService, SqlServerStudentDbService>();
            services.AddSingleton<IDbService, MockDbService>(); 
            services.AddControllers();

        // 1. Dodawanie dokumentacji
        services.AddSwaggerGen(config =>
            {
                config.SwaggerDoc("v1", new OpenApiInfo { Title = "Students App API [s16446]", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

        //Middleware
            app.UseSwagger();
            app.UseSwaggerUI(
                config =>
                {
                    config.SwaggerEndpoint("/swagger/v1/swagger.json", "Students App API [s16446]");
                }
            );

            //app.Use(async (context, next) =>
            //    {
            //        if (!context.Request.Headers.ContainsKey("Index"))
            //    {
            //    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //    await context.Response.WriteAsync("ERROR [Middleware]: Musisz podac numer indeksu");
            //    return;
            //}
            //await next();
            //});

            //app.UseMiddleware<LoggingMiddleware>();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
			app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
