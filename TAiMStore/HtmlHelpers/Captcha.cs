using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace TAiMStore.HtmlHelpers
{
    public static class Captcha
    {
        public const string SessionKeyPrefix = "_Captcha";
        private const string ImgFormat = "<img src=\"{0}\" alt=\"captcha\" />";

        /// <summary>
        /// Helper for captcha
        /// </summary>
        /// <param name="html">HTML</param>
        /// <param name="name">Name for input password</param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string CaptchaHlp(this HtmlHelper html, string name, int height, int width)
        {
            string challengeGuid = Guid.NewGuid().ToString();
            var session = html.ViewContext.HttpContext.Session;
            session[SessionKeyPrefix + challengeGuid] = MakeRandomSolution(true);

            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            // ReSharper disable Asp.NotResolved
            string url = urlHelper.Action("Render", "Captcha", new { challengeGuid, height, width });
            // ReSharper restore Asp.NotResolved

            return String.Format(ImgFormat, url) + html.Password(name, challengeGuid, new { @style = "display: none;" });
        }
        public static string ConnectCaptchaHlp(this HtmlHelper html, string name, int height, int width)
        {
            string challengeGuid = Guid.NewGuid().ToString();
            var session = html.ViewContext.HttpContext.Session;
            session[SessionKeyPrefix + challengeGuid] = MakeRandomSolution(true);

            var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
            // ReSharper disable Asp.NotResolved
            string url = urlHelper.Action("Render", "Captcha", new { challengeGuid, height, width });
            // ReSharper restore Asp.NotResolved
            return String.Format(ImgFormat, url) + html.Hidden(name, challengeGuid, new { @style = "display: none;" });
        }

        /// <summary>
        /// Get random solution
        /// </summary>
        /// <param name="useNumer">Use numbers in solution</param>
        /// <returns>Solution</returns>
        private static string MakeRandomSolution(bool useNumer)
        {
            var rng = new Random();
            int length = rng.Next(4, 6);
            var buf = new char[length];
            for (int i = 0; i < length; i++)
            {
                var flag = useNumer ? rng.Next(2) : 0;
                if (flag == 0)
                {
                    buf[i] = (char)('a' + rng.Next(26));
                }
                else
                {
                    buf[i] = (char)('0' + rng.Next(10));
                }
            }

            return new string(buf);
        }

        /// <summary>
        /// Verify and expire solution
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="challengeGuid">Challenge guid</param>
        /// <param name="attemptedSolution">Attempted solution</param>
        /// <returns>Result</returns>
        public static bool VerifyAndExpireSolution(HttpContextBase context, string challengeGuid, string attemptedSolution)
        {
            return VerifyAndExpireSolution(context, challengeGuid, attemptedSolution, false);
        }

        /// <summary>
        /// Verify and expire solution
        /// </summary>
        /// <param name="context">Context</param>
        /// <param name="challengeGuid">Challenge guid</param>
        /// <param name="attemptedSolution">Attempted solution</param>
        /// <returns>Result</returns>
        public static bool VerifyAndExpireSolution(HttpContextBase context, string challengeGuid, string attemptedSolution, bool IsCommented)
        {
            var solution = (string)context.Session[SessionKeyPrefix + challengeGuid];
            if (!IsCommented)
            {
                context.Session.Remove(SessionKeyPrefix + challengeGuid);
            }
            return ((solution != null) && (attemptedSolution == solution));
        }
    }
}