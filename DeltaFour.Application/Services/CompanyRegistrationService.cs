using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;
using System.Security.Cryptography;
using System.Text;

namespace DeltaFour.Application.Services;

public class CompanyRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISubscriptionService _subscriptionService;

    public CompanyRegistrationService(IUnitOfWork unitOfWork, ISubscriptionService subscriptionService)
    {
        _unitOfWork = unitOfWork;
        _subscriptionService = subscriptionService;
    }

    public async Task<SubscriptionResult> RegisterCompanyWithSubscription(RegisterCompanyRequest request)
    {
        var company = new Company()
        {
            Name = request.Name,
            Cnpj = request.Cnpj,
            IsActive = true,
            CreatedBy = Guid.Empty,
        };

        var role = new Role()
        {
            Name = "ADMIN",
            CompanyId = company.Id,
            IsActive = true,
            CreatedBy = company.Id,
        };

        using var hash = SHA256.Create();
        byte[] bytes = hash.ComputeHash(Encoding.UTF8.GetBytes(request.User!.Password));
        var hashPassword = new StringBuilder();
        foreach (byte b in bytes)
        {
            hashPassword.Append(b.ToString("x2"));
        }

        var user = new User()
        {
            Name = request.User.Name,
            Email = request.User.Email,
            Password = hashPassword.ToString(),
            CreatedBy = company.Id,
            CompanyId = company.Id,
            RoleId = role.Id,
            IsActive = true,
            IsConfirmed = true,
        };

        _unitOfWork.CompanyRepository.Create(company);
        _unitOfWork.RoleRepository.Create(role);
        _unitOfWork.UserRepository.Create(user);

        await _unitOfWork.Save();

        var subscriptionRequest = new CreateSubscriptionRequest
        {
            CompanyId = company.Id,
            PlanName = "Basic",
            CompanyEmail = request.User.Email
        };

        var subscriptionResult = await _subscriptionService.CreateSubscriptionAsync(subscriptionRequest);

        if (subscriptionResult.Success)
        {
            var subscription = new Subscription
            {
                CompanyId = company.Id,
                PlanName = "Basic",
                Status = SubscriptionStatus.PENDING.ToString(),
                StartDate = DateTime.UtcNow,
                ExternalId = subscriptionResult.ExternalId
            };

            _unitOfWork.SubscriptionRepository.Create(subscription);
            await _unitOfWork.Save();
        }

        return subscriptionResult;
    }

    public async Task<SubscriptionInfo?> GetCompanySubscription(Guid companyId)
    {
        var subscription = await _unitOfWork.SubscriptionRepository.Find(s => s.CompanyId == companyId);

        if (subscription == null)
            return null;

        return new SubscriptionInfo
        {
            Id = subscription.Id,
            PlanName = subscription.PlanName,
            Status = subscription.Status,
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            ExternalId = subscription.ExternalId
        };
    }

    public async Task CancelCompanySubscription(Guid companyId)
    {
        var subscription = await _unitOfWork.SubscriptionRepository.Find(s => s.CompanyId == companyId);

        if (subscription == null)
            throw new Exception("Subscription not found");

        if (!string.IsNullOrEmpty(subscription.ExternalId))
        {
            await _subscriptionService.CancelSubscriptionAsync(subscription.ExternalId);
        }

        subscription.Status = SubscriptionStatus.CANCELED.ToString();
        subscription.EndDate = DateTime.UtcNow;

        await _unitOfWork.Save();
    }
}
