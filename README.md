# ALife

Welcome to Devin's Artificial Life Application. This is a simulated world of agents complely independantly learning the rules of their surroundings without any preconcieved notions. 
The worlds are entirely procedural, and given an identical seed, will produce identical results. 
During a simulation, each agent grows, lives, and, if successful, is able to produce an evolved child. There are over a dozen scenarios with different success and failure conditions each trying to learn 
and evolve different behaviours.

# Simulation

This system is intended so simulate a set of agents following a set of non-declarative rules, interacting with each other with the hopes to show off some emergent behaviour(s).

The Agents are little digital creatures that each have an independant brain.

The Brain is any algorithm or function that takes a set of Agent Inputs, and maps them (in any way) to a set of Agent Actions.

An Input is anything that the creature can read numerically, external or internal (True/False values are read as 1/0)
- "MyAge"
- "MyEnergyLevel"
- "DistanceToGoal"
- "RightEye SeeSomething"

Inputs can come in "Input Clusters" which is a set of related inputs. This helps optimize the performance
For Example "Eye Cluster" is a set of inputs 
- DoISeeSomething
- HowManyThings (currently disabled)
- DoISeeBlue
- DoISeeRed
- DoISeeGreen
- DoIRecognize (currently disabled)
- Etc.

Actions are anything the Agent can do
- Move
- Turn
- Bite (not implemented yet)


A Scenario is a set of rules about agents, their interactions with each other and the world.

The Scenarios each contain the definitions of the agents, as well as the definition for what allows them to reproduce or die. It also contains the instructions for the intial set up of the world.

# Using the App
TODO: Documentation in flux, as there are now two differet visualization for the application, the usage for each is different.
The Simulation core is the same for both.

UWP
Avalonia

## UWP Controls

To Run the UWP version of the app, 
    1. open the Solution in Visual Studio.
    2. Under "Runners" folder, under "UWP" folder, is the "ALifeUniv" project.
    3. Right Click on that project and "Set As Startup Project"

The UWP version of the app is still supported, but will not longer be receiving new UI features.

### Startup Screen

### Simulation
The "Q" and "E" keys will zoom in and out.

### Seed Section
The Seed Input is at the top of the control panel. The textbox reveals the worldseed. 
If you click on reset, a new world will be generated with the seed that is in the textbox. If the seed has not changed, then the world will reset.
The "New Seed" Button is cut off by the Agent Panel, but it will generate a new random seed, and start a simulation from that.

### Speed Controls
The Speed Controls section consists of the following buttons. 
 - "Pause", "1 Turn", "Slow", "Play", "Fast"
Each of these descibe the speed of the simulation, and should behave as expected.
 - "FF200", "FF5000", "FF[      ]"
These are the Fast Forward controls. They will suspend the visualization and run the selected number of turns as fast as possible.
While this is running a small panel will appear indicating some stats about the fast-forward.
 - ETA - Estimated time of completion based on the latest segment simulated 
 - ETC - Estimated time of completion based on the cumulative time elapsed in the Fast Forward
 - TPSA - Turns Per Second for the latest segment of simulation fast forwarded
 - TPSC - Turns Per Second cumulative, for the duration of the fast forward

### Zoom Controls
### "Layer Controls"
### "Special Selector Buttons"
### "Wall Panel"
### "Agent Panel"
### "Neural Network Brain Viewer"

### Original Documentation for This stuff
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

CTRL-Clicking on empty space will create a "wall" in that space. If the Scenario does not have a definition for wall collisions, then the behaviour will be undefined and this could cause unexpected behaviour. 
CTRL-Clicking on an object will kill that object. KILL. This can also occasionally cause unexpected behaviours, as it does not follow any scenario rules.

### Avalonia Controls

To Run the Avalonia  version of the app, 
    1. open the Solution in Visual Studio.
    2. Under "Runners" folder, under "Avalonia" folder is the "ALife.Avalonia.Desktop" project.
    3. Right Click on that project and "Set As Startup Project"


## Code

The code is meant to be clean, commented and easier to read. 
It is NOT, however, currently easy to understand then WHOLE of it. 

To change the scenario being executed depends on whether you're running the GUI or the Console App:

- Console App: Update the string in Program.cs that is passed to the `ConsoleScenarioRunner` to the name of the scenario you want to run.
- GUI App: Select the scenario you wish to execute from the list. Suggested seeds are available and you can choose to run either a console-like experience or a GUI experience.

TODO: Assets Folder
TODO: Core Folder
TODO: Documentation Folder
TODO: Runners Folder

