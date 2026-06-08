using DeltaFour.Application.Integrations;
using DeltaFour.Application.Integrations.Storage;
using DeltaFour.Application.Emails;
using DeltaFour.Application.Integrations;
using DeltaFour.Application.Services;
using DeltaFour.Domain.IRepositories;
using DeltaFour.Infrastructure.Context;
using DeltaFour.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DeltaFour.CrossCutting.Ioc;

public static class DependencyInjection
{
    ///<summary>
    ///Configuration for Dependency injection and enviroment
    ///</summary>
    public static IServiceCollection AddInfrastructure
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var isTesting = Environment.GetEnvironmentVariable("IS_TESTING")!;

        if (isTesting.ToLower().Equals("true"))
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        }
        else
        {
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")!;

            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
                    uoptions => uoptions.UseNetTopologySuite()));
        }

        // Registrar repositórios e serviços independente do ambiente
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAddressRepository, AddressRepository>();
        services.AddScoped<IUserAuthRepository, UserAuthRepository>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<UserService>();
        services.AddScoped<WorkShiftService>();
        services.AddScoped<CompanyService>();
        services.AddScoped<DepartmentService>();
        services.AddScoped<RoleService>();
        services.AddScoped<CompanyRegistrationService>();
        services.AddScoped<SubscriptionWebhookService>();
        services.AddScoped<ITimeSheetPdfService, TimeSheetPdfService>();
        services.AddScoped<ISubscriptionService, Application.Integrations.Subscription.StripeSubscriptionService>();
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<ISubscriptionEventRepository, SubscriptionEventRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IActionRepository, ActionRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IWorkShiftRepository, WorkShiftRepository>();
        services.AddScoped<IUserAttendanceRepository, UserAttendanceRepository>();
        services.AddScoped<IUserShiftRepository, UserShiftRepository>();
        services.AddScoped<ITimeSheetRepository, TimeSheetRepository>();
        services.AddScoped<ITimeSheetSignatureRepository, TimeSheetSignatureRepository>();
        services.AddScoped<ITimeSheetSignatureTokenRepository, TimeSheetSignatureTokenRepository>();
        services.AddScoped<ITimeSheetAuditRepository, TimeSheetAuditRepository>();
        services.AddScoped<ICompanyGeolocationRepository, CompanyGeolocationRepository>();
        services.AddScoped<IUserFaceRepository, UserFaceRepository>();
        services.AddScoped<IUserPunctualityMetricRepository, UserPunctualityMetricRepository>();
        services.AddScoped<IClusterCentroidRepository, ClusterCentroidRepository>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUnitOfWork, AllRepositories>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IEmailSender, EmailSender>();
        services.AddScoped<PunctualityMetricsService>();
        services.AddScoped<NotificationService>();

        // Google Cloud Storage (upload de anexos do ponto)
        services.AddScoped<IStorageService, GoogleCloudStorageService>();

        var faceRecocnitionBaseUrl = Environment.GetEnvironmentVariable("FACE_RECOGNITION_BASE_URL");

        services.AddHttpClient<IFaceRecognitionIntegration, FaceRecognitionIntegration>(client =>
        {
            client.BaseAddress = new Uri(faceRecocnitionBaseUrl);
        });

        return services;
    }
}