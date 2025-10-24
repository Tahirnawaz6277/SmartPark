using SmartPark.Dtos.Billing;

namespace SmartPark.Services.Interfaces
{
    public interface IBillingService
    {
        Task<BillingResponse> CreateBillingAsync(BillingRequest request, CancellationToken cancellationToken);
        Task<BillingResponse> UpdateBillingAsync(Guid id, BillingRequest request, CancellationToken cancellationToken);
        Task<bool> DeleteBillingAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<BillingDto>> GetAllBillingsAsync(CancellationToken cancellationToken);
        Task<IEnumerable<BillingDto>> GetMyBillingsAsync(CancellationToken cancellationToken);
        Task<BillingDto?> GetBillingByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
