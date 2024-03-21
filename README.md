# CQRS.WebApi


1. create a new asp.net core project with webApi template

    ```
    dotnet new webapi -f net6.0 -n CQRS.WebApi -o CQRS.WebApi
    ```
2. Go to the project folder directory and open the project in vsCode
    ```
    cd CQRS.WebApi
    code .
    ```
3. Add gitIgnore file by the vsCode shortcut for mac (you have to have gitignore installed for getting this template)
    ```
    cmd + shipt + p
    add gitignore
    ```
4. Go to directory and add readme.md file and overwrite the file with standard template from online
    ```
    touch README.md
    ```
5. Initialize the project into github as initial commit
6. Install the following packages to the API project 
    ```
    dotnet add package Microsoft.EntityFrameworkCore
    dotnet add package Microsoft.EntityFrameworkCore.Relational
    dotnet add package Microsoft.EntityFrameworkCore.SqlServer
    dotnet add package MediatR
    dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
    dotnet add package Swashbuckle.AspNetCore
    dotnet add package Swashbuckle.AspNetCore.Swagger
    dotnet add package Microsoft.EntityFramework.Design
    ```
7. Create a new folder called "Models" and add new class called "Product"
    ```
    dotnet new class -n Product.cs
    ```
8. Add a new folder called "Context" and add new class called "ApplicationContext" and configure this up for product entity
    ```
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) 
        { 

        }
        public DbSet<Product> Products { get; set; }
        public async Task<int> SaveChanges()
        {
            return await base.SaveChangesAsync();
        }
    }
    ```
9. Add interface for ApplicationContext class named "IApplicationContext" and set up properties for ApplicationContext class
    ```
    dotnet new interface -n IApplicationContext
    ```

    ```
    public interface IApplicationContext
    {
    DbSet<Product> Products { get; set; }
    Task<int> SaveChanges();
    }
    ```
10. Register ApplicationContext into program
    ```
    builder.Services.AddDbContext<ApplicationContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationContext).Assembly.FullName)));
    ```
11. Define a connectionString in the appsettings.json
    ```
    "ConnectionStrings": 
    {
        "DefaultConnection": "Server=localhost;Database=developmentDb;User=sa;Password=Docker@123;"
    }
    ```
12. add migration
    ```
    dotnet ef migrations add initial_migration
    ```
13. update database
    ```
    dotnet ef database update
    ```
14. register mediatR into the program file
    ```
    builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
    ```
15. Go to the Controllers folder and add new controller named "ProductController"
16. Create a folder named "Features", create another folder called "ProductFeatures" and create subfolders "Commands" and "Queries"
17. Create a class named "GetAllProductsQuery" into Queries folder
18. Now implement handler for GetAllProductsQuery
    ```
    public class GetAllProductsQuery : IRequest<IEnumerable<Product>>
    {
        public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery,IEnumerable<Product>>
        {
            private readonly IApplicationContext _context;
            public GetAllProductsQueryHandler(IApplicationContext context)
            {
                _context = context;
            }
            public async Task<IEnumerable<Product>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
            {
                var productList = await _context.Products.ToListAsync();
                if (productList == null)
                {
                    return null;
                }
                return productList.AsReadOnly();
            }
        }
    }
    ```
19. Create a class named "GetProductByIdQuery" into queries folder
20. Now implement handler for GetProductByIdQuery
    ```
    public class GetProductByIdQuery : IRequest<Product>
    {
        public int Id { get; set; }
        public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Product>
        {
            private readonly IApplicationContext _context;
            public GetProductByIdQueryHandler(IApplicationContext context)
            {
                _context = context;
            }
            public async Task<Product> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
            {
                var product =  _context.Products.Where(a => a.Id == query.Id).FirstOrDefault();
                if (product == null) return null;
                return product;
            }
        }
    }
    ```
21. Go to commands and create a class named "CreateProductCommand" 
    ```
    public class CreateProductCommand : IRequest<int>
    {
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal Rate { get; set; }
        public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
        {
            private readonly IApplicationContext _context;
            public CreateProductCommandHandler(IApplicationContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(CreateProductCommand command, CancellationToken cancellationToken)
            {
                var product = new Product();
                product.Barcode = command.Barcode;
                product.Name = command.Name;
                product.BuyingPrice = command.BuyingPrice;
                product.Rate = command.Rate;
                product.Description = command.Description;
                _context.Products.Add(product);
                await _context.SaveChanges();
                return product.Id;
            }
        }
    }
    ```
22. Go to commands folder and create another class "DeleteProductByIdCommand"
    ```
    public class DeleteProductByIdCommand : IRequest<int>
    {
        public int Id { get; set; }
        public class DeleteProductByIdCommandHandler : IRequestHandler<DeleteProductByIdCommand, int>
        {
            private readonly IApplicationContext _context;
            public DeleteProductByIdCommandHandler(IApplicationContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(DeleteProductByIdCommand command, CancellationToken cancellationToken)
            {
                var product = await _context.Products.Where(a => a.Id == command.Id).FirstOrDefaultAsync();
                if (product == null) return default;
                _context.Products.Remove(product);
                await _context.SaveChanges();
                return product.Id;
            }
        }
    }
    ```
23. Go to commands folder and create another class "UpdateProductCommand"
    ```
    public class UpdateProductCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal BuyingPrice { get; set; }
        public decimal Rate { get; set; }
        public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, int>
        {
            private readonly IApplicationContext _context;
            public UpdateProductCommandHandler(IApplicationContext context)
            {
                _context = context;
            }
            public async Task<int> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
            {
                var product = _context.Products.Where(a => a.Id == command.Id).FirstOrDefault();

                if (product == null)
                {
                    return default;
                }
                else
                {
                    product.Barcode = command.Barcode;
                    product.Name = command.Name;
                    product.BuyingPrice = command.BuyingPrice;
                    product.Rate = command.Rate;
                    product.Description = command.Description;
                    await _context.SaveChanges();
                    return product.Id;
                }
            }
        }
    }
    ```
24. Now update the "productController" so that it calls out the handlers 
    ```
    public class ProductController : ControllerBase
    {
        private IMediator _mediator;

        protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductCommand command)
        {
            return Ok(await Mediator.Send(command));
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await Mediator.Send(new GetAllProductsQuery()));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(await Mediator.Send(new GetProductByIdQuery { Id = id }));
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return Ok(await Mediator.Send(new DeleteProductByIdCommand { Id = id }));
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateProductCommand command)
        {
            if (id != command.Id)
            {
                return BadRequest();
            }
            return Ok(await Mediator.Send(command));
        }
    }
    ```
25. Configure swagger in the program file
    ```
    #region Swagger
            services.AddSwaggerGen(c =>
            {
                //c.IncludeXmlComments(string.Format(@"{0}\CQRS.WebApi.xml", System.AppDomain.CurrentDomain.BaseDirectory));
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "CQRS.WebApi",
                });

            });
    #endregion

    #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CQRS.WebApi");
            });
    #endregion
    ```
26. Inject ApplicationContext into services in program file
    ```
    builder.Services.AddScoped<IApplicationContext, ApplicationContext>();
    ```

---

#### Reference: https://codewithmukesh.com/blog/cqrs-in-aspnet-core-3-1/