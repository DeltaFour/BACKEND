using DeltaFour.Application.Dtos.Requests;
using DeltaFour.Application.Dtos.Responses;
using DeltaFour.Domain.Entities;
using DeltaFour.Domain.Enum;
using DeltaFour.Domain.IRepositories;
using System;

namespace DeltaFour.Application.Services;

public class CompanyRegistrationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISubscriptionService _subscriptionService;
    private readonly IPasswordService _passwordService;

    public CompanyRegistrationService(
        IUnitOfWork unitOfWork,
        ISubscriptionService subscriptionService,
        IPasswordService passwordService
        )
    {
        _unitOfWork = unitOfWork;
        _subscriptionService = subscriptionService;
        _passwordService = passwordService;
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

        var user = new User()
        {
            Name = request.User.Name,
            Email = request.User.Email,
            Password = _passwordService.Hash(request.User.Password!),
            CreatedBy = company.Id,
            CompanyId = company.Id,
            RoleId = role.Id,
            IsActive = true,
            IsConfirmed = true,
        };

        _unitOfWork.CompanyRepository.Create(company);
        _unitOfWork.RoleRepository.Create(role);
        _unitOfWork.UserRepository.Create(user);

        // Default work shifts for newly registered company (8 hours each)
        var matutino = new WorkShift
        {
            ShiftType = ShiftType.Matutino,
            StartTime = new TimeOnly(6, 0),
            EndTime = new TimeOnly(14, 0),
            ToleranceMinutes = 0,
            CompanyId = company.Id,
            CreatedBy = company.Id
        };

        var diurno = new WorkShift
        {
            ShiftType = ShiftType.Diurno,
            StartTime = new TimeOnly(14, 0),
            EndTime = new TimeOnly(22, 0),
            ToleranceMinutes = 0,
            CompanyId = company.Id,
            CreatedBy = company.Id
        };

        var noturno = new WorkShift
        {
            ShiftType = ShiftType.Noturno,
            StartTime = new TimeOnly(22, 0),
            EndTime = new TimeOnly(6, 0),
            ToleranceMinutes = 0,
            CompanyId = company.Id,
            CreatedBy = company.Id
        };

        _unitOfWork.WorkShiftRepository.CreateRange(new[] { matutino, diurno, noturno });

        await _unitOfWork.Save();

        var subscriptionRequest = new CreateSubscriptionRequest
        {
            CompanyId = company.Id,
            PlanName = "Basic",
            CompanyEmail = request.User.Email,
            CustomerName = request.User.Name
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
                ExternalId = !string.IsNullOrEmpty(subscriptionResult.ExternalId) && subscriptionResult.ExternalId.StartsWith("sub_")
                    ? subscriptionResult.ExternalId
                    : null,
                CustomerId = subscriptionResult.CustomerId
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
            ExternalId = subscription.ExternalId,
            CustomerId = subscription.CustomerId
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

    public async Task<SubscriptionResult> ReactivateCompanySubscription(Guid companyId)
    {
        var subscription = await _unitOfWork.SubscriptionRepository.Find(s => s.CompanyId == companyId);

        if (subscription == null)
            throw new Exception("Subscription not found");

        if (subscription.Status != SubscriptionStatus.CANCELED.ToString())
            throw new Exception("Only canceled subscriptions can be reactivated");

        if (string.IsNullOrEmpty(subscription.CustomerId))
            throw new Exception("Customer ID not found. Cannot reactivate subscription.");

        var result = await _subscriptionService.ReactivateSubscriptionAsync(subscription.CustomerId, companyId);

        if (result.Success)
        {
            subscription.Status = SubscriptionStatus.PENDING.ToString();
            subscription.EndDate = null;
            await _unitOfWork.Save();
        }

        return result;
    }

    public async Task<string?> GetPaymentMethodUpdateUrl(Guid companyId, string returnUrl)
    {
        var subscription = await _unitOfWork.SubscriptionRepository.Find(s => s.CompanyId == companyId);

        if (subscription == null)
            throw new Exception("Subscription not found");

        if (string.IsNullOrEmpty(subscription.CustomerId))
            throw new Exception("Customer ID not found");

        return await _subscriptionService.CreatePaymentMethodUpdateSessionAsync(subscription.CustomerId, returnUrl);
    }

    public async Task<string?> GetBillingPortalUrl(Guid companyId, string returnUrl)
    {
        var subscription = await _unitOfWork.SubscriptionRepository.Find(s => s.CompanyId == companyId);

        if (subscription == null)
            throw new Exception("Subscription not found");

        if (string.IsNullOrEmpty(subscription.CustomerId))
            throw new Exception("Customer ID not found");

        return await _subscriptionService.CreateBillingPortalSessionAsync(subscription.CustomerId, returnUrl);
    }
}
