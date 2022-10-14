using Domain;

namespace ApiOrigTests.Client 
{
    public interface IApiOrigClient
    {
        Task<OperationResult> PostOriginationGetChaseYetToFundEmailCandidatesAndEmailAsync(int lomHoursOld, bool affiliate);
    }
}