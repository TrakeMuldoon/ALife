using ALifeUni.ALife;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALifeUni.UI
{
    [DebuggerDisplay("{LayerName}:{ShowLayer}")]
    public class LayerUISettings
    {
        public String LayerName { get; set; }
        public Boolean ShowLayer { get; set; }
        public Boolean ShowObjects { get; set; }
        public Boolean ShowAgents { get; set; }
        public Boolean ShowBoundingBoxes { get; set; }
        public Boolean ShowSenses { get; set; }

        public LayerUISettings(string layerName) : this(layerName, false) { }

        public LayerUISettings(string layerName, bool showLayer) : this(layerName, showLayer, true, true, false, false) {  }

        public LayerUISettings(string layerName, bool showLayer, bool showObjects, bool showAgents, bool showBoundingBoxes, bool showSenses)
        {
            LayerName = layerName;
            ShowLayer = showLayer;
            ShowObjects = showObjects;
            ShowAgents = showAgents;
            ShowBoundingBoxes = showBoundingBoxes;
            ShowSenses = showSenses;
        }

        public static List<LayerUISettings> GetSettings()
        {
            List<LayerUISettings> settingsList = new List<LayerUISettings>();
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelZone));
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelPhysical, true));
            settingsList.Add(new LayerUISettings(ReferenceValues.COllisionLevelSight));
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelSound));
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelScent));
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelDead));

            return settingsList;
        }
    }
}
