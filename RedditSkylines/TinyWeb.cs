using SimpleJson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using UnityEngine;


namespace RedditClient
{
    internal class TinyWeb
    {
        private string ACCESS_TOKEN = null;
        private string BASE_URL = null;
        private string DEVICE_ID = null;

        public TinyWeb()
        {
            this.DEVICE_ID = genDeviceId();
            this.BASE_URL = "http://www.reddit.com{0}.json?limit={1}";
            JsonObject accessTokenResponse = PostString(getAuthCurlString());
            this.ACCESS_TOKEN = (string)accessTokenResponse["access_token"];
            // UnityEngine.Debug.Log(string.Format("Reddit ACCESS TOKEN IS {0}", ACCESS_TOKEN));
            
        }
        private String getAuthCurlString()
        {
            return string.Format(
                            "-s --location {0} --header 'User-Agent: win64:chirpit:/u/jalen1' --header \"{1}\" --header \"{2}\" --data-urlencode \"{3}\" --data-urlencode \"{4}\"",
                            "https://www.reddit.com/api/v1/access_token",
                            "Authorization: Basic NktLR1YyMUlCelRPQ1YycmczYkFCZzo=",
                            "Content-Type: application/x-www-form-urlencoded",
                            "grant_type=https://oauth.reddit.com/grants/installed_client",
                            string.Format("device_id={0}", this.DEVICE_ID)
                        );
        }
        private static string genDeviceId()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[25];
            var random = new System.Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        public static JsonObject PostString(string args)
        {
            Process p = null;
            try
            {
                UnityEngine.Debug.Log(args);
                var psi = new ProcessStartInfo
                {
                    FileName = "curl",
                    Arguments = args,
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };

                p = Process.Start(psi);
                string readString = p.StandardOutput.ReadToEnd();
                // UnityEngine.Debug.Log(readString);
                return (JsonObject)SimpleJson.SimpleJson.DeserializeObject(readString);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (p != null && p.HasExited == false)
                {
                    p.Kill();
                }
            }
        }


        public IEnumerable<RedditPost> FindLastPosts(string subreddit)
        {

            string args = string.Format("-s --location http://www.reddit.com{0}.json?limit=5 --header 'User-Agent: win64:chirpit:/u/jalen1' --header 'Authorization: Bearer '{1}''", subreddit, this.ACCESS_TOKEN);

            JsonObject response = PostString(args);


            JsonObject responseData = (JsonObject)response["data"];
            JsonArray responseChildren = (JsonArray)responseData["children"];

            if ( responseChildren.Count == 0)
            {
                return null;
            }

            var list = new List<RedditPost>();
            foreach (object obj in responseChildren)
            {
                JsonObject child = (JsonObject)obj;
                JsonObject data = (JsonObject)child["data"];
                UnityEngine.Debug.Log(data.ToString());
                var post = createPost(data);
                if (post != null)
                    list.Add(post);
            }
            return list;
            
            
        }

/*        public static string GetAnnouncement()
 *        
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(ANNOUNCEMENT_URL, ModInfo.Version));
            request.Method = WebRequestMethods.Http.Get;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode != HttpStatusCode.OK)
                    return null;

                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    return sr.ReadLine();
                }
            }
        }*/

        private static RedditPost createPost(JsonObject data)
        {
            // Any karma at all?
            var karma = data["score"];
            if (karma is Int32)
                if ((Int32)karma <= 0)
                    return null;

            // Sticky post?
            var sticky = data["stickied"];
            if (sticky is Boolean)
                if ((Boolean)sticky == true)
                    return null;
            string postText = data["title"].ToString();
            string selfText = data["selftext"].ToString();
            if (selfText.Length > 0 && postText.Length + selfText.Length <= 240)
                postText += ": " + selfText;
            // create post object
            var post = new RedditPost { id = data["id"].ToString(), title = postText, author = data["author"].ToString(), subreddit = data["subreddit"].ToString() };

            // does it have a flair?
            var flair = data["link_flair_text"];
            if (flair != null)
            {
                var flairStr = flair.ToString();
                if (flairStr.Equals("meta", StringComparison.InvariantCultureIgnoreCase))
                    return null;

                post.title += " #" + flairStr.Replace(" ", "");
            }

            return post;
        }
    }

    internal class RedditPost
    {
        internal string title;
        internal string author;
        internal string id;
        internal string subreddit;
    }
}
