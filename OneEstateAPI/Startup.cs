using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OneEstateAPI.Middlewares;
using FluentValidation.AspNetCore;
using OneEstate.Application.Services;
using AutoMapper;
using OneEstateAPI.Utils;
using OneEstate.Domain.Services.Mappings;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Resources.Base.SettingsModels;
using OneEstate.Domain.Services;
using Resources.Base.Utils;

namespace OneEstateAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            // for JWT - token authentication 
            var secretForJwt = Configuration.GetSection("AppSettings:Secret").Value;
            var issuerJwt = Configuration.GetSection("AppSettings:Issuer").Value;
            var audienceForJwt = Configuration.GetSection("AppSettings:Audience").Value;
            var jwtKey = Encoding.ASCII.GetBytes(secretForJwt);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuerJwt,
                        ValidAudience = audienceForJwt,
                        IssuerSigningKey = new SymmetricSecurityKey(jwtKey)
                    };
                });

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateModelStateAttribute));
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.ContractResolver =
                    new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
            })
            .AddFluentValidation(fvc =>
                fvc.RegisterValidatorsFromAssemblyContaining<UserAppService>());

            Mapper.Initialize(cfg =>
            {
                AutoMapperRegister.RegisterMappings(cfg);
            });

            services.Configure<MongoConfiguration>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });

            services.AddSingleton<IMongoClient>(sp =>
            {
                var mongoConfiguration = sp.GetService<IOptions<MongoConfiguration>>();
                var client = new MongoClient(mongoConfiguration.Value.ConnectionString);
                return client;
            });

            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var mongoConfiguration = sp.GetService<IOptions<MongoConfiguration>>();
                var mongoClient = sp.GetService<IMongoClient>();
                var database = mongoClient.GetDatabase(mongoConfiguration.Value.Database);
                return database;
            });

            services.Configure<EmailServerSettings>(Configuration.GetSection("EmailSettings"));
            services.Configure<AppGeneralSettings>(Configuration.GetSection("AppSettings"));

            // Application
            services.AddTransient<IUserAppService, UserAppService>();
            services.AddTransient<IProjectAppService, ProjectAppService>();

            // Domain
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IProjectService, ProjectService>();

            // Utils
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "One Estate API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "One Estate API V1");
            });

            app.UseMiddleware(typeof(HttpStatusCodeExceptionMiddleware));

            app.UseAuthentication();

            app.UseCors("CorsPolicy");
            app.UseMvc();

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
