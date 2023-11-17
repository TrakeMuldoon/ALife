# Current Roadmap

## Held Over from previous roadmap.
These items may be complete already. It is unclear to me as of right now.
* [ ] Implmenet Arc / Rectangle collision detection (hard)
* [ ] Write list of all possible tests
* [ ] Investigate why Sector/Rectangle Collision Detection isn't working consistently
* [ ] Implement INumeric for every value (min, max, modmax, mod%, value, add, sub);
* [ ] Implement ReadOnlyProperties for Internal/Mandatory values [i.e. Age, NumChildren, etc.]
* [ ] Give each WorldObject access to its collider (Not sure why I thought this was necessary)
* [ ] Differentiate between the "Visible" and "physical" layers, so that you can there are things which are visible, but no tangible (perhaps smells and fogs?)\

### Agent Properties
* [ ] Mutability as an inherited property. Agents have the ability to determine how different their children will be.
### Senses
* [ ] Ear Clusters, Nose Cluster, and their requisite collision layer stuff. This should be trivial, we just don't have a scenario that calls for it yet.
### Actions
* [ ] Bite
* [ ] Idle
* [ ] Punch
* [ ] Instant 180
* [ ] Composite Actions (such as Veer Left)
* [ ] 
### Properties
* [ ] "Energy" - Actions, Inputs, nothings, could all have energy costs. 	
       This is quite a lot of work, as it would need to somehow modify all other actions. This would be very difficult to build in a generic way so it doesn't break existing scenarios.
### Action related Effects
* [ ] Inputs as Actions - based on a state, choose inputs
* [ ] Reactions - when something happens to an agent
   * [ ] Collision -> Collide
   * [ ] Bite -> Bitten
   * [ ] Punch -> Punched

### Scenario
* [ ] Custom Objects (Pineapples)
* [ ] AmbientFood? (relies on "Energy")

### UI
* [ ] Live editing of Agents
* [ ] Saving of Current State (Including flag for "edited or not")

# 2024 Roadmap - Ideas (No Particular Order)
* [ ] Scenario: Fruit trees. Bump into them an fruit falls nearby. Eat 2 (x) fruits, to reproduce. Fruits would be different colour from the agents.
* [ ] Scenario: Predation. Biting other agents (not just bumping) will kill them, and two kills reproduces. 
* [ ] Aggressive Code Cleanup. There are a lot of config defintions scattered around that don't really make sense anymore, and aren't used. They should be put into an archive folder as reference artifacts.
* [ ] Update the UI into popout panels, instead of inline.
* [ ] Update the Scenario Runner to be genericizable. Feed in a Scenario and outputs, etc, instead of replacing them every time. 
* [ ] Find a good optimized C# Neural Network that allows for huge networks efficiently.
* [ ] Modified fitness by time/trigger


Neural Network Open Source projects
https://www.heatonresearch.com/encog/ - Last Release 2017
http://www.aforgenet.com/news/2013.07.17.releasing_framework_2.2.5.html - Last Release 2013
https://sourceforge.net/projects/neurondotnet/ - LAst update of any kind 2015