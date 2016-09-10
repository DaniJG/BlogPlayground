using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlogPlayground.Services
{
    public class GooglePictureLocator: IGooglePictureLocator
    {
        public async Task<string> GetProfilePictureAsync(ExternalLoginInfo info)
        {
            var accessToken = info.AuthenticationTokens.SingleOrDefault(t => t.Name == "access_token");
            Uri apiRequestUri = new Uri("https://www.googleapis.com/oauth2/v2/userinfo?access_token=" + accessToken.Value);
            //request profile image
            using (var client = new HttpClient())
            {
                var stringResponse = await client.GetStringAsync(apiRequestUri);
                dynamic profile = JsonConvert.DeserializeObject(stringResponse);
                return profile.picture;
            }
        }
    }
}
