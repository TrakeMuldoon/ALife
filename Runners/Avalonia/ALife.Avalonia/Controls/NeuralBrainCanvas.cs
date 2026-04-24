using ALife.Core.WorldObjects.Agents;
using ALife.Core.WorldObjects.Agents.Brains;
using ALife.Core.WorldObjects.Agents.Brains.NeuralNetworkBrains;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Globalization;
using AvPoint = Avalonia.Point;

namespace ALife.Avalonia.Controls;

public class NeuralBrainCanvas : Control
{
    public static readonly DirectProperty<NeuralBrainCanvas, Agent?> AgentProperty =
        AvaloniaProperty.RegisterDirect<NeuralBrainCanvas, Agent?>(nameof(Agent), o => o.Agent, (o, v) => o.Agent = v);

    public static readonly DirectProperty<NeuralBrainCanvas, int> TurnCountProperty =
        AvaloniaProperty.RegisterDirect<NeuralBrainCanvas, int>(nameof(TurnCount), o => o.TurnCount, (o, v) => o.TurnCount = v);

    private Agent? _agent;
    private int _turnCount;
    private Dictionary<Neuron, AvPoint>? _nodeMap;
    private Dictionary<Neuron, List<(Dendrite, Neuron)>>? _downstreamMap;
    private Neuron? _selectedNeuron;
    private int _forgiveness;
    private Size _lastBuiltSize;
    private const int NeuronRadius = 8;

    static NeuralBrainCanvas()
    {
        AffectsRender<NeuralBrainCanvas>(TurnCountProperty);
    }

    public Agent? Agent
    {
        get => _agent;
        set
        {
            SetAndRaise(AgentProperty, ref _agent, value);
            _nodeMap = null;
            _downstreamMap = null;
            _selectedNeuron = null;
            _lastBuiltSize = default;
            InvalidateVisual();
        }
    }

    public int TurnCount
    {
        get => _turnCount;
        set => SetAndRaise(TurnCountProperty, ref _turnCount, value);
    }

    public override void Render(DrawingContext ctx)
    {
        base.Render(ctx);
        if (_agent?.MyBrain is not NeuralNetworkBrain brain) return;
        if (Bounds.Width <= 0 || Bounds.Height <= 0) return;

        if (_nodeMap == null || _lastBuiltSize != Bounds.Size) BuildNodeMap(brain);

        ctx.FillRectangle(new SolidColorBrush(Color.FromRgb(20, 20, 20)),
            new Rect(0, 0, Bounds.Width, Bounds.Height));

        var backup = _nodeMap!;

        // Draw all dendrites
        foreach (var (neuron, pt) in backup)
        {
            foreach (var den in neuron.UpstreamDendrites)
            {
                if (!backup.TryGetValue(den.TargetNeuron, out var targetPt)) continue;
                ctx.DrawLine(new Pen(GetDendriteColor(den.CurrentValue), 1), pt, targetPt);
            }
        }

        // Highlight downstream connections from selected neuron
        if (_selectedNeuron != null && _downstreamMap != null
            && _downstreamMap.TryGetValue(_selectedNeuron, out var downstream)
            && backup.TryGetValue(_selectedNeuron, out var homePt))
        {
            foreach (var (den, parent) in downstream)
            {
                if (!backup.TryGetValue(parent, out var targetPt)) continue;
                ctx.DrawLine(new Pen(GetDendriteColor(den.CurrentValue), 2.5), homePt, targetPt);
            }
        }

        // Draw neurons
        int counter = 0;
        foreach (var (neuron, pt) in backup)
            DrawNeuron(ctx, neuron, pt, counter++ % 2 == 0, neuron == _selectedNeuron);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (_nodeMap == null) return;

        var pos = e.GetPosition(this);
        foreach (var (neuron, pt) in _nodeMap)
        {
            double dist = Math.Sqrt(Math.Pow(pos.X - pt.X, 2) + Math.Pow(pos.Y - pt.Y, 2));
            if (dist <= NeuronRadius)
            {
                _selectedNeuron = (_selectedNeuron == neuron) ? null : neuron;
                _forgiveness = 0;
                InvalidateVisual();
                return;
            }
        }

        if (++_forgiveness >= 2)
        {
            _forgiveness = 0;
            _selectedNeuron = null;
            InvalidateVisual();
        }
    }

