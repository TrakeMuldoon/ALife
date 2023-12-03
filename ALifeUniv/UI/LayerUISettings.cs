using ALifeUni.ALife;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ALifeUni.UI
{
    [DebuggerDisplay("{LayerName}:{ShowLayer}")]
    public class LayerUISettings
    {
        public String LayerName { get; set; }

        public Boolean ShowLayer { get; set; }
        public Boolean ShowObjects { get; set; }
        public Boolean ShowBoundingBoxes { get; set; }
        public Boolean ShowSenses { get; set; }
        public Boolean ShowSenseBoundingBoxes { get; set; }

        public LayerUISettings(string layerName) : this(layerName, false) { }

        public LayerUISettings(string layerName, bool showLayer) : this(layerName, showLayer, showLayer, showLayer, showLayer, showLayer) { }

        public LayerUISettings(string layerName, bool showLayer, bool showObjects, bool showBoundingBoxes, bool showSenses, bool showSenseBoundingBoxes)
        {
            LayerName = layerName;
            ShowLayer = showLayer;
            ShowObjects = showObjects;
            ShowBoundingBoxes = showBoundingBoxes;
            ShowSenses = showSenses;
            ShowSenseBoundingBoxes = showSenseBoundingBoxes;
        }

        public static List<LayerUISettings> GetSettings()
        {
            List<LayerUISettings> settingsList = new List<LayerUISettings>();
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelZone, true, false, false, false, false));
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelPhysical, true, true, false, true, false));
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelSound));
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelScent));
            settingsList.Add(new LayerUISettings(ReferenceValues.CollisionLevelDead));

            return settingsList;
        }
    }
}
