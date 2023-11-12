# Current Roadmap
## Short term tasks
* [ ] Implmenet Arc / Rectangle collision detection (hard)
* [*] Implement no movement countdown to death.
* [*] Implmenet running across the field scenario
* [*] Fix Dead Things View not working
* [*] Implement Keegan's maze concept
* [*] Create Wall Splitter
* [x] Implement Testing Framework / Scenario concept
* [ ] Write list of all possible tests
* [ ] Investigate why Sector/Rectangle Collision Detection isn't working consistently
* [ ] Implement INumeric for every value (min, max, modmax, mod%, value, add, sub);
* [ ] Implement ReadOnlyProperties for Internal/Mandatory values [i.e. Age, NumChildren, etc.]
* [x] Goal Based Features
* [x] Refactor Environment size into Scenarios
* [ ] Give each WorldObject access to its collider (?)
* [ ] Differentiate between the "Visible" and "physical" layers, so that you can there are things which are visible, but no tangible (perhaps smells and fogs?)


## Goal-based Features
The goal is provided/set to the agents; they do not decide at this time.
* [x] Rotation to Goal Input
* [x] Distance to Goal Input

## Procedural Features
Required to start running simulations; currently, these are configured/provided.
* [x] Eyeball Generator
* [*] Random Brain Generator
* [x] Replace Brain Paradigm with Neural Network approach

## Other things
* [x] Custom Environment Setup / Provide a configuration and a means to configure the environment build
   * [x] Includes Custom Starting Points for individual Agents
   * [x] Specified Goals for All Agents / Groups of Agents / up to Individual Agents
* [x] External Fitness Function -- the concept here is that there may be some environment-level calculations required to determine the fitness of the agents; e.g., if you want to determine who the fastest agent is, the agents don't decide that / there is some evaluator we can build to query for such
* [x] Reproduction, "Asexual"; requires:
   * [*] Clone functionality - cloning a subset of the attributes; creating a child with identical rules + brain* [ ] (brains can only be RNG'd - this needs to be changed, first)
   * [x] "Brain Mutability" concept - certain aspects of the NN can be modified, but need to temper this to not completely change the behaviour(s) from the parent
   * [x] "Body Mutability" concept - number and orientation/power of each of their sense/actions
   * [ ] "Variable mutability" - each agent is determining how mutable its offspring is

## End Goals
* [*] Run Foot Traffic Simulation - requires: Goals, Death Condition, and Base Clone Functionality (with custom start point)

## End-of-Run Tasks
* [ ] Inspect End Of Run - some meaningful report at end of run (TBD); e.g., might include ability to export the best agent(s), or a selection based on some criterion/-ia

## Runtime Tasks
* [x] Inspect Individual Agent - window to show agent stats

## Senses and Behaviours
* [*] ProximityDetectorInput
* [ ] SenseClusters: Most senses are composites of multiple values (0-255); e.g., ears could be "volume of sound", "pitch of sound" and "calibre/timbre of sound". This could include some optimizations about detection. Senses we want to target:
   * [x] Eyes
   * [ ] Ears
   * [ ] Nose
   * [ ] Touch

## Actions
Actions are outputs of the NN, anything which the agent could "choose" to do as a result of the inputs.
* [ ] Bite
* [ ] Idle
* [x] Move
* [x] Rotate
_Note_: We could make composite actions as in "veer right = move forward + rotate right" kind of thing.

### Agent properties connection
This will be its own section, but for now we need to be aware/cognizant that this will have an effect in the Action domain.
* [ ] "Energy" - Actions, Inputs, nothings, could all have energy costs.

### Action-related Effects
We need a way for an agent to respond to its own actions and the results of its own actions.
* [x] Actions as Inputs
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
