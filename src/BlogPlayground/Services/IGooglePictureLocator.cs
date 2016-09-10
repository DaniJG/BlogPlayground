using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BlogPlayground.Services
{
    public interface IGooglePictureLocator
    {
        Task<string> GetProfilePictureAsync(ExternalLoginInfo info);
    }
}