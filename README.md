OneLogin OpenId Connect Dotnet Core 2.0 Sample
==============================================

This sample app demonstrates how to connect to an OpenId Connect Provider like [OneLogin](https://www.onelogin.com)
for user authentication.

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

This will enable a `/signin-oidc` endpoint in the app which you will use for you OpenId Connect app.

## Setting up OpenId Connect with OneLogin
In order to make this sample work with OneLogin you will need to create an OpenId Connect app in your OneLogin portal. See our developer docs for [more detail on how to complete this step](https://developers.onelogin.com/openid-connect).

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