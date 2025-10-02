using Microsoft.Extensions.Options;
using Techugo.POS.ECom.Model;
using Techugo.POS.ECOm;
using Techugo.POS.ECOm.ApiClient;

public static class ApiServiceFactory
{
    public static ApiService Create()
    {
        var apiSettingsOptions = App.ServiceProvider?.GetService(typeof(IOptions<ApiSettings>)) as IOptions<ApiSettings>;
        if (apiSettingsOptions == null)
            throw new System.Exception("ApiSettings not configured.");

        return new ApiService(apiSettingsOptions, TokenService.BearerToken);
    }
}