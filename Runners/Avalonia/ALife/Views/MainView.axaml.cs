using ALife.Core.Scenarios;
using ALife.ViewModels;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ALife.Views;

public partial class MainView : UserControl
{
    private MainViewModel _mvm => (MainViewModel)DataContext;

    public MainView()
    {
        InitializeComponent();

        var startingScenario = ScenarioRegister.GetAutoStartScenario();
        if(startingScenario != null)
        {
            var scenarioName = startingScenario.Value.Item1;
            var scenarioSeed = startingScenario.Value.Item2;
            var mode = startingScenario.Value.Item3;
            // TODO: Start the scenario in the requested mode (Visual or Console)
        }
        else
        {
            // otherwise setup the DataContext
            this.DataContext = new MainViewModel();

        }
    }

    public void ScenariosList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox)sender;
        var selectedItem = listBox.SelectedItem?.ToString() ?? string.Empty;
        _mvm.SelectScenario(selectedItem);
    }

    public void SeedSuggestions_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var listBox = (ListBox)sender;
        var selectedItem = listBox.SelectedItem?.ToString() ?? string.Empty;
        _mvm.SelectSeed(selectedItem);
    }

    public void LaunchRunner_Click(object sender, RoutedEventArgs args)
    {
        if(true) ;
        //message.Text = "Button clicked!";
    }

    public void LaunchGui_Click(object sender, RoutedEventArgs args)
    {
        //message.Text = "Button clicked!";
    }
}
