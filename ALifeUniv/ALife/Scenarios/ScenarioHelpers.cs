using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public static class ScenarioHelpers
    {
        public static void SetUpMaze()
        {
            List<Wall> walls = ScenarioHelpers.GetMazeWalls();

            foreach(Wall w in walls)
            {
                List<Wall> splitsies = Wall.WallSplitter(w);
                foreach(Wall smallWall in splitsies)
                {
                    //Big walls have huge bounding boxes, which impacts performance
                    Planet.World.AddObjectToWorld(smallWall);
                }
            }

            List<Wall> borderWalls = new List<Wall>()
            {
                new Wall(new Point(3000, 3), 6000, new Angle(0), "bNorth"),
                new Wall(new Point(3000, 1997), 6000, new Angle(0), "bSouth"),
                new Wall(new Point(1, 1000), 2000, new Angle(90), "bWest"),
            };

            foreach(Wall w in borderWalls)
            {
                Planet.World.AddObjectToWorld(w);
            }
        }


        public static List<Wall> GetMazeWalls()
        {
            List<Wall> walls = new List<Wall>();
            walls.Add(new Wall(new Point(260, 410), 850, new Angle(75), "w1-1"));
            walls.Add(new Wall(new Point(260, 1590), 850, new Angle(105), "w1-2"));
            walls.Add(new Wall(new Point(480, 410), 850, new Angle(105), "w1-3"));
            walls.Add(new Wall(new Point(480, 1590), 850, new Angle(75), "w1-4"));

            //i 2
            walls.Add(new Wall(new Point(735, 200), 200, new Angle(80), "w5"));
            walls.Add(new Wall(new Point(735, 500), 200, new Angle(100), "w6"));
            walls.Add(new Wall(new Point(735, 800), 200, new Angle(80), "w7"));
            walls.Add(new Wall(new Point(735, 1100), 200, new Angle(100), "w8"));
            walls.Add(new Wall(new Point(735, 1400), 200, new Angle(80), "w9"));
            walls.Add(new Wall(new Point(735, 1700), 200, new Angle(100), "w10"));

            for(int j = 1; j < 10; j++)
            {
                int yVal = j * 200;
                int angleVal = ((j * 30) % 90) - 10;
                walls.Add(new Wall(new Point(900, yVal), 175, new Angle(angleVal), "w3-" + j));
            }

            walls.Add(new Wall(new Point(1100, 390), 800, new Angle(80), "w4-1"));
            walls.Add(new Wall(new Point(1120, 1450), 1100, new Angle(95), "w4-2"));
            walls.Add(new Wall(new Point(1240, 390), 800, new Angle(100), "w4-3"));
            walls.Add(new Wall(new Point(1220, 1450), 1100, new Angle(85), "w4-4"));

            for(int k = 1; k < 20; k++)
            {
                int val = k * 100;
                walls.Add(new Wall(new Point(1400, val), 100, new Angle(350), "w5-" + k));
            }

            walls.Add(new Wall(new Point(1550, 740), 1500, new Angle(85), "w6-1"));
            walls.Add(new Wall(new Point(1560, 1800), 450, new Angle(105), "w6-2"));
            walls.Add(new Wall(new Point(1680, 1800), 450, new Angle(75), "w6-3"));
            walls.Add(new Wall(new Point(1680, 740), 1500, new Angle(95), "w6-4"));


            for(int m = 1; m < 11; m++)
            {
                int val = (m * 200) - 150;
                walls.Add(new Wall(new Point(1810, val), 100, new Angle(340), "w7-" + m));
                walls.Add(new Wall(new Point(1810, val + 80), 100, new Angle(20), "w7-" + m + "_1"));
            }

            for(int n = 1; n < 20; n++)
            {
                int val = (n * 100) - 50;
                walls.Add(new Wall(new Point(1930, val + 25), 100, new Angle(342), "w8-" + n));
                walls.Add(new Wall(new Point(1930, val + 55), 100, new Angle(18), "w8-" + n + "_1"));
            }

            walls.Add(new Wall(new Point(2150, 230), 450, new Angle(85), "w9-1"));
            walls.Add(new Wall(new Point(2143, 1260), 1500, new Angle(92), "w9-2"));
            walls.Add(new Wall(new Point(2198, 1260), 1500, new Angle(88), "w9-3"));
            walls.Add(new Wall(new Point(2190, 230), 450, new Angle(95), "w9-4"));

            return walls;
        }
        public static Color GetRandomColor()
        {
            Color color = new Color()
            {
                R = (byte)Planet.World.NumberGen.Next(100, 255),
                G = (byte)Planet.World.NumberGen.Next(100, 255),
                B = (byte)Planet.World.NumberGen.Next(100, 255),
                A = 255
            };
            return color;
        }
    }
}
