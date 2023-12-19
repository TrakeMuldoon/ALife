using ALife.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ALifeUni.UI
{
    [DebuggerDisplay("{LayerName}:{ShowLayer}")]
    public class LayerUISettings
    {
        public String LayerName { get; set; }

        public Boolean ShowObjects { get; set; }
        public Boolean ShowBoundingBoxes { get; set; }

        public LayerUISettings(string layerName) : this(layerName, false) { }

        public LayerUISettings(string layerName, bool showLayer) : this(layerName, showLayer, showLayer) { }

        public LayerUISettings(string layerName, bool showObjects, bool showBoundingBoxes)
        {
            LayerName = layerName;
            ShowObjects = showObjects;
            ShowBoundingBoxes = showBoundingBoxes;
        }

        public static List<LayerUISettings> GetDefaultSettings()
        {
            List<LayerUISettings> settingsList = new List<LayerUISettings>
            {
                new LayerUISettings(ReferenceValues.CollisionLevelZone, true),
                new LayerUISettings(ReferenceValues.CollisionLevelPhysical, true),
                new LayerUISettings(ReferenceValues.CollisionLevelSound),
                new LayerUISettings(ReferenceValues.CollisionLevelDead)
            };

            return settingsList;
        }
    }
}
