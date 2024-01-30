using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApi.Models;
using WebApplicationTraining.Models;

namespace WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // db population
            using (var context = new DataContext()) // call to .Dispose()
            {
                SeedData.SeedDatabase(context);
            }

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<DataContext>(
                opts => opts.UseSqlServer("Data Source=localhost;Initial Catalog=FinalEs;Persist Security Info=True;User ID=sa;Password=Uform@2023#;Encrypt=False")
                );

            //activate swagger gen
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "WebAPI",
                    Version = "v1"
                });
            }
            );

            // Add cors
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("ReactEnd", builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowAnyOrigin();
                });
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // JWT auth
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true, // chi ha rilasciato il token
                        ValidateAudience = true, // portatore del token
                        ValidateLifetime = true, // scadenza del token
                        ValidateIssuerSigningKey = true, // validare chiave che ha firmato il token
                        ValidIssuer = builder.Configuration["Jwt:Issuer"], // chi è l'issuer?
                        ValidAudience = builder.Configuration["Jwt:Audience"], // chi è l'audience
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) //chiave per firmare il token
                    };
                });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseCors("ReactEnd");

            app.UseAuthorization();

            app.UseSwagger();
            app.UseSwaggerUI(opt =>
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI")
            );

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
