using appify.Business;
using appify.Business.Contract;
using appify.DataAccess;
using appify.DataAccess.Contract;
using Asp.Versioning;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using appify.DataAccessML;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(
options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}
).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:key"])),
        //ClockSkew = TimeSpan.Zero
    };
});

//Data Access services
builder.Services.AddSingleton<IAddressRepository, AddressRepository>(); //
builder.Services.AddSingleton<ICustomerRepository, CustomerRepository>();//
builder.Services.AddSingleton<ICommonServicesRepository, CommonServicesRepository>();
builder.Services.AddSingleton<IDiscountDetailRepository, DiscountDetailRepository>();//
builder.Services.AddSingleton<IDiscountHeaderRepository, DiscountHeaderRepository>();//
builder.Services.AddSingleton<IEventLogRepository, EventLogRepository>();//
builder.Services.AddSingleton<IInvoiceDetailRepository, InvoiceDetailRepository>();//
builder.Services.AddSingleton<IInvoiceHeaderRepository, InvoiceHeaderRepository>();//
builder.Services.AddSingleton<ILookUpRepository, LookUpRepository>();//
builder.Services.AddSingleton<IMemberAppSettingRepository, MemberAppSettingRepository>();//
builder.Services.AddSingleton<IMemberContactRepository, MemberContactRepository>();//
builder.Services.AddSingleton<IMemberKYCRepository, MemberKYCRepository>();//
builder.Services.AddSingleton<IMemberRepository, MemberRepository>();//
builder.Services.AddSingleton<IMemberReturnPolicyRepository, MemberReturnPolicyRepository>();//
builder.Services.AddSingleton<IMemberThemeRepository, MemberThemeRepository>();//
builder.Services.AddSingleton<INotificationRepository, NotificationRepository>();//
builder.Services.AddSingleton<IOrderDetailRepository, OrderDetailRepository>();//
builder.Services.AddSingleton<IOrderHeaderRepository, OrderHeaderRepository>();//
builder.Services.AddSingleton<IProductImageRepository, ProductImageRepository>();//
builder.Services.AddSingleton<IProductPriceRepository, ProductPriceRepository>();//
builder.Services.AddSingleton<IProductRepository, ProductRepository>();//
builder.Services.AddSingleton<IThemeMasterRepository, ThemeMasterRepository>();//
builder.Services.AddSingleton<IVendorPaymentRepository, VendorPaymentRepository>();//
builder.Services.AddSingleton<IWebAdminRepository, WebAdminRepository>();
builder.Services.AddSingleton<IRolesRepository, RolesRepository>();
builder.Services.AddSingleton<ISecurablesRepository, SecurablesRepository>();
builder.Services.AddSingleton<IAdminDashboardRepository, AdminDashboardRepository>();
builder.Services.AddSingleton<IRoleRightsRepository, RoleRightsRepository>();
builder.Services.AddSingleton<ICategoryParameterRepository, CategoryParameterRepository>();
builder.Services.AddSingleton<IParameterTypeRepository, ParameterTypeRepository>();
//builder.Services.AddSingleton<IOrderHeaderRepository, OrderHeaderRepository>();
//builder.Services.AddSingleton<IOrderDetailRepository, OrderDetailRepository>();

//Business services
builder.Services.AddSingleton<IAddressBusiness, AddressBusiness>();//
builder.Services.AddSingleton<ICustomerBusiness, CustomerBusiness>();//
builder.Services.AddSingleton<ICommonServicesBusiness, CommonServicesBusiness>();
builder.Services.AddSingleton<IDiscountDetailBusiness, DiscountDetailBusiness>();//
builder.Services.AddSingleton<IDiscountHeaderBusiness, DiscountHeaderBusiness>();//
builder.Services.AddSingleton<IEventLogBusiness, EventLogBusiness>();//
builder.Services.AddSingleton<IInvoiceBusinesss, InvoiceBusiness>();//
builder.Services.AddSingleton<ILookupBusiness, LookupBusiness>();//
builder.Services.AddSingleton<IMemberAppSettingBusiness, MemberAppSettingBusiness>();//
builder.Services.AddSingleton<IMemberBusiness, MemberBusiness>();//
builder.Services.AddSingleton<IMemberContactBusiness, MemberContactBusiness>();//
builder.Services.AddSingleton<IMemberKYCBusiness, MemberKYCBusiness>();//
builder.Services.AddSingleton<IMemberReturnPolicyBusiness, MemberReturnPolicyBusiness>();//
builder.Services.AddSingleton<IMemberThemeBusiness, MemberThemeBusiness>();//
builder.Services.AddSingleton<INotificationBusiness, NotificationBusiness>();//
builder.Services.AddSingleton<IOrderBusiness, OrderBusiness>();//
builder.Services.AddSingleton<IProductBusiness, ProductBusiness>();//
builder.Services.AddSingleton<IProductImageBusiness, ProductImageBusiness>();//
builder.Services.AddSingleton<IProductPriceBusiness, ProductPriceBusiness>();//
builder.Services.AddSingleton<IThemeMasterBusiness, ThemeMasterBusiness>();//
builder.Services.AddSingleton<IVendorPaymentBusiness, VendorPaymentBusiness>();//
builder.Services.AddSingleton<IWebAdminBusiness, WebAdminBusiness>();
builder.Services.AddSingleton<IRolesBusiness, RolesBusiness>();
builder.Services.AddSingleton<ISecurablesBusiness, SecurablesBusiness>();
builder.Services.AddSingleton<IAdminDashboardBusiness, AdminDashboardBusiness>();
builder.Services.AddSingleton<IRoleRightsBusiness, RoleRightsBusiness>();
builder.Services.AddSingleton<ICategoryParameterBusiness, CategoryParameterBusiness>();
builder.Services.AddSingleton<IParameterTypeBusiness, ParameterTypeBusiness>();
//builder.Services.AddSingleton<IOrderBusiness, OrderBusiness>();


//builder.Services.AddSwaggerGen(sw=>
//    sw.AddSecurityDefinition("Bearer",new Microsoft.OpenApi.Models.OpenApiSecurityScheme() { 
//        Name="Authorization",
//        Type= Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
//        Scheme="Bearer",
//        BearerFormat="JWT",
//        In= Microsoft.OpenApi.Models.ParameterLocation.Header,
//        Description ="Use AWT Token in the Header as Bearer scheme",

//    })
//);

builder.Services.AddSwaggerGen(sg=> {
    sg.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Appify API Doc",
        Version = "v1"
    });

    var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    var swaggerdocfilePath = Path.Combine(baseDirectory, "appify.web.api.xml");
    sg.IncludeXmlComments(swaggerdocfilePath);

    sg.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Enter Authentication Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"

    });

    sg.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type =ReferenceType.SecurityScheme,
                    Id= "Bearer"
                }
            },
            new string[]{ }
        }
    });
} );

builder.Services.AddApiVersioning(o =>
{
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.ReportApiVersions = true;
    o.ApiVersionReader = ApiVersionReader.Combine(
        //new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("appify-version"));
        //new MediaTypeApiVersionReader("ver"));
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Appify.web.API v2"));

    
}
app.UseStaticFiles();
app.UseRouting();
app.UseCors(x=> x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

