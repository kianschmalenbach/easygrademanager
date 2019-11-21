using EasyGradeManager.Models;
using EasyGradeManager.Static;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace EasyGradeManager.Controllers.API
{
    public class AuthController : ApiController
    {
        public HttpResponseMessage Post([FromBody]string value)
        {
            Authorize auth = new Authorize();
            User user = auth.GetAuthorizedUser(value);
            if (user == null)
                return new HttpResponseMessage(HttpStatusCode.Unauthorized);
            HttpResponseMessage resp = new HttpResponseMessage();
            CookieHeaderValue cookie = new CookieHeaderValue("user", user.Id.ToString() + "&" + user.Identifier + "&" + auth.GetPassword(value))
            {
                Expires = DateTimeOffset.Now.AddHours(1),
                Domain = Request.RequestUri.Host,
                Path = "/",
                HttpOnly = true,
                Secure = true
            };
            resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
            resp.StatusCode = HttpStatusCode.Moved;
            resp.Headers.Location = new Uri("https://" + Request.RequestUri.Host + ":" + Request.RequestUri.Port + "/Users/" + user.Id);
            return resp;
        }
    }
}