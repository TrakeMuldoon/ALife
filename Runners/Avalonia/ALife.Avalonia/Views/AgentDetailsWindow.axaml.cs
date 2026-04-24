using ALife.Avalonia.ViewModels;
using ALife.Core;
using Avalonia.Controls;
using Avalonia.Threading;
using System;

namespace ALife.Avalonia.Views;

public partial class AgentDetailsWindow : Window
{
    private AgentDetailsViewModel? _vm;
    private DispatcherTimer? _refreshTimer;

    // Parameterless constructor required by Avalonia's XAML compiler.
    public AgentDetailsWindow()
    {
        InitializeComponent();
    }

    public AgentDetailsWindow(AgentDetailsViewModel vm) : this()
    {
        _vm = vm;
        DataContext = vm;
        Title = $"Agent: {vm.AgentName}";

        _refreshTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(250) };
        _refreshTimer.Tick += OnRefreshTick;
        _refreshTimer.Start();
    }

    private void OnRefreshTick(object? sender, EventArgs e)
    {
        if (_vm == null) return;
        int turn = Planet.HasWorld ? Planet.World.Turns : 0;
        bool stillAlive = _vm.Refresh(turn);
        if (!stillAlive)
            _refreshTimer?.Stop();
    }

    protected override void OnClosed(EventArgs e)
    {
        _refreshTimer?.Stop();
        base.OnClosed(e);
    }
}
