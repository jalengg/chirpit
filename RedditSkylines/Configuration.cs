using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RedditClient
{
    internal class Configuration
    {

        public static List<string> Subreddits;
        public static int TimerInSeconds = 300;
        public static int AssociationMode = 0;
        public static int Hashtags = 0;
        public static int ClickBehaviour = 0;

        public static int FilterMessages = 0;
        public static int LastAnnouncement = 0;


        internal static void SetTimer(string val)
        {
            int newTimer = -1;
            if (Int32.TryParse(val, out newTimer) && newTimer >= 10)
            {
                TimerInSeconds = newTimer;
            }
        }

        internal static void SetAssociationMode(int val)
        {
            AssociationMode = val;
        }

        internal static void SetHashtagMode(bool val)
        {
            Hashtags = val ? 1 : 0;
        }

        internal static void SetFilterMessage(int val)
        {
            FilterMessages = val;
        }
        internal static void SetClickBehavior(int val)
        {
            ClickBehaviour = val;
        }

        internal static void SetSubredditList(string val)
        {
            try
            {
                Subreddits = new List<string>();
                string[] lines = val.Split(
                    new string[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );
                for (int i = 0; i < lines.Length; ++i)
                {
                    string line = lines[i];
                    if (line.Length > 0)
                    {
                        if (line.IndexOf('/') == -1)
                        {
                            line = string.Format("/r/{0}/new", line);
                        }
                        Subreddits.Add(line);
                    }
                }
            }
            catch
            {
                Subreddits = DefaultSubreddits;
                CreateConfig();
            }
        }

        public static List<string> DefaultSubreddits
        {
            get
            {
                var s = new List<string>();
                s.Add("/r/ShowerThoughts/rising");
                s.Add("/r/CrazyIdeas/new");
                s.Add("/r/ChirpIt/new");
                s.Add("/r/AskReddit/rising");
                s.Add("/r/CitiesSkylines/rising");
                s.Add("/r/News/rising");
                s.Add("/r/DoesAnybodyElse/rising");
                return s;
            }
        }

        internal static void CreateConfig()
        {
            using (StreamWriter sw = File.AppendText(ModInfo.ConfigPath))
            {
                foreach (string sub in DefaultSubreddits)
                {
                    sw.WriteLine(sub);
                }
            }
            Subreddits = DefaultSubreddits;
        }

    }
}