    private void DrawNeuron(DrawingContext ctx, Neuron neuron, AvPoint pt, bool labelHigh, bool isSelected)
    {
        // Background (covers dendrite ends)
        ctx.DrawEllipse(new SolidColorBrush(Color.FromRgb(255, 228, 181)), null, pt, NeuronRadius, NeuronRadius);

        byte alpha = (byte)Math.Clamp(Math.Abs(neuron.Value) * 255, 0, 255);
        var fillColor = neuron.Value > 0
            ? Color.FromArgb(alpha, 0, 0, 255)
            : Color.FromArgb(alpha, 255, 0, 0);
        var outlinePen = isSelected
            ? new Pen(Brushes.Yellow, 2)
            : new Pen(neuron.Value > 0 ? Brushes.Blue : Brushes.Red, 1);

        ctx.DrawEllipse(new SolidColorBrush(fillColor), outlinePen, pt, NeuronRadius, NeuronRadius);

        // Name label
        var label = neuron.Name.Length > 10 ? neuron.Name[^10..] : neuron.Name;
        var ft = new FormattedText(label, CultureInfo.CurrentCulture, FlowDirection.LeftToRight,
            Typeface.Default, 8, Brushes.LightGray);
        double textY = labelHigh ? pt.Y - NeuronRadius - 12 : pt.Y + NeuronRadius + 2;
        ctx.DrawText(ft, new AvPoint(pt.X - ft.Width / 2, textY));

        // Value label when selected
        if (isSelected)
        {
            var valFt = new FormattedText(neuron.Value.ToString("0.00"), CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight, Typeface.Default, 9, Brushes.Yellow);
            ctx.DrawText(valFt, new AvPoint(pt.X - valFt.Width / 2, pt.Y - 4.5));
        }
    }

    private void BuildNodeMap(NeuralNetworkBrain brain)
    {
        _nodeMap = new Dictionary<Neuron, AvPoint>();
        _downstreamMap = new Dictionary<Neuron, List<(Dendrite, Neuron)>>();
        _lastBuiltSize = Bounds.Size;

        double h = Math.Max(Bounds.Height, 200);
        double w = Math.Max(Bounds.Width, 150);
        double heightSpacer = h / brain.Layers.Count;

        for (int i = brain.Layers.Count - 1; i >= 0; i--)
        {
            var layer = brain.Layers[i];
            double widthSpacer = w / (layer.Neurons.Count + 1);
            for (int j = 0; j < layer.Neurons.Count; j++)
            {
                double x = widthSpacer * (j + 1);
                double y = heightSpacer * i + heightSpacer / 2;
                _nodeMap[layer.Neurons[j]] = new AvPoint(x, y);
            }
        }

        foreach (var layer in brain.Layers)
            foreach (var neuron in layer.Neurons)
                foreach (var den in neuron.UpstreamDendrites)
                {
                    var target = den.TargetNeuron;
                    if (!_downstreamMap.ContainsKey(target))
                        _downstreamMap[target] = new List<(Dendrite, Neuron)>();
                    _downstreamMap[target].Add((den, neuron));
                }
    }

    private static IBrush GetDendriteColor(double value) => value switch
    {
        < -0.5 => new SolidColorBrush(Colors.Purple),
        < 0    => new SolidColorBrush(Colors.Salmon),
        < 0.5  => new SolidColorBrush(Color.FromRgb(100, 100, 100)),
        _      => new SolidColorBrush(Colors.DimGray)
    };
}
