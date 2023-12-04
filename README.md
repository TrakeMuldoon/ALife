# ALife

## Introduction

This system is intended so simulate a set of agents following a set of non-declarative rules, interacting with each other with the hopes to show off some emergent behaviour(s).

The Agents are little digital creatures that each have an independant brain.

The Brain is any algorithm or function that takes a set of Agent Inputs, and maps them (in any way) to a set of Agent Actions.

An Input is anything that the creature can read numerically, external or internal (True/False values are read as 1/0)
- "MyAge"
- "MyEnergyLevel"
- "DistanceToGoal"
- "RightEye SeeSomething"

Inputs can come in "Input Clusters" which is a set of related inputs. This helps optimize the performance
- Eye Cluster is a set of inputs 
-- DoISee
-- HowManyThings (currently disabled)
-- DoIRecognize (currently disabled)
-- WhatColor(Avg) (currently disabled)
-- HowMuchColor (currently disabled)
-- Etc.

Actions are anything the Agent can do
- Move
- Turn
- Bite (not implemented yet)

A Scenario is a set of rules about agents, their interactions with each other and the world.

## Using the App

Controls

the "Q" and "E" keys will zoom in and out.

There is a Seed Input at the top. Once it is changed, you can click "reset" and the world will be reset to whatever is currently in the Seed textbox.
FF will fast forward that many turns. The UI is blanked during this time to save cpu.

Beneath that there is a poorly styled grid. 
Each row of the grid represents a "collision layer" in the world.
The columns (which will be labeled eventually) are
    Show Layer
    Show Objects (including agents)
    Show Objects Bounding Boxes
    Show Agent Senses
    Show Agent Senses Bounding Boxes

Show Ancestry tickbox will show the ancestry of each living agent. It highlights the "Eldest Living" in a blue circle.

The Smallest Brain, Oldest, and Most Children buttons will CYCLE through any tied agents who match that description and Select them.

Clicking on an agent will "Select" them. They will be circled in Red and their information displayed in the Agent Information display on the right.

Holding "X" while an agent is selected will show the "shadow world" which is what the agent saw last turn. This is really only useful when the simulation is paused.

CTRL-Clicking on empty space will create a "wall" in that space. This sometimes causes bugs, because most things don't know what to do when they collide with rocks. 
CTRL-Clicking on an object will kill that object. KILL. This is also buggy, because it doesn't always remove them correctly


## Codewise

The code is meant to be clean, commented and easier to read. 
It is NOT, however, currently easy to understand then WHOLE of it. 

To change the scenario being executed find the following comment in the Main.Xaml.cs
"            /* ***********************************
             * Set Scenario Here
             * ***********************************/"
That is where you pick the scenario which will be run. 
The scenarios themselves are under
ALife\ALifeUniv\ALife\Scenarios

And each one should have a header with a description of what it is there for.
Eventually, there will be a UI in the app to select, but that's long term.
