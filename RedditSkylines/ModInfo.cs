using ICities;
using ColossalFramework.UI;
using ColossalFramework.IO;
using System.IO;
using System;

namespace RedditClient
{
    public class ModInfo : IUserMod
    {
        public string Description
        {
            get { return "Stream Reddit posts through Chirper"; }
        }

        public string Name
        {
            get { return "Chirpit: Reddit for Chirpy (REBOOTED)"; }
        }

        public static int Version
        {
            get { return 11; }
        }
        public static string ConfigPath
        {
            get
            {
                // base it on the path Cities: Skylines uses
                string path = string.Format("{0}/{1}/", DataLocation.localApplicationData, "ModConfig");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                path += "chirpit-subreddits.txt";

                return path;
            }
        }

        private void OpenConfigFile(ChirpitConfig config)
        {
            string tmppath = ConfigPath;
            if (!System.IO.File.Exists(tmppath))
                ChirpitConfig.CreateSubConfig();
            System.Diagnostics.Process tmpproc = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("notepad.exe", tmppath) { UseShellExecute = false });
            tmpproc.Close();  //this will still cause CSL to not fully release unless user closes notepad.
            ChirpitConfig.SetSubredditList(File.ReadAllText(ConfigPath));
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            ChirpitConfig config = Configuration<ChirpitConfig>.Load();
            UIHelper group = helper.AddGroup("Chirpit Rebooted") as UIHelper;
            UIPanel panel = group.self as UIPanel;
            group.AddCheckbox("Enable automated hashtags", config.Hashtags == 1, (isChecked) => config.SetHashtagMode(isChecked));

            UIDropDown customTagsFilePath = (UIDropDown)group.AddDropdown(
                "Name Handling", 
                new string[] { 
                    "Disable name handling", 
                    "Use CIM names instead of reddit usernames",
                    "Permanently rename CIMs to reddit users" 
                }, 
                config.AssociationMode, 
                (index) => config.SetAssociationMode(index));
            customTagsFilePath.width = panel.width - 30;

            UIDropDown filtersDropdown = (UIDropDown)group.AddDropdown(
                "Filters",
                new string[] {
                    "Include all chirps",
                    "Hide useless chirps",
                    "Hide all chirps"
                },
                config.FilterMessages,
                (index) => config.SetFilterMessage(index));
            filtersDropdown.width = panel.width - 30;

            UIDropDown clickBehaviorDropdown = (UIDropDown)group.AddDropdown(
                 "Clicking on reddit chirps",
                 new string[] {
                    "Open Steam Overlay",
                    "Copy to clipboard",
                    "Open system browser",
                    "Nothing"
                 },
                 config.ClickBehaviour,
                 (index) => config.SetClickBehavior(index));
            clickBehaviorDropdown.width = panel.width - 30;

            UITextField frequencyTextfield = (UITextField)group.AddTextfield(
                "Frequency (seconds)",
                config.TimerInSeconds.ToString(), 
                (value) => config.SetTimer(value), _ => { }
            );

            UIButton configbutton = (UIButton)group.AddButton("Edit/View Subreddit List", () => OpenConfigFile(config));

            // group.AddSlider("My Slider", 0, 1, 0.01f, 0.5f, (value) => UnityEngine.Debug.Log(value));
            // group.AddDropdown("My Dropdown", new string[] { "First Entry", "Second Entry", "Third Entry" }, -1, (index) => Debug.Log(index));
            // group.AddSpace(250);
            // group.AddButton("My Button", () => { Debug.Log("Button clicked!"); });
        }
    }
}
