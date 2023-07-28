using AutoMapper;
using CmsLocalization.DB;
using CmsLocalization.Infastructure;
using CmsLocalization.Models;
using CmsLocalization.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CmsLocalization
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            #region Dependency Injections

            services.AddScoped<ILanguageRepository, LanguageRepository>();
            services.AddScoped<IContentRepository, ContentRepository>();
            services.AddScoped<IContentMappingRepository, ContentMappingRepository>();

            #endregion Dependency Injections

            #region AutoMapperConfiguration

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                #region Content

                cfg.CreateMap<ContentModel, Content>()
                .ForMember(dest => dest.CreatedBy, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedTime, mo => mo.Ignore());
                cfg.CreateMap<Content, ContentModel>()
                .ForMember(dest => dest.Languages, mo => mo.Ignore());

                #endregion Content

                #region ContentMapping

                cfg.CreateMap<ContentMappingModel, ContentMapping>()
                .ForMember(dest => dest.Content, mo => mo.Ignore())
                .ForMember(dest => dest.Language, mo => mo.Ignore());
                cfg.CreateMap<ContentMapping, ContentMappingModel>();

                #endregion ContentMapping

                #region Language

                cfg.CreateMap<Language, LanguageModel>();
                cfg.CreateMap<LanguageModel, Language>()
                .ForMember(dest => dest.ContentMappings, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedBy, mo => mo.Ignore())
                .ForMember(dest => dest.CreatedTime, mo => mo.Ignore());

                #endregion Language
            });

            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);

            #endregion AutoMapperConfiguration

            services.AddDbContext<CMS_Context>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddMvc(options => options.EnableEndpointRouting = false);
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
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}