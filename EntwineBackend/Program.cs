using EntwineBackend;
using EntwineBackend.Authentication;
using EntwineBackend.DbContext;
using EntwineBackend.PubSub;
using EntwineBackend.Services;
using EntwineBackend.UserId;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        DefaultJsonOptions.InitOptions(options.JsonSerializerOptions);
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IAuthorizationHandler, UserInChatRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, UserInChatCommunityRequirementHandler>();
builder.Services.AddSingleton<TokenBlacklist>();
builder.Services.AddHttpClient();
builder.Services.AddSignalR();
builder.Services.AddDbContext<EntwineDbContext>(options => {
    options.UseNpgsql(builder.Configuration.GetValue<string>("ConnectionStrings:DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<EntwineDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:5173").AllowAnyMethod().AllowAnyHeader().AllowCredentials();
    });
});

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetValue<string>("Jwt:Issuer"),
            ValidAudience = builder.Configuration.GetValue<string>("Jwt:Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetValue<string>("Jwt:Key")!))
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) &&
                    (path.StartsWithSegments("/chatHub") || path.StartsWithSegments("/communityChatHub")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserId", policy =>
    {
        policy.Requirements.Add(new UserIdRequirement());
    });
    options.AddPolicy("UserInChat", policy =>
    {
        policy.Requirements.Add(new UserInChatRequirement());
    });
    options.AddPolicy("UserInChatCommunity", policy =>
    {
        policy.Requirements.Add(new UserInChatCommunityRequirement());
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<TokenBlacklistMiddleware>();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
app.MapHub<CommunityChatHub>("/communityChatHub");

app.Run();

// Just so it's visible to tests
public partial class Program
{
}