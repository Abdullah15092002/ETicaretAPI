using ETicaretAPI.API.Configurations.ColumnWriters;
using ETicaretAPI.Application;
using ETicaretAPI.Application.Validators.Products;
using ETicaretAPI.Infrastructure;
using ETicaretAPI.Infrastructure.Filters;
using ETicaretAPI.Persistance;
using ETicaretAPI.SignalR;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.PostgreSQL;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddPersistanceServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddSignalRServices();


builder.Services.AddCors(options => options.AddDefaultPolicy(policy => 
policy.WithOrigins("http://localhost:4200", "https://localhost:4200").AllowAnyHeader().AllowAnyMethod().AllowCredentials() ));


Logger log = new LoggerConfiguration()
    .WriteTo.Console()//console logla 
    .WriteTo.File("logs/log.txt")// dosyaya logla
    .WriteTo.PostgreSQL(builder.Configuration.GetConnectionString("PostgreSQL"), "logs", needAutoCreateTable: true
    , columnOptions: new Dictionary<string, ColumnWriterBase>//veritaban?na logla
    {
        {"message",new RenderedMessageColumnWriter() },
        {"message_template", new MessageTemplateColumnWriter() },
        {"level",new LevelColumnWriter() },
        {"time_stamp", new TimestampColumnWriter() },          //veritaban?ndali kolonlar?n tan?mlanmas?
        {"exception",new ExceptionColumnWriter() },
        {"log_event", new LogEventSerializedColumnWriter() },
         {"user_name", new UsernameColumnWriter() },
    })
    .WriteTo.Seq(builder.Configuration["Seq:ServerURL"])         // seq url si appsettings.json da
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    
    .CreateLogger();
builder.Host.UseSerilog(log);
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateProductValidator>();
builder.Services.AddControllers(options => options.Filters.Add<ValidationFilter>())
    .ConfigureApiBehaviorOptions(options => options.SuppressModelStateInvalidFilter = true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Admin", options => {
        options.TokenValidationParameters = new()
        {
            ValidateAudience = true,//Olu?turulacak Token de?erini hangi sitelerin orjinlerin kullanaca??n? belirledi?imiz de?erdir. =>www.bilmemne.com
            ValidateIssuer = true,  //  Olu?turulacak Token de?erini kimin da??tt???n? ifade edece?imiz aland?r =>www.myapi.com
            ValidateLifetime = true,//Olu?turulan token de?erinin süresini kontrol edecek olan do?rulamad?r
            ValidateIssuerSigningKey = true,//Üretilecek token de?erinin uygulamam?za ait bir de?er oldu?unu
                                            //ifade eden security key verisinin do?rulanmas?d?r
            ValidAudience = builder.Configuration["Token:Audience"],
            ValidIssuer = builder.Configuration["Token:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Token:SecurityKey"])),
            LifetimeValidator = (notBefore, expires, securityToken, validationParameters) => expires != null ? expires > DateTime.UtcNow : false,
            NameClaimType = ClaimTypes.Name
    
    };
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseSerilogRequestLogging();
app.UseCors();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();


app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next();
});
app.MapControllers();
app.MapHubs();
app.Run();
