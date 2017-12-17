using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VirtualStudentAdviser.Services;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;
using VirtualStudentAdviser.Filters;

namespace VirtualStudentAdviser
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services. 
            //services.AddDbContext<VirtualAdviserContext>();
            services
                .AddDbContext<VirtualAdviserContext>(options => options.UseSqlServer(Configuration.GetConnectionString("VirtualAdvisorDatabase")));
         
            services.AddCors(o => o.AddPolicy("VsaCorsPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .WithExposedHeaders()
                       ;
                       
            }));
            services.AddMvc()
                  .AddJsonOptions(options =>
                  {
                      options.SerializerSettings.Formatting = Formatting.Indented;
                  })
                 // .AddMvcOptions(opt =>
                 //opt.Filters.Add(new VsaFilter()))
               ;
            services.AddScoped<IVirtualAdviserRepository, VirtualAdviserRepository>();

            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "VAA API", Version = "v1" });

                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "VirtualStudentAdviser.xml");
                c.IncludeXmlComments(xmlPath);
            });


            //services.Configure<Microsoft.AspNetCore.Mvc.MvcOptions>(options =>
            //{

            //});

            //services.AddMvc();
            //services.ConfigureMvc(options =>
            //{
            //    options.Filters.Add(new YouGlobalActionFilter());
            //}

        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();
            app.UseCors("VsaCorsPolicy");


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });


            app.UseMvc();


            


            // Shows UseCors with CorsPolicyBuilder.
            //app.UseCors(builder =>
            //   builder.WithOrigins("*").AllowAnyHeader());


        }


    }
}
