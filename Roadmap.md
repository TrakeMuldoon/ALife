# Current Roadmap
## Short term tasks
* [ ] Implement Actions in BehaviourBrain
* [ ] Implement Constants in BehaviourBrain
* [ ] Refactor all Actions into "ActionClusters"
* [ ] Add Actions into BehaviourCabinet
* [ ] Implement "WAIT" Action
* [ ] Implement Random BehaviourBrain creation
* [ ] TEST complete BehaviourBrain with custom behaviours
* [ ] Update UI to show Behaviours in Right hand Panel
* [ ] Implement Cloning/Mutation
* [ ] Implement no movement countdown to death.
* [ ] Implement Bumping Behaviour to test First Scenario

* First Scenario is : Agents all run around, if they do not move for 100 turns, they automatically die. If they bump into another agent, the passive agent dies, and the active agent produces an offspring 
** Desired Behaviour evolves: See Something, RUN FORWARD HARD. Or perhaps running forward as fast as they can blindly, might still work. 

## Goal-based Features
The goal is provided/set to the agents; they do not decide at this time.
* [ ] Rotation to Goal Input
* [ ] Distance to Goal Input

## Procedural Features
Required to start running simulations; currently, these are configured/provided.
* [ ] Eyeball Generator
* [ ] Random Brain Generator
* [ ] Replace Brain Paradigm with Neural Network approach

## Other things
* [ ] Custom Environment Setup / Provide a configuration and a means to configure the environment build
   * [ ] Includes Custom Starting Points for individual Agents
   * [ ] Specified Goals for All Agents / Groups of Agents / up to Individual Agents
* [ ] External Fitness Function -- the concept here is that there may be some environment-level calculations required to determine the fitness of the agents; e.g., if you want to determine who the fastest agent is, the agents don't decide that / there is some evaluator we can build to query for such
* [ ] Reproduction, "Asexual"; requires:
   * [ ] Clone functionality - cloning a subset of the attributes; creating a child with identical rules + brain* [ ] (brains can only be RNG'd - this needs to be changed, first)
   * [ ] "Brain Mutability" concept - certain aspects of the NN can be modified, but need to temper this to not completely change the behaviour(s) from the parent
   * [ ] "Body Mutability" concept - number and orientation/power of each of their sense/actions
   * [ ] "Variable mutability" - each agent is determining how mutable its offspring is

## End Goals
* [ ] Run Foot Traffic Simulation - requires: Goals, Death Condition, and Base Clone Functionality (with custom start point)

## End-of-Run Tasks
* [ ] Inspect End Of Run - some meaningful report at end of run (TBD); e.g., might include ability to export the best agent(s), or a selection based on some criterion/-ia

## Runtime Tasks
* [ ] Inspect Individual Agent - window to show agent stats

## Senses and Behaviours
* [ ] ProximityDetectorInput
* [ ] SenseClusters: Most senses are composites of multiple values (0-255); e.g., ears could be "volume of sound", "pitch of sound" and "calibre/timbre of sound". This could include some optimizations about detection. Senses we want to target:
   * [ ] Eyes
   * [ ] Ears
   * [ ] Nose
   * [ ] Touch

## Actions
Actions are outputs of the NN, anything which the agent could "choose" to do as a result of the inputs.
* [ ] Bite
* [ ] Idle
* [ ] Move
* [ ] Rotate
_Note_: We could make composite actions as in "veer right = move forward + rotate right" kind of thing.

### Agent properties connection
This will be its own section, but for now we need to be aware/cognizant that this will have an effect in the Action domain.
* [ ] "Energy" - Actions, Inputs, nothings, could all have energy costs.

### Action-related Effects
We need a way for an agent to respond to its own actions and the results of its own actions.
* [ ] Actions as Inputs
* [ ] Inputs as Actions - based on a state, choose inputs
* [ ] Reactions - when something happens to an agent
   * [ ] Collision -> Collide
   * [ ] Bite -> Bitten
   * [ ] Punch -> Punched

## Configurability
* [ ] Custom Objects (Pineapples)

## Miscellaneous
* [ ] AmbientFood? (relies on "Energy")
* [ ] Live editing of Agents
* [ ] Saving of Current State (Including flag for "edited or not")
