using HmServiceCache.Master.Hubs;
using HmServiceCache.Master.Storage;
using HmServiceCache.Storage.Extensions;
using MessagePack;
using MessagePack.AspNetCoreMvcFormatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HmServiceCache.Master
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
            services.AddSwaggerGen();
            services.AddControllers();  //.AddNewtonsoftJson();
            services.AddMvc().AddMvcOptions(option =>
            {
                option.OutputFormatters.Clear();
                option.OutputFormatters.Add(new MessagePackOutputFormatter(options: MessagePackSerializerOptions.Standard));
                //option.OutputFormatters.Add(new MessagePackOutputFormatter(ContractlessStandardResolver.Instance));
                option.InputFormatters.Clear();
                //option.InputFormatters.Add(new MessagePackInputFormatter(ContractlessStandardResolver.Instance));
                option.InputFormatters.Add(new MessagePackInputFormatter(MessagePackSerializerOptions.Standard));
            });


            services.AddSignalR().AddMessagePackProtocol(options =>
            {
                options.SerializerOptions = MessagePackSerializerOptions.Standard
                    //.WithResolver(new CustomResolver())
                    .WithSecurity(MessagePackSecurity.UntrustedData);
            }); //.AddMessagePackProtocol().AddNewtonsoftJsonProtocol();
            services.AddHttpClient();
            services.AddSingleton<INodeStorage, NodeStorage>();
            services.AddDataStorages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseSwagger();
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<NodeServiceHub>("/nodehub");
                endpoints.MapHub<ClientServiceHub>("/clienthub");
                endpoints.MapControllers();
            });
        }
    }
}
