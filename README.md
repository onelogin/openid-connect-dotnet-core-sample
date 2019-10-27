# OneLogin OpenId Connect Dotnet Core 3.0 Sample

This sample app demonstrates 2 ways to connect to an OpenId Connect Provider like [OneLogin](https://www.onelogin.com)
for user authentication.

* Authorization Code flow - This is the recommended approach to OpenId Connect authentication. It will redirect the user to a secure hosted login page before returning to your app. See `Startup.cs` for configuring this approach.
* Resource Owner / Password Grant flow - This method is reserved for trusted applications where you capture the username/password and authenticate against OneLogin without redirecting the user to a hosted login page. The main code and configuration for this is found in `Controllers/AccountController.cs`

This app also includes an example of obtaining an OAuth2 `access_token` for use in accessing the [OneLogin Admin APIs](https://developers.onelogin.com/api-docs/1/getting-started/dev-overview). The `Dashboard` route in the `Controllers/HomeController.cs` demonstrates how to use that token to fetch a list of apps that are accessible by a user and then provides a way to launch the apps in `Views/Home/Dashboard.cshtml`.

## Dotnet Core

The base of the project is a Dotnet Core 2.0 MVC project that was generated via command line

```sh
dotnet new mvc --auth None --name OidcSampleApp
```

You will find the majority of the important code in [Startup.cs](Startup.cs) which is where the OpenId Connect Provider is configured.

```csharp
...

    services.AddAuthentication(options => {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
    })
    .AddCookie(options => {
        options.LoginPath = "/Account/Login/";
    })
    .AddOpenIdConnect(options =>
        {
            options.ClientId = Configuration["oidc:clientid"];
            options.ClientSecret = Configuration["oidc:clientsecret"];
            options.Authority = String.Format("https://{0}.onelogin.com/oidc", Configuration["oidc:region"]);

            options.ResponseType = "code";
            options.GetClaimsFromUserInfoEndpoint = true;
        }
    );

...
```

This will enable a `/signin-oidc` endpoint in the app which you will use as the Redirect Uri when configuring your OneLogin OpenId Connect app.

## Customizing your configuratiion

Per the [ASP.NET Configuration documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration), put `oidc:clientid` and `oidc:clientsecret` in your application configuration.

### Using the secret store for configuration (recommended)

Because `oidc:clientid` and `oidc:clientsecret` are application secrets, we recommend NOT putting them in files that might accidentally be checked into version control (your `appsettings*.json` files.)

The [Secret Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets) is our recommended approach:

1. Set your Client ID using the command `dotnet user-secrets set "oidc:clientid" "your-client-id"`
1. Set your Client Secret using the command `dotnet user-secrets set "oidc:clientsecret" "your-client-secret"`

## Setting up OpenId Connect with OneLogin

In order to make this sample work with OneLogin you will need to create an OpenId Connect app in your OneLogin portal. See our developer docs for [more detail on how to complete this step](https://developers.onelogin.com/openid-connect).

Make sure you add `http://localhost:5000/signin-oidc` as an allowed Redirect URI on your OIDC app configuration tab.

You will also need to make sure you configure the **Token Endpoint** for the app in OneLogin
to use the **POST** Authentication method.

![Token Endpoint Authentication Method](https://s3.amazonaws.com/onelogin-screenshots/dev_site/images/client_secret_post.png)

## Run this sample

Pull the repo then from the command line run

```sh
dotnet run
```

Browse to [http://localhost:5000](http://localhost:5000)

## Author

[OneLogin](onelogin.com)

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.