using AutoMapper;
using DAM.Application.Cache;
using DAM.Infrastructure.Caching;
using DAM.Persistence;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;

using Microsoft.AspNetCore.Http;
using DAM.Application.Assets.Handlers;
using DAM.Application.Common.Assemblers;
using DAM.Application.Services.Interfaces;
using DAM.Application.Services;
using Microsoft.AspNetCore.Identity;
using DAM.Application.Users.Models;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DAM.Domain.Entities.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using System;
using Microsoft.AspNetCore.Http.Features;

namespace DAM.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static IConfiguration StaticConfig { get; private set; }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddResponseCompression(options =>
			{
				options.Providers.Add<BrotliCompressionProvider>();
				options.Providers.Add<GzipCompressionProvider>();
				options.EnableForHttps = true;
			});
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddCors(o =>
            {
                o.AddPolicy("MyPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });

            });

            services.AddMvc().AddNewtonsoftJson();

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration["redisAddress"];
            });

            services.AddTransient<ICacheProvider, CacheAsideProvider>();
            services.AddTransient<ICacheKeyGenerator, TypePrefixerCacheKeyGenerator>();
            services.AddScoped<IAzureStorageService, AzureStorageService>();
            services.AddScoped<IConversionService, ConversionService>();
            services.AddScoped<IHelperService, HelperService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<IFeatureFlagService, FeatureFlagService>();

            services.AddMediatR(typeof(GetAssetRequestHandler).Assembly);

            services.AddDbContext<DAMDBContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DAMDBConnectionString"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                }));

            var DBoptions = new DbContextOptionsBuilder<DAMDBContext>().UseSqlServer(Configuration.GetConnectionString("DAMDBConnectionString"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure();
                });

            services.AddScoped<IDbContext, DAMDBContext>(s => new DAMDBContext(DBoptions.Options));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                //options.Password.RequiredUniqueChars = 3;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/ ";
                options.SignIn.RequireConfirmedEmail = true;
            }).AddEntityFrameworkStores<DAMDBContext>()
              .AddDefaultTokenProviders();
            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(1));
            services.Configure<FormOptions>(options =>
            {
                options.ValueLengthLimit = int.MaxValue;
                options.ValueCountLimit = int.MaxValue;
            });

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddHttpContextAccessor();

            // Get the database context and apply the migrations
            // To be moved to Release Pipeline
            var context = services.BuildServiceProvider().GetService<IDbContext>();
            context.Database.Migrate();

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            services.AddMemoryCache();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //.AddCookie(cfg => cfg.SlidingExpiration = true)
                .AddJwtBearer(cfg =>
                {
                    cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidIssuer = JwtConfigConstants.Issuer,
                        ValidAudience = JwtConfigConstants.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                        ValidateLifetime = true,
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfigConstants.Key))
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseResponseCompression();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                //app.UseHsts();
                app.UseDeveloperExceptionPage();//for QA_env show the error message
            }
            app.UseRouting();
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseSpaStaticFiles();
  
            app.UseAuthentication();
                
            app.UseAuthorization();

            app.UseCors("MyPolicy");

            app.UseEndpoints(routes =>
            {
                routes.MapControllers().RequireCors("MyPolicy");
                routes.MapControllerRoute(
                    name: "default", "{controller=Content}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

        }
    }
}