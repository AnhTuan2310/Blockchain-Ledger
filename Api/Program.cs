using Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddInfrastructure(builder.Configuration);

//Controller
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => {
    c.SwaggerDoc("v1", new() { Title = "Blockchain Ledger API", Version = "v1" });
});//Setup doc cho Swagger
var app = builder.Build();

//Set lowercase cho URL
builder.Services.AddRouting(options => {
    options.LowercaseUrls = true;
});
//Middleware xu ly exception
app.UseMiddleware<Api.Middlewares.ExceptionMiddleware>(); 
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();




//Swagger
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}