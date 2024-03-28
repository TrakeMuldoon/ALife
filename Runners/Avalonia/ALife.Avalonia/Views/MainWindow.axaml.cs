using Avalonia;
using Avalonia.Controls;
using System;

namespace ALife.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        ZIndex = 10;
    }
}
