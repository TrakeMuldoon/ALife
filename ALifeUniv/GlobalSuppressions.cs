// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

//This is related to a few cases (especially test scenarios) where properties are throwing "not implemented exceptions", because the are, in fact, not implemented.
//These test scenarios do not require an implementation of those functions, which, under normal scenario circumstances would be required.
[assembly: SuppressMessage("Design", "CA1065:Do not raise exceptions in unexpected locations", Justification = "<Pending>", Scope = "member", Target = "~P:ALifeUni.ALife.Scenarios.ScenarioTemplate.Name")]
