using Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructure(builder.Configuration);

//Controller
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "Blockchain Ledger API", Version = "v1" });
});//Setup doc cho Swagger

//Set lowercase cho URL
builder.Services.AddRouting(options => {
    options.LowercaseUrls = true;
});
//CORS
builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontend", policy => {
        policy.WithOrigins("http://localhost:5173") // FE port
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

//Swagger
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowFrontend");
//Middleware xu ly exception
app.UseMiddleware<Api.Middlewares.ExceptionMiddleware>(); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();

