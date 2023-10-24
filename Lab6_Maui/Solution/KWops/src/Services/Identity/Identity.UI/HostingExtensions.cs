using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Identity.UI;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Identity.UI;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddRazorPages();

        var isBuilder = builder.Services.AddIdentityServer(options =>
            {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // see https://docs.duendesoftware.com/identityserver/v6/fundamentals/resources/
                options.EmitStaticAudienceClaim = true;
            })
            .AddTestUsers(TestUsers.Users);

        // in-memory, code config
        isBuilder.AddInMemoryIdentityResources(Config.IdentityResources);
        isBuilder.AddInMemoryApiScopes(Config.ApiScopes);
        var clientSection = builder.Configuration.GetSection("IdentityServer:Clients");
        var clients = new List<Client>();
        clientSection.Bind(clients);
        isBuilder.AddInMemoryClients(clients);
        isBuilder.AddInMemoryApiResources(Config.ApiResources);


        // if you want to use server-side sessions: https://blog.duendesoftware.com/posts/20220406_session_management/
        // then enable it
        //isBuilder.AddServerSideSessions();
        //
        // and put some authorization on the admin/management pages
        //builder.Services.AddAuthorization(options =>
        //       options.AddPolicy("admin",
        //           policy => policy.RequireClaim("sub", "1"))
        //   );
        //builder.Services.Configure<RazorPagesOptions>(options =>
        //    options.Conventions.AuthorizeFolder("/ServerSideSessions", "admin"));


        builder.Services.AddAuthentication();
        //.AddGoogle(options =>
        //{
        //    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

        //    // register your IdentityServer with Google at https://console.developers.google.com
        //    // enable the Google+ API
        //    // set the redirect URI to https://localhost:5001/signin-google
        //    options.ClientId = "copy client ID from Google here";
        //    options.ClientSecret = "copy client secret from Google here";
        //});

        return builder.Build();
    }
    
    public static WebApplication ConfigurePipeline(this WebApplication app)
    { 
        app.UseSerilogRequestLogging();
    
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();
        
        app.MapRazorPages()
            .RequireAuthorization();

        app.UseStaticFiles();

        if (app.Environment.IsDevelopment())
        {
            // Make identity server redirections over http possible in Edge and latest versions of browsers.
            // WARNING: Not valid in a production environment.
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("Content-Security-Policy", "script-src 'unsafe-inline'");
                await next();
            });

            // Fix a problem with chrome. Chrome enabled a new feature "Cookies without SameSite must be secure",
            // the cookies should be expired from https, but in KWops, the internal communication in docker compose is http.
            // To avoid this problem, the policy of cookies should be in Lax mode.
            app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });
        }

        app.UseRouting();

        return app;
    }
}