using Domain;
using OutputTests.Client.Models;

namespace OutputTests.Client 
{
    public interface ITestAPIClient
    {
        Task<OperationResult> PostOriginationGetChaseYetToFundEmailCandidatesAndEmailAsync(int lomHoursOldboolean affiliate);
    }
}