using System.Net.Http.Headers;
using System.Text.Json;
using Moe.Core.Null;

namespace Moe.Core.Helpers;

public class OAuthGoogleClient
{
    private const string TOKEN_REQUEST_URI = "https://oauth2.googleapis.com/token";
    private const string USER_INFO_REQUEST_URI = "https://www.googleapis.com/oauth2/v2/userinfo";
    
    private readonly string _id;
    private readonly string _secret;
    private readonly string _redirectUri;
    public OAuthGoogleClient(string id, string secret, string redirectUri)
    {
        _id = id;
        _secret = secret;
        _redirectUri = redirectUri;
    }

    public async Task<string> GetUserEmailAsync(string accessToken)
    {
        using var httpClient = new HttpClient();

        #region Retrieve Email
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        var userInfoResponse = await httpClient.GetAsync(USER_INFO_REQUEST_URI);
        try
        {
            userInfoResponse.EnsureSuccessStatusCode();
        }
        catch
        {
            ErrResponseThrower.BadRequest();
        }

        var userInfoResponseContent = await userInfoResponse.Content.ReadAsStreamAsync();
        var email = JsonDocument.Parse(userInfoResponseContent).RootElement.GetProperty("email").ToString();
        #endregion

        return email;
    }
}

public class OAuthLinkedinClient
{
    private const string USER_INFO_REQUEST_URI =
        "https://api.linkedin.com/v2/userinfo";

    private readonly string _id;
    private readonly string _secret;

    public OAuthLinkedinClient(string id, string secret)
    {
        _id = id;
        _secret = secret;
    }

    public async Task<string> GetUserEmailAsync(string accessToken)
    {
        using var httpClient = new HttpClient();

        // Set the Authorization header with the Bearer token
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        
        // Make the GET request to fetch user information
        var userInfoResponse = await httpClient.GetAsync(USER_INFO_REQUEST_URI);
        
        // Ensure the response is successful
        userInfoResponse.EnsureSuccessStatusCode();
        
        // Read the response content
        var userInfoResponseContent = await userInfoResponse.Content.ReadAsStringAsync();
        
        // Parse the JSON response to get the email
        using var jsonDoc = JsonDocument.Parse(userInfoResponseContent);
        var email = jsonDoc.RootElement.GetProperty("email").GetString();
        
        return email;
    }
}
