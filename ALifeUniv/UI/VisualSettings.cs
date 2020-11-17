using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.UI
{
    enum VisualSettingsEnum
    {
        ShowLiveLayer,
        ShowLiveBoundingBox,
        ShowLiveSenses,
        ShowLiveSenseBoundingBox,
        ShowDeadLayer,
        ShowDeadBoundingBox,
        ShowDeadSenses,
        ShowDeadSenseBoundingBox,
        ShowZoneLayer
    }

    class VisualSettings 
    {
        public Dictionary<VisualSettingsEnum, VisualSetting> SettingsEnumMatrix = new Dictionary<VisualSettingsEnum, VisualSetting>();
        public Dictionary<string, VisualSetting> SettingsStringMatrix = new Dictionary<string, VisualSetting>();
        public Dictionary<VisualSettingsEnum, VisualSetting> For;


        public VisualSettings()
        {
            foreach(VisualSettingsEnum ee in Enum.GetValues(typeof(VisualSettingsEnum)))
            {
                VisualSetting nextSetting = new VisualSetting(ee);
                SettingsEnumMatrix.Add(ee, nextSetting);
                SettingsStringMatrix.Add(ee.ToString(), nextSetting);
                For = SettingsEnumMatrix;
            }
        }

        public IEnumerable<VisualSetting> AllValues
        {
            get { return SettingsEnumMatrix.Values; }
        }
    }

    class VisualSetting
    {
        public readonly VisualSettingsEnum Setting;
        public bool IsChecked = true;

        public VisualSetting(VisualSettingsEnum setting)
        {
            Setting = setting;
        }

        public string Name
        {
            get
            {
                return Setting.ToString();
            }
        }
    }

    class AgentSettings
    {
        public bool DrawBoundingBox;
        public bool DrawSenses;
        public bool DrawSenseBoundingBox;

        public AgentSettings(bool drawBoundingBox, bool drawSenses, bool drawSenseBoundingBox)
        {
            DrawBoundingBox = drawBoundingBox;
            DrawSenses = drawSenses;
            DrawSenseBoundingBox = drawSenseBoundingBox;
        }
    }

}
