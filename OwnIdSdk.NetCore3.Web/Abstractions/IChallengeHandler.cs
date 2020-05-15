using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OwnIdSdk.NetCore3.Contracts;
using OwnIdSdk.NetCore3.Web.FlowEntries;

namespace OwnIdSdk.NetCore3.Web.Abstractions
{
    public interface IChallengeHandler<T> where T : class
    {
        Task UpdateProfileAsync(UserProfileFormContext<T> context);

        Task<LoginResult<object>> OnSuccessLoginAsync(string did, HttpResponse response);
    }
}