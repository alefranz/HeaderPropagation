using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddHeaderPropagation(o =>
            {
                // propagate the header if present
                o.Headers.Add("User-Agent");

                // if still missing, set it with a value factory
                o.Headers.Add("User-Agent", context => "Mozilla/5.0 (trust me, I'm really Mozilla!)");

                // propagate the header if present, using a different name in the outbound request
                o.Headers.Add("Accept-Language", "Lang");
            });

            services.AddHttpClient<GitHubClient>(c =>
            {
                c.BaseAddress = new Uri("https://api.github.com/");
                c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            }).AddHeaderPropagation();  // propagate all the headers

            services.AddHttpClient("example", c =>
            {
                c.BaseAddress = new Uri("https://api.github.com/");
                c.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");
            }).AddHeaderPropagation(o =>
            {
                // or propagate only a specific header, also redefining the name to use
                o.Headers.Add("User-Agent", "Source");
            });

            services.AddHostedService<OneOffBackgroundService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseHeaderPropagation();

            app.UseMvc();
        }
    }
}
