using FeedbackApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using System.Text.Json;
using FeedbackApp.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace FeedbackApp
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			builder.Services.AddRouting();

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
			.AddCookie(options =>
            {
                options.LoginPath = "/";
                options.LogoutPath = "/";
            })
			.AddGitHub(options =>
            {
                options.ClientId = builder.Configuration["GitHub:ClientId"] ?? string.Empty;
                options.ClientSecret = builder.Configuration["GitHub:ClientSecret"] ?? string.Empty;
                options.EnterpriseDomain = builder.Configuration["Github:EnterpriseDomain"] ?? string.Empty;
                options.Scope.Add("user:email");

				options.Events = new OAuthEvents
				{
					OnCreatingTicket = async context =>
					{
						var request = new HttpRequestMessage(HttpMethod.Get, context.Options.UserInformationEndpoint);
						request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
						request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

						var response = await context.Backchannel.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, context.HttpContext.RequestAborted);
						response.EnsureSuccessStatusCode();

						var user = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
						var githubId = user.GetProperty("id").GetInt32();
						var userName = user.GetProperty("login").GetString();

						var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
						var appUser = dbContext.Users.SingleOrDefault(u => u.GitHubId == githubId);

						if (appUser == null)
						{
							appUser = new UserModel
							{
								GitHubId = githubId,
								UserName = userName,
								isAdmin = false,
								isBlocked = false,
							};
							dbContext.Users.Add(appUser);
						}
						else
						{
							appUser.UserName = userName;
						}

						await dbContext.SaveChangesAsync();

						var claims = new List<Claim>
						{
							new Claim(ClaimTypes.NameIdentifier, appUser.Id.ToString()),
							new Claim(ClaimTypes.Name, appUser.UserName),
							new Claim("isAdmin", appUser.isAdmin.ToString()),
							new Claim("isBlocked", appUser.isBlocked.ToString())
						};

						var identity = new ClaimsIdentity(claims, context.Scheme.Name);
						context.Principal = new ClaimsPrincipal(identity);
                    }
				};
            });

			builder.Services.AddDbContext<ApplicationDbContext>
                (options => options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

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

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}