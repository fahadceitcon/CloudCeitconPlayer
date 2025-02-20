using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ceitcon_Player.Utilities
{
    class PlayerSettings
    {
        public PlayerSettings()
        {
            AllPlayerSettings = new List<PlayerSetting>();
        }

        public static List<PlayerSetting> AllPlayerSettings { get; set; }

        public static PlayerSetting GetPlayerSetting(string playersettingname)
        {
            try
            {
                foreach (PlayerSetting setting in AllPlayerSettings)
                {
                    if (setting.PlayerSettingName.ToLower() == playersettingname.ToLower())
                    {
                        return setting;
                    }
                }
                return null;
            }
            catch { return null; }
        }
    }

    class PlayerSetting
    {
        public string PlayerSettingName { get; set; }
        public int PlayerSettingTypeID { get; set; }
        public string PlayerSettingValue { get; set; }
    }
}