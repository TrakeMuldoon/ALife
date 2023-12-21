using ALife.Core.Geometry.Shapes;
using System;
using System.Drawing;

namespace ALife.Core.WorldObjects.Prebuilt
{
    public class Fruit : WorldObject
    {
        const int FRUIT_RADIUS = 3;

        static int FruitID = 1;
        Zone StartZone = null;

        public Fruit(Geometry.Shapes.Point centrePoint, IShape shape, Color colour, Zone startZone)
             : base(centrePoint, shape, "Fruit", (Fruit.FruitID++).ToString(), ReferenceValues.CollisionLevelPhysical, colour)
        {
            StartZone = startZone;
        }

        public static Fruit FruitCreator(Zone creationZone, Color colour)
        {
            int fruitRadius = FRUIT_RADIUS;
            int fruitDiameter = fruitRadius * 2;
            Geometry.Shapes.Point centrePoint = creationZone.Distributor.NextObjectCentre(fruitDiameter, fruitDiameter);

            Circle fruitCircle = new Circle(centrePoint, fruitRadius);
            Fruit newFruit = new Fruit(centrePoint, fruitCircle, colour, creationZone);
            return newFruit;

        }

        public override WorldObject Clone()
        {
            throw new NotImplementedException();
        }

        public override void Die()
        {
            TrashItem();
        }

        public override void ExecuteAliveTurn()
        {
        }

        public override void ExecuteDeadTurn()
        {
            throw new NotImplementedException();
        }

        public override WorldObject Reproduce()
        {
            throw new NotImplementedException();
        }
    }

}
