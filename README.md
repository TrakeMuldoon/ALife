# ALife

This system is intended so simulate a set of agents following a set of non-declarative rules, interacting with each other with the hopes to show off some emergent behaviour(s).

Controls

the "Q" and "E" keys will zoom in and out.

There is a Seed Input at the top. Once it is changed, you can click "reset" and the world will be reset to whatever is currently in the Seed textbox.
Stupid Fast does "200" turns without pausing.
Obscenely Fast does 5000 turns without pausing and usually takes 5-10 seconds. The UI is blanked during this time to save cpu.

Show Ancestry tickbox will show the ancestry of each living agent. It highlights the "Eldest Living" in a blue circle.

The Smallest Brain, Oldest, and Most Children buttons will CYCLE through any tied agents who match that description and Select them.

Clicking on an agent will "Select" them. They will be circled in Red and their information displayed in the Agent Ifnromation display on the right.

Holding "X" while an agent is selected will show the "shadow world" which is what the agent saw last turn. This is really only useful when the simulation is paused.

CTRL-Clicking on empty space will create a "rock" in that space. This sometimes causes bugs, because most things don't know what to do when they collide with rocks. 
CTRL-Clicking on an object will kill that object. KILL.
