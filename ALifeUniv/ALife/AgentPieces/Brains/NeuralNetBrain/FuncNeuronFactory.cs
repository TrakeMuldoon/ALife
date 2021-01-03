using System;
using System.Collections.Generic;

namespace ALifeUni.ALife.Brains
{
    internal class FuncNeuronFactory
    {
        internal static List<FuncNeuron> GenerateFuncNeuronsForSenseInput(SenseInput si)
        {
            List<FuncNeuron> newNeurons = new List<FuncNeuron>();
            switch(si)
            {
                case Input<bool> sib: 
                    newNeurons.Add(new FuncNeuron(si.Name + ".Value", () => sib.Value ? 1.0 : 0.0));
                    newNeurons.Add(new FuncNeuron(si.Name + ".MostRecentValue", () => sib.MostRecentValue ? 1.0 : 0.0));
                    newNeurons.Add(new FuncNeuron(si.Name + ".Modified", () => sib.Modified ? 1.0 : 0.0));
                    break;
                case Input<double> sid:
                    newNeurons.Add(new FuncNeuron(si.Name + ".Value", () => sid.Value));
                    newNeurons.Add(new FuncNeuron(si.Name + ".MostRecentValue", () => sid.MostRecentValue));
                    newNeurons.Add(new FuncNeuron(si.Name + ".Modified", () => sid.Modified ? 1.0 : 0.0));
                    newNeurons.Add(new FuncNeuron(si.Name + ".Increased", () => sid.Value > sid.MostRecentValue ? 1.0 : 0.0));
                    newNeurons.Add(new FuncNeuron(si.Name + ".Decreased", () => sid.Value < sid.MostRecentValue ? 1.0 : 0.0));
                    break;
                case Input<int> sii:
                    //TODO: How to convert Int to double? Can't. Not until Inputs have type of "Evo"
                    newNeurons.Add(new FuncNeuron(si.Name + ".Modified", () => sii.Modified ? 1.0 : 0.0));
                    newNeurons.Add(new FuncNeuron(si.Name + ".Increased", () => sii.Value > sii.MostRecentValue ? 1.0 : 0.0));
                    newNeurons.Add(new FuncNeuron(si.Name + ".Decreased", () => sii.Value < sii.MostRecentValue ? 1.0 : 0.0));
                    break;
                case Input<string> sis:
                    //TODO: How to convert string to double? Can't. Not until ... something... special happens
                    newNeurons.Add(new FuncNeuron(si.Name + ".Modified", () => sis.Modified ? 1.0 : 0.0));
                    break;
            }

            return newNeurons;
        }
    }
}