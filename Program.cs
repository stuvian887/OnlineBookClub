using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineBookClub.Models;
using OnlineBookClub.Repository;
using OnlineBookClub.Service;
using OnlineBookClub.Services;
using OnlineBookClub.Token;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddDbContext<OnlineBookClubContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("WebDatabase")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<MembersService>();
builder.Services.AddScoped<MailService>();
builder.Services.AddScoped<MembersRepository>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<BookPlanRepository>();
builder.Services.AddScoped<BookPlanService>();
builder.Services.AddScoped<LearnRepository>();
builder.Services.AddScoped<LearnService>();
builder.Services.AddScoped<PlanMemberRepository>();
builder.Services.AddScoped<PlanMemberService>();
builder.Services.AddScoped<TopicRepository>();
builder.Services.AddScoped<TopicService>();
builder.Services.AddScoped<BookRepository>();
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<PostRepository>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<ReplyRepository>();
builder.Services.AddScoped<ReplyService>();
builder.Services.AddScoped<ReportRepository>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<StatisticRepository>();
builder.Services.AddScoped<StatisticService>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true; // 可選，讓輸出的 JSON 更易讀
});
//builder.Services.AddControllers().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
//    // 或使用 Preserve（會保留引用資訊，但 JSON 格式會比較複雜）
//    // options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
//});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
   .AddJwtBearer(options =>
   {
       options.TokenValidationParameters = new TokenValidationParameters
       {
           ValidateIssuer = true,
           ValidIssuer = builder.Configuration["JWT:Issuer"],
           ValidateAudience = true,
           ValidAudience = builder.Configuration["JWT:Audience"],
           ValidateLifetime = true,
           ValidateIssuerSigningKey = true,
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"]))

       };
       options.Events = new JwtBearerEvents
       {
           OnMessageReceived = context =>
           {
               if (context.Request.Cookies.ContainsKey("JWT"))
               {
                   context.Token = context.Request.Cookies["JWT"];
               }
               return Task.CompletedTask;
           },

       };
   });
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500") // 允許前端的來源網址
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();  // 設定允許攜帶憑證（cookie）
    });
});
builder.Services.AddMvc(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});
var app = builder.Build();
app.UseStaticFiles();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();

app.UseCookiePolicy();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
