OneLogin OpenId Connect Dotnet Core 2.0 Sample
==============================================

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
  .AddCookie()
  .AddOpenIdConnect(o =>
      {
          o.ClientId = "<OneLogin OIDC Client ID>";
          o.ClientSecret = "<OneLogin OIDC Client Secret>";
          o.Authority = "https://<OneLogin Subdomain>.onelogin.com/oidc";
          o.ResponseType = "code";
          o.GetClaimsFromUserInfoEndpoint = true;
      }
  );

...
```

Replace the `<OneLogin OIDC Client ID>` etc placeholders with your own values from OneLogin.

Don't hardcode these values in `Startup.cs`. Use config file instead, this was done just
to demonstrate where you should put them.

This will enable a `/signin-oidc` endpoint in the app which you will use as the Redirect Uri when configuring your OneLogin OpenId Connect app.

## Setting up OpenId Connect with OneLogin
In order to make this sample work with OneLogin you will need to create an OpenId Connect app in your OneLogin portal. See our developer docs for [more detail on how to complete this step](https://developers.onelogin.com/openid-connect).

You will also need to make sure you configure the **Token Endpoint** for the app in OneLogin
to use the **POST** Authentication method.

![Token Endpoint Authentication Method](https://s3.amazonaws.com/onelogin-screenshots/dev_site/images/client_secret_post.png)

## Run this sample
Pull the repo then from the command line run

```sh
dotnet restore
dotnet run
```

## Using Ngrok
In order to test end to end you need to expose this project to the internet.
Do this using ngrok which can be easily downloaded using NPM.

```sh
npm install ngrok -g
ngrok http 5000
```

You will then use the Ngrok HTTPS url as the Redirect Uri for your OpenId Connect
App in OneLogin. e.g. https://2afc2196.ngrok.io/signin-oidc

## Author

[OneLogin](onelogin.com)

## License

This project is licensed under the MIT license. See the [LICENSE](LICENSE) file for more info.