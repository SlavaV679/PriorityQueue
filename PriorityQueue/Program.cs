
namespace PriorityQueue
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            //builder.Services.AddSingleton(typeof(IPriorityQueue), typeof(PriorityQueue));
            //builder.Services.AddSingleton(typeof(Handler), typeof(Handler));

            builder.Services.AddSingleton(typeof(IInMemoryPriorityQueue<>), typeof(InMemoryPriorityQueue<>));
            builder.Services.AddScoped<IHandler, Handler>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
