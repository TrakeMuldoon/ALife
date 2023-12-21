/*** 
 * This is a historical artifact, and not currently in use 
 * It remains because it might represent something of value someday
 * And it reflects a time when I was very thoroughly thinking about the design of the inheritance tree
 * */
using ALifeUni.ALife.Geometry;
using System;
using System.Collections.Generic;
using Windows.Foundation;


namespace ALifeUni.Configs
{
#pragma warning disable 0169
#pragma warning disable IDE0051
#pragma warning disable IDE0044
    struct WorldConfig
    {
        int Height;
        int Width;
        int Seed;

        readonly List<ZoneConfig> Zones;
        readonly List<GlobalEOTTrigger> EndOfTurnTriggers;
    }

    struct ZoneConfig
    {
        Point TopLeft;
        int Width;
        int Height;
        String Name;
        String Color; //(String or RGB)

        readonly List<ObjectGroupConfig> ObjectGroups;
    }

    struct ObjectGroupConfig
    {
        DistributionConfig Distribution;
        int NumStartingAgents;
        ObjectConfig cohort; //Could be defined inline or just a name reference to later in the config
    }

    struct DistributionConfig
    {
        ObjectDistributions DistributionType;
        bool RandomAvoidCollisions;
        Angle StraightLineDirection;
        int StraightLineSpacing;
        Point StraightLineStartPoint; //If null, then random
        int StraightLineWrapAroundLength; //If null, then it'll wrap around the whole space
    }

    enum ObjectDistributions
    {
        Random,
        StraightLine,
        SharedStraightLine
    }

    struct ObjectConfig
    {

    }

    struct AgentConfig
    {
        String Genus; //TypeNAme 
        String Color; //STRING || R,G,B
        Evo<Byte> r, g, b;

        ShapeConfig Shape;

        List<SenseConfig> Senses;

        List<ActionConfig> Actions;

        List<PropertyConfig> Properties;

        BrainConfig brain;

        //      ReproductionRules
        //          ReproductionTrigger : <ReproductionTrigger>
        //	ReproductionEffect : <ReproductionEffect>

        ObjectEndOfTurnConfig EndOfTurn;
    }

    struct ObjectEndOfTurnConfig
    {
        List<ObjectEndOfTurnTriggerConfig> Triggers;
    }

    struct ObjectEndOfTurnTriggerConfig
    {
        StateCheckConfig StateCondition;
        PropertyCheckConfig PropCondition;
        ActivationConfig Positive;
    }

    struct StateCheckConfig
    {
        //This is to check anything that is not a property on the Agent
        String ZoneCheck;
        String SurvivalCheck;
    }

    struct ActivationConfig
    {
        //This is for any activity which is not a modification of the agent properties.
        //Reproduction would fall into this
        String Activity;
    }

    struct BrainConfig
    {
        String BrainType; //Random, Tester, BehaviourBrain, etc.
        RandomBrainConfig Rando;
        TesterBrainConfig Tester;
        BehaviourBrainConfig Behaviour;
    }

    struct RandomBrainConfig
    {
        double ActivationPercent;
    }

    struct TesterBrainConfig
    {

    }

    struct BehaviourBrainConfig
    {
        List<String> behaviours;
    }

    struct PropertyConfig
    {
        String Name;
        Type Type;
        Evo<Type> Value;
    }

    struct ActionConfig
    {
        String Type; //Could be known or custom
        List<PropertyCheckConfig> PreActionValidations;
        List<PropertyModificationConfig> Cost;
        List<PropertyCheckConfig> CheckSuccess;
        List<PropertyModificationConfig> PositiveOutcomes;
        List<PropertyModificationConfig> NegativeOutcomes;
    }

    struct PropertyCheckConfig
    {
        String Target; //Myself or "Target"
        String PropertyName; //or "Exists"
        String Operation; //greaterthan, lessthan, equalto
        String CompareTo; //Propertyname or Value
    }

    struct PropertyModificationConfig
    {
        String Target; //Myself or "Target"
        String PropertyName;
        String EffectType; //Plus or Minus
        String Value; //?
    }

    struct SenseConfig
    {
        String Type; //Could be known or custom
        String Name;

        Evo<double> OrientationAroundParent;
        Evo<double> RelativeOrientation;
        ShapeConfig Shape;

        String CollisionLevel; //For Custom ones;

        bool SomethingInput;
        bool HowManyInput;
        bool ColorRInput;
        bool ColorGInput;
        bool ColorBInput;
        bool IntensityInput;
    }

    struct ShapeConfig
    {
        String Type;
        Evo<double> StartingOrientation;

        Evo<double> CircleRadius;

        Evo<double> RectangleWidth;
        Evo<double> RectangleLength;

        Evo<double> SectorRadius;
        Evo<double> SectorSweep;
    }

    struct GlobalEOTTrigger
    {
        String globalProperty;
        String Comparison;
        String Value;
        GlobalAction SuccessResult;
        GlobalAction FailureResult;
    }

    struct GlobalAction
    {
        //Thing to do?
    }

    struct Evo<T>
    {
        T StartValue;
        T StartValueEvoDeltaMax;

        T Value;
        T ValueMax;
        T ValueMin;
        T ValueHardMax;
        T ValueHardMin;

        T DeltaMax;
        T DeltaEvoMax;
        T DeltaHardMax;
    }
}
