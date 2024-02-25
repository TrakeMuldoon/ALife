# Current Roadmap

## Held Over from previous roadmap.
* [ ] Implmenet Arc / Rectangle collision detection (hard)
* [ ] Investigate why Sector/Rectangle Collision Detection isn't working consistently
* [ ] Implement INumeric for every value (min, max, modmax, mod%, value, add, sub);
* [ ] Differentiate between the "Visible" and "physical" layers, so that you can there are things which are visible, but no tangible (perhaps smells and fogs?)

### Agent Properties
* [ ] Mutability as an inherited property. Agents have the ability to determine how different their children will be.

### Senses
* [ ] Ear Clusters, Nose Cluster, and their requisite collision layer stuff.
* [*] Ear Clusters
* [ ] EarInputs (Volume, Timber, Specific)
* [*] SoundWave Object
* [*] Soundbased Scenario

### Actions
* [ ] Bite
* [ ] Idle
* [ ] Punch
* [x] Instant 180 (Tried it, discarded it. Caused very frenetic behaviour)
* [ ] Composite Actions (such as Veer Left)
* [ ] Perhaps combining Move and Turn into a single Action cluster. This will theoretically cut down on collision checks. (Currently agents are circles, and circles do not do collision checks when they turn)

### Properties
* [ ] "Energy" - Actions, Inputs, nothings, could all have energy costs. 	
       This is quite a lot of work, as it would need to somehow modify all other actions. This would be very difficult to build in a generic way so it doesn't break existing scenarios.

### Action related Effects
* [ ] Inputs as Actions - based on a state, choose inputs
* [ ] Reactions - when something happens to an agent
   * [ ] Collision -> Collide
   * [ ] Bite -> Bitten
   * [ ] Punch -> Punched

### UI
* [ ] Live editing of Agents
* [ ] Saving of Current State (Including flag for "edited or not")

# 2024 Roadmap - Ideas (No Particular Order)
* [ ] Update the UI into popout panels, instead of inline.
* [ ] Find a good optimized C# Neural Network that allows for huge networks efficiently.
* [ ] Modified fitness by time/trigger
* [ ] Implement "Randomable" type
* [ ] Comment Every Class and Function
* [ ] Add new Senses "Collided Left, Collided Right, Collided Back, Collided Front"
* [ ] Reproduction is currently based on the Zone Distributor. Make it so that it could be "next to parent"
* [ ] Get FileLogger Working again
* [ ] Add New GeneologyViewer
* [ ] Agents select their own target
* [ ] Agents can tell target orientation
* [ ] NEAT brain. 
* [ ] Compile different versions of Point and Color for different targets
* [ ] Change GoalSenseCluster so it's UI shows the direction of the target.

## Scenario Ideas
* [ ] Temperature Scenario (Low temps steal energy, high temps restore energy)
* [ ] Fruit Trees, drop food on collision
* [*] Rabbit Chasing Scenario (Implement WorldObjectTargetSense)
* [*] Mushroom scenario. Agents differentiate between good colour and bad colour mushrooms.
* [ ] Scenario: Predation. Biting other agents (not just bumping) will kill them, and two kills reproduces. 
 
## Fixing the Basics - Avalonia
* [*] Time Controls
* [*] Layer Controls
* [*] Zoom Controls
* [ ] ClickSelect
* [ ] AgentInfoPanel
* [ ] Brain Viewers
* [ ] Get rid of black panel.

## Code CleanUp / Refactors
* [ ] Migrate Clone and Reproduce OUT of WorldObjects. They are only relevant to Agents. (... is this true?)
* [*] Aggressive Code Cleanup. There are a lot of config defintions scattered around that don't really make sense anymore, and aren't used. They should be put into an archive folder as reference artifacts.
* [*] Update the Scenario Runner to be genericizable. Feed in a Scenario and outputs, etc, instead of replacing them every time. 
* [ ] Write list of all possible tests
* [ ] Refactor "Die" and "Dead Turn" out of WorldObject in favour of some sort of a a "StateList" for objects.