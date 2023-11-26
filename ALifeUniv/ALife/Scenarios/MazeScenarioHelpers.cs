using ALifeUni.ALife.CustomWorldObjects;
using ALifeUni.ALife.Utility;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;

namespace ALifeUni.ALife.Scenarios
{
    public static class MazeScenarioHelpers
    {
        public static void SetUpMaze()
        {
            List<Wall> walls = MazeScenarioHelpers.GetMazeWalls();

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

            int x_offset = 300;

            walls.Add(new Wall(new Point(x_offset, 410), 850, new Angle(75), "w1-1"));
            walls.Add(new Wall(new Point(x_offset, 1590), 850, new Angle(105), "w1-2"));
            walls.Add(new Wall(new Point(x_offset + 220, 410), 850, new Angle(105), "w1-3"));
            walls.Add(new Wall(new Point(x_offset + 220, 1590), 850, new Angle(75), "w1-4"));

            x_offset += 450;
            walls.Add(new Wall(new Point(x_offset, 200), 200, new Angle(80), "w2-1"));
            walls.Add(new Wall(new Point(x_offset, 500), 200, new Angle(100), "w2-2"));
            walls.Add(new Wall(new Point(x_offset, 800), 200, new Angle(80), "w2-3"));
            walls.Add(new Wall(new Point(x_offset, 1100), 200, new Angle(100), "w2-4"));
            walls.Add(new Wall(new Point(x_offset, 1400), 200, new Angle(80), "w2-5"));
            walls.Add(new Wall(new Point(x_offset, 1700), 200, new Angle(100), "w2-6"));

            x_offset += 150;
            for(int j = 1; j < 10; j++)
            {
                int yVal = j * 200;
                int angleVal = ((j * 30) % 90) - 10;
                walls.Add(new Wall(new Point(x_offset, yVal), 175, new Angle(angleVal), "w3-" + j));
            }

            x_offset += 200;
            walls.Add(new Wall(new Point(x_offset, 390), 800, new Angle(80), "w4-1"));
            walls.Add(new Wall(new Point(x_offset + 20, 1450), 1100, new Angle(95), "w4-2"));
            walls.Add(new Wall(new Point(x_offset + 140, 390), 800, new Angle(100), "w4-3"));
            walls.Add(new Wall(new Point(x_offset + 120, 1450), 1100, new Angle(85), "w4-4"));

            x_offset += 350;
            for(int k = 1; k < 20; k++)
            {
                int val = k * 100;
                walls.Add(new Wall(new Point(x_offset, val), 100, new Angle(350), "w5-" + k));
            }

            x_offset += 200;
            walls.Add(new Wall(new Point(x_offset + 50, 740), 1500, new Angle(85), "w6-1"));
            walls.Add(new Wall(new Point(x_offset + 60, 1800), 450, new Angle(105), "w6-2"));
            walls.Add(new Wall(new Point(x_offset + 180, 1800), 450, new Angle(75), "w6-3"));
            walls.Add(new Wall(new Point(x_offset + 180, 740), 1500, new Angle(95), "w6-4"));

            x_offset += 400;
            for(int m = 1; m < 11; m++)
            {
                int val = (m * 200) - 150;
                walls.Add(new Wall(new Point(x_offset + 10, val), 100, new Angle(340), "w7-" + m));
                walls.Add(new Wall(new Point(x_offset + 10, val + 80), 100, new Angle(20), "w7-" + m + "_1"));
            }

            x_offset += 100;
            for(int n = 1; n < 20; n++)
            {
                int val = (n * 100) - 50;
                walls.Add(new Wall(new Point(x_offset + 30, val + 25), 100, new Angle(342), "w8-" + n));
                walls.Add(new Wall(new Point(x_offset + 30, val + 55), 100, new Angle(18), "w8-" + n + "_1"));
            }

            x_offset += 200;
            walls.Add(new Wall(new Point(x_offset + 50, 230), 450, new Angle(85), "w9-1"));
            walls.Add(new Wall(new Point(x_offset + 43, 1260), 1500, new Angle(92), "w9-2"));
            walls.Add(new Wall(new Point(x_offset + 98, 1260), 1500, new Angle(88), "w9-3"));
            walls.Add(new Wall(new Point(x_offset + 90, 230), 450, new Angle(95), "w9-4"));

            return walls;
        }
    }
}
