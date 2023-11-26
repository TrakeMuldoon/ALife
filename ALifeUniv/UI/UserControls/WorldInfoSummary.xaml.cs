using ALifeUni.ALife;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace ALifeUni.UI.UserControls
{
    public sealed partial class WorldInfoSummary : UserControl
    {
        public WorldInfoSummary()
        {
            this.InitializeComponent();
        }

        public void UpdateWorldSummary()
        {
            UpdateZoneInfo();
            UpdateGeneology();
            UpdateTurns();
        }

        private void UpdateZoneInfo()
        {
            Dictionary<string, int> zoneCount = new Dictionary<string, int>();
            foreach(Zone z in Planet.World.Zones.Values)
            {
                zoneCount.Add(z.Name, 0);
            }
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < Planet.World.AllActiveObjects.Count; i++)
            {
                WorldObject wo = Planet.World.AllActiveObjects[i];
                if(wo is Agent ag
                    && ag.Alive)
                {
                    zoneCount[ag.Zone.Name]++;
                }
            }
            foreach(string name in zoneCount.Keys)
            {
                sb.AppendLine(name + ":" + zoneCount[name]);
            }
            //sb.AppendLine("WORLD: " + Planet.World.AllActiveObjects.Where(wo => wo.Alive).Count());
            ZoneInfo.Text = sb.ToString();
        }

        private void UpdateGeneology()
        {
            Dictionary<string, int> geneCount = new Dictionary<string, int>();
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < Planet.World.AllActiveObjects.Count; i++)
            {
                WorldObject wo = Planet.World.AllActiveObjects[i];
                if(wo is Agent ag
                    && ag.Alive)
                {
                    string gene = ag.IndividualLabel.Substring(0, 3);
                    if(!geneCount.ContainsKey(gene))
                    {
                        geneCount.Add(gene, 0);
                    }
                    ++geneCount[gene];
                }
            }

            GeneologyInfo.Text = "Genes Active: " + geneCount.Count;
        }

        private void UpdateTurns()
        {
            Turns.Text = Planet.World.Turns.ToString();
        }
    }
}
