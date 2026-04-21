using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using ReactiveUI.Avalonia;

namespace AvaloniaUniv.Android;

[Activity(
    Label = "AvaloniaUniv.Core.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity
{
}