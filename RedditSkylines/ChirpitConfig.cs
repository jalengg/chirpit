using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace RedditClient
{
    [ConfigurationPath("RedditForChirpyConfig.xml")]
    public class ChirpitConfig
    {

        public static List<string> Subreddits { get; set;}
        public int TimerInSeconds { get; set; } = 300;
        public int AssociationMode { get; set; } = 0;
        public int Hashtags { get; set; } = 0;
        public int ClickBehaviour { get; set; } = 0;
        public int FilterMessages { get; set; } = 0;
        public int LastAnnouncement { get; set; } = 0;


        public void SetTimer(string val)
        {
            int newTimer = -1;
            if (Int32.TryParse(val, out newTimer) && newTimer >= 10)
            {
                TimerInSeconds = newTimer;
            }
            Configuration<ChirpitConfig>.Save();
        }

        public void SetAssociationMode(int val)
        {
            AssociationMode = val;
            Configuration<ChirpitConfig>.Save();
        }

        public void SetHashtagMode(bool val)
        {
            Hashtags = val ? 1 : 0;
            Configuration<ChirpitConfig>.Save();
        }

        public void SetFilterMessage(int val)
        {
            FilterMessages = val;
            Configuration<ChirpitConfig>.Save();
        }
        public void SetClickBehavior(int val)
        {
            ClickBehaviour = val;
            Configuration<ChirpitConfig>.Save();
        }

        public static void SetSubredditList(string val)
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
                CreateSubConfig();
            }

            Configuration<ChirpitConfig>.Save();
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

        public static void CreateSubConfig()
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