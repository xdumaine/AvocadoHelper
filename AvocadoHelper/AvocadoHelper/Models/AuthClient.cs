using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;

namespace Avocado.Models
{

    class AuthClient
    {

        private static string API_URL_BASE = "https://avocado.io/api/";
        private static string API_URL_LOGIN = API_URL_BASE + "authentication/login";
        private static string API_URL_COUPLE = API_URL_BASE + "couple";
        private static string API_URL_ACTIVITIES = API_URL_BASE + "activities";
        private static string API_URL_LISTS = API_URL_BASE + "lists/";
        private static string API_URL_CONVERSATION = API_URL_BASE + "conversation/";
        private static string API_URL_CALENDAR = API_URL_BASE + "calendar/";
        private static string API_URL_MEDIA = API_URL_BASE + "media/";
        private static string COOKIE_NAME = "user_email";
        private static string USER_AGENT = "Avocado Windows 8 Client v.1.0";
        private static string ERROR_MSG = "\nFAILED.  Signature was tested and failed. Try again and check the auth information.";
        private static string API_DEV_KEY = "xnWY1vZw064tCoigoeLIUt9wkfpjg2x6DpKjZzEn4YlYAhyULGTuHsBoyKXd+a3x";
        private static string API_DEV_ID = "46";


        public string Email { get; set; }
        public string Password { get; set; }
        private string cookieValue;
        public string CookieValue {
            get
            {
                return cookieValue;
            }
            set
            {
                cookieValue = value;
            }
        }
        private string avoSignature;
        public string AvoSignature {
            get
            {
                return avoSignature;
            }
            set
            {
                avoSignature = value;
            }
        }

        public AuthClient(string sessionSignature, string sessionCookie) 
        {
            if (!string.IsNullOrEmpty(sessionSignature) && !string.IsNullOrEmpty(sessionCookie))
            {
                AvoSignature = sessionSignature;
                CookieValue = sessionCookie;
            }
        }

        #region DataAccessHelpers

        private HttpResponseMessage PostBuffer(Uri uri, MultipartFormDataContent content)
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));

                var response = client.PostAsync(uri, content);

                return response.Result;

            }
        }

        private HttpResponseMessage Post(Uri uri, FormUrlEncodedContent body)
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));
                var response = client.PostAsync(uri, body);

                return response.Result;

            }
        }

        private HttpResponseMessage Get(Uri uri)
        {
            var baseAddress = new Uri(API_URL_BASE);
            var cookieContainer = new CookieContainer();
            using (var handler = new HttpClientHandler() { CookieContainer = cookieContainer })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                client.DefaultRequestHeaders.Add("User-Agent", USER_AGENT);
                client.DefaultRequestHeaders.Add("X-AvoSig", AvoSignature);
                cookieContainer.Add(baseAddress, new Cookie(COOKIE_NAME, CookieValue));

                var response = client.GetAsync(uri);
                return response.Result;
            }
        }
        
        #endregion

        #region GetData

        public Couple GetUsers()
        {
            var uri = new Uri(API_URL_COUPLE);
            var response = Get(uri);
            response.EnsureSuccessStatusCode();

            var couple = JsonConvert.DeserializeObject<Couple>(response.Content.ReadAsStringAsync().Result);
            return couple;
        }

        public List<Activity> GetActivities()
        {
            var uri = new Uri(API_URL_ACTIVITIES);
            var response = Get(uri);
            response.EnsureSuccessStatusCode();
            
            var activities = JsonConvert.DeserializeObject<List<Activity>>(response.Content.ReadAsStringAsync().Result);
            return activities;
        }

        #endregion

    }
}