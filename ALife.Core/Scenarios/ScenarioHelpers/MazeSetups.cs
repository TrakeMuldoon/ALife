using ALife.Core.Geometry;
using ALife.Core.WorldObjects.Prebuilt;


namespace ALife.Core.Scenarios.ScenarioHelpers
{
    public static class MazeSetups
    {
        public static void SetUpMaze()
        {
            List<Wall> walls = MazeSetups.GetMazeWalls();

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
                new Wall(new Geometry.Shapes.Point(3000, 3), 6000, new Angle(0), "bNorth"),
                new Wall(new Geometry.Shapes.Point(3000, 1997), 6000, new Angle(0), "bSouth"),
                new Wall(new Geometry.Shapes.Point(1, 1000), 2000, new Angle(90), "bWest"),
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

            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, 410), 850, new Angle(75), "w1-1"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, 1590), 850, new Angle(105), "w1-2"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 220, 410), 850, new Angle(105), "w1-3"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 220, 1590), 850, new Angle(75), "w1-4"));

            x_offset += 450;
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, 200), 200, new Angle(80), "w2-1"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, 500), 200, new Angle(100), "w2-2"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, 800), 200, new Angle(80), "w2-3"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, 1100), 200, new Angle(100), "w2-4"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, 1400), 200, new Angle(80), "w2-5"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, 1700), 200, new Angle(100), "w2-6"));

            x_offset += 150;
            for(int j = 1; j < 10; j++)
            {
                int yVal = j * 200;
                int angleVal = ((j * 30) % 90) - 10;
                walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, yVal), 175, new Angle(angleVal), "w3-" + j));
            }

            x_offset += 200;
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, 390), 800, new Angle(80), "w4-1"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 20, 1450), 1100, new Angle(95), "w4-2"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 140, 390), 800, new Angle(100), "w4-3"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 120, 1450), 1100, new Angle(85), "w4-4"));

            x_offset += 350;
            for(int k = 1; k < 20; k++)
            {
                int val = k * 100;
                walls.Add(new Wall(new Geometry.Shapes.Point(x_offset, val), 100, new Angle(350), "w5-" + k));
            }

            x_offset += 200;
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 50, 740), 1500, new Angle(85), "w6-1"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 60, 1800), 450, new Angle(105), "w6-2"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 180, 1800), 450, new Angle(75), "w6-3"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 180, 740), 1500, new Angle(95), "w6-4"));

            x_offset += 400;
            for(int m = 1; m < 11; m++)
            {
                int val = (m * 200) - 150;
                walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 10, val), 100, new Angle(340), "w7-" + m));
                walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 10, val + 80), 100, new Angle(20), "w7-" + m + "_1"));
            }

            x_offset += 100;
            for(int n = 1; n < 20; n++)
            {
                int val = (n * 100) - 50;
                walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 30, val + 25), 100, new Angle(342), "w8-" + n));
                walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 30, val + 55), 100, new Angle(18), "w8-" + n + "_1"));
            }

            x_offset += 200;
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 50, 230), 450, new Angle(85), "w9-1"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 43, 1260), 1500, new Angle(92), "w9-2"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 98, 1260), 1500, new Angle(88), "w9-3"));
            walls.Add(new Wall(new Geometry.Shapes.Point(x_offset + 90, 230), 450, new Angle(95), "w9-4"));

            return walls;
        }

        public static void BuildThinningCarTrack()
        {
            List<Wall> walls = new List<Wall>();

            walls.Add(new Wall(new Geometry.Shapes.Point(25, 400), 400, new Angle(90), "OutsideWest"));
            walls.AddRange(Build60Curve(30, 620, 2, "Outside SW"));
            walls.Add(new Wall(new Geometry.Shapes.Point(760, 784), 1100, new Angle(0), "OutsideSouth"));
            walls.AddRange(Build60Curve(1490, 620, 3, "Outside SE"));
            walls.Add(new Wall(new Geometry.Shapes.Point(1495, 400), 400, new Angle(270), "OutsideEast"));
            walls.AddRange(Build60Curve(1490, 180, 4, "Outside NE"));
            walls.Add(new Wall(new Geometry.Shapes.Point(760, 15), 1100, new Angle(180), "OutsideNorth"));
            walls.AddRange(Build60Curve(30, 180, 1, "Outside NW"));

            walls.Add(new Wall(new Geometry.Shapes.Point(225, 350), 100, new Angle(90), "Inside West"));
            walls.AddRange(Build60Curve(230, 420, 2, "Inside SW"));
            walls.Add(new Wall(new Geometry.Shapes.Point(760, 585), 700, new Angle(0), "Inside South"));
            walls.AddRange(Build60Curve(1290, 420, 3, "Inside SE"));
            walls.Add(new Wall(new Geometry.Shapes.Point(1305, 365), 70, new Angle(105), "Inside East"));
            //walls.AddRange(Build60Curve(1290, 280, 4, "Inside NE"));
            walls.AddRange(Build60Curve(1310, 310, 4, "Inside NE"));
            Wall northy = new Wall(new Geometry.Shapes.Point(768, 130), 730, new Angle(2), "Inside North");
            walls.AddRange(Wall.WallSplitter(northy));

            walls.AddRange(Build60Curve(230, 280, 1, "Inside NW"));

            walls.Add(new Wall(new Geometry.Shapes.Point(128, 330), 200, new Angle(0), "FinishLine"));

            foreach(Wall w in walls)
            {
                Planet.World.AddObjectToWorld(w);
            }
        }

        private static List<Wall> Build60Curve(int xStart, int yStart, int orientation, string name)
        {
            //1 = NW + (->) - (^)  ori 270
            //2 = SW + (->) + (v)  ori 90
            //3 = SE - (<-) + (v)  ori 90
            //4 = NE - (<-) - (^)  ori 270

            //This is a temp solution, could be made more elegant.
            int counter = 0;
            int xSign, ySign, ori;
            switch(orientation)
            {
                case 1: xSign = 1; ySign = -1; ori = 270; break;
                case 2: xSign = 1; ySign = 1; ori = 90; break;
                case 3: xSign = -1; ySign = 1; ori = 90; break;
                case 4: xSign = -1; ySign = -1; ori = 270; break;
                default: throw new Exception("invalid orientation");
            }
            int oSign = xSign * ySign;

            List<Wall> curve = new List<Wall> {
                new Wall(new Geometry.Shapes.Point(xStart + (xSign * 3), yStart + (ySign * 8)), 60, new Angle(ori - (oSign * 15)), $"{name}_{++counter}"),
                new Wall(new Geometry.Shapes.Point(xStart + (xSign * 24), yStart + (ySign * 59)), 60, new Angle(ori - (oSign * 30)), $"{name}_{++counter}"),
                new Wall(new Geometry.Shapes.Point(xStart + (xSign * 57), yStart + (ySign * 102)), 60, new Angle(ori - (oSign * 45)), $"{name}_{++counter}"),
                new Wall(new Geometry.Shapes.Point(xStart + (xSign * 100), yStart + (ySign * 135)), 60, new Angle(ori - (oSign * 60)), $"{name}_{++counter}"),
                new Wall(new Geometry.Shapes.Point(xStart + (xSign * 151), yStart + (ySign * 157)), 60, new Angle(ori - (oSign * 75)), $"{name}_{++counter}")
            };
            return curve;
        }
    }
}
