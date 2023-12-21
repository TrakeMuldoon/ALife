﻿using System;
using System.Collections.Generic;
using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.AgentActions;

namespace ALife.Core.WorldObjects.Agents.AgentActions
{
    public abstract class ActionCluster
    {
        public readonly string Name;
        public readonly Dictionary<string, ActionPart> SubActions = new Dictionary<string, ActionPart>();
        protected readonly Agent self;

        public ActionCluster(Agent self, String name)
        {
            this.self = self;
            Name = name;
        }

        public bool ActivatedLastTurn
        {
            get;
            private set;
        }

        protected abstract bool ValidatePreconditions();
        protected abstract bool SubActionsEngaged();
        public virtual void ActivateAction()
        {
            ActivatedLastTurn = false;
            bool preconditionsPassed = ValidatePreconditions();
            if(preconditionsPassed)
            {
                if(SubActionsEngaged())
                {
                    //Limit the Intensity.
                    foreach(ActionPart ap in SubActions.Values)
                    {
                        ap.Clamp();
                    }
                    bool success = AttemptEnact();
                    if(success)
                    {
                        SuccessResults();
                    }
                    else
                    {
                        FailureResults();
                    }
                    ActivatedLastTurn = true;
                }
            }
            foreach(ActionPart ap in SubActions.Values)
            {
                ap.Reset();
            }
        }

        protected abstract bool AttemptEnact();
        protected abstract void SuccessResults();
        protected abstract void FailureResults();

        public abstract string LastTurnString();
        public abstract ActionCluster CloneAction(Agent newParent);
    }
}
