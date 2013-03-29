using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Avocado.Models;

namespace AvocadoHelper.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public string TileContent(string id, string sessionCookie, string sessionSignature)
        {
            var authClient = new AuthClient(sessionSignature, sessionCookie);
            var activityList = authClient.GetActivities();
            var couple = authClient.GetUsers();
            foreach (var activity in activityList)
            {
                activity.User = activity.UserId == couple.CurrentUser.Id ? couple.CurrentUser : couple.OtherUser;
            }
            activityList.Reverse();

            
            // get the first image, or the first message from the other user
            var message = (from a in activityList
                           where a.IsList == false && a.IsEvent == false
                           && (a.IsMessage == false || a.UserId != id)
                           select a).FirstOrDefault();
            var result = string.Empty;
            if (message.IsMessage)
            {
                var messageTile = "<tile><visual version=\"1\" lang=\"en-US\"><binding template=\"TileWideImageAndText01\">"
                    + "<image id=\"1\" src=\"{0}\"/><text id=\"1\">{1}</text></binding><binding template=\"TileSquareText04\"><text id=\"1\">{1}</text></binding></visual></tile>";
                result = string.Format(messageTile, message.User.AvatarUrl, message.Data.Text);
            }
            else if (message.IsImage)
            {
                var imageTile = "<tile><visual version=\"1\" lang=\"en-US\"><binding template=\"TileWideImage\">"
                    + "<image id=\"1\" src=\"{0}\"/></binding><binding template=\"TileSquareImage\">"
                    + "<image id=\"1\" src=\"{0}\"/></binding></visual></tile>";
                result = string.Format(imageTile, message.Data.ImageUrls.Small);

            }
            return result;
        }
    }
}
