﻿using ALife.Avalonia.ViewModels;
using ALife.Core.ScenarioRunners.ScenarioLoggers;

namespace ALife.Avalonia.ALifeImplementations
{
    /// <summary>
    /// Defines a base implementation for an Avalonia Scenario Runner Logger
    /// </summary>
    /// <seealso cref="ALife.Core.ScenarioRunners.ScenarioLoggers.Logger"/>
    public abstract class AvaloniaLogger(ScenarioRunnerViewModel vm) : Logger
    {
        /// <summary>
        /// The View Model
        /// </summary>
        protected readonly ScenarioRunnerViewModel _vm = vm;
    }
}
