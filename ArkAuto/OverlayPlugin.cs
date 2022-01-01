using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Overlay.NET.Common;
using Overlay.NET.Directx;
using Process.NET.Windows;

namespace ArkAuto;

[RegisterPlugin("ArkOverLay", "", "ArkOverLay", "0.0",
    "")]
public class OverlayPlugin: DirectXOverlayPlugin {
 
    public class DemoOverlaySettings {
        public int UpdateRate { get; set; }

        public string Author { get; set; }
        public string Description { get; set; }
        public string Identifier { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
    }
        
    private readonly TickEngine _tickEngine = new TickEngine();
    public readonly ISettings<DemoOverlaySettings> Settings = new SerializableSettings<DemoOverlaySettings>();
    private int _font;
    private int _redBrush;
    private int _greenBrush;
    private float _rotation;
    private Stopwatch _watch;
    private static List<OverlayItem> overlayItems = new List<OverlayItem>();
    private  object _lock = new ();
    public void UpdateStatusOverlayItem(int index, bool status)
    {
        lock (_lock)
        {
            for (int i = 0; i < overlayItems.Count; i++)
            {
                if (overlayItems[i].Index == index)
                {
                    overlayItems[i].Status = status;
                    return;
                }
            }
            
        }
    }

    public void InitOverlayItem(List<OverlayItem> items)
    {
        lock (_lock)
        {
            overlayItems.Clear();
            overlayItems.AddRange(items);
        }
    }
    public override void Initialize(IWindow targetWindow) {
        // Set target window by calling the base method
        base.Initialize(targetWindow);
            
        // For demo, show how to use settings
        var current = Settings.Current;
        var type = GetType();

        if (current.UpdateRate == 0)
            current.UpdateRate = 1000 / 60;

        current.Author = GetAuthor(type);
        current.Description = GetDescription(type);
        current.Identifier = GetIdentifier(type);
        current.Name = GetName(type);
        current.Version = GetVersion(type);

        // File is made from above info
        Settings.Save();
        Settings.Load();
          
        OverlayWindow = new DirectXOverlayWindow(targetWindow.Handle, false);
        _watch = Stopwatch.StartNew();

        _redBrush = OverlayWindow.Graphics.CreateBrush(0x7FFF0000);
            
        _greenBrush = OverlayWindow.Graphics.CreateBrush(Color.Green.ToArgb());

        _font = OverlayWindow.Graphics.CreateFont("Arial", 12);

        _rotation = 0.0f;
        // Set up update interval and register events for the tick engine.

        _tickEngine.PreTick += OnPreTick;
        _tickEngine.Tick += OnTick;
    }

    private void OnTick(object sender, EventArgs e) {
        if (!OverlayWindow.IsVisible) {
            return;
        }
        OverlayWindow.Update();
        InternalRender();
    }

    private void OnPreTick(object sender, EventArgs e) {
        var targetWindowIsActivated = TargetWindow.IsActivated;
        if (!targetWindowIsActivated && OverlayWindow.IsVisible) {
            _watch.Stop();
            ClearScreen();
            OverlayWindow.Hide();
        }
        else if (targetWindowIsActivated && !OverlayWindow.IsVisible) {
            OverlayWindow.Show();
        }
    }

    // ReSharper disable once RedundantOverriddenMember
    public override void Enable() {
        _tickEngine.Interval = Settings.Current.UpdateRate.Milliseconds();
        _tickEngine.IsTicking = true;
        base.Enable();
    }

    // ReSharper disable once RedundantOverriddenMember
    public override void Disable() {
        _tickEngine.IsTicking = false;
        base.Disable();
    }

    public override void Update() => _tickEngine.Pulse();

    private void DrawText(string label ,bool isEnabled, int index)
    {
        string status = isEnabled ? "Enabled" : "Disabled";
        OverlayWindow.Graphics.DrawText(label, _font, _redBrush, 50, 40 + (20 * index));
        OverlayWindow.Graphics.DrawText(status, _font, isEnabled? _greenBrush : _redBrush, 160, 40 + (20 * index));

    }
    protected void InternalRender() {
        if (!_watch.IsRunning) {
            _watch.Start();
        }

        OverlayWindow.Graphics.BeginScene();
        OverlayWindow.Graphics.ClearScene();
        lock (_lock)
        {
            for (int i = 0; i < overlayItems.Count; i++)
            {
                    
                DrawText(overlayItems[i].Label, overlayItems[i].Status, overlayItems[i].Index);
            }
        }
         
           
        _rotation += 0.03f; //related to speed

        if (_rotation > 50.0f) //size of the swastika
        {
            _rotation = -50.0f;
        }

        if (_watch.ElapsedMilliseconds > 1000) {
            _watch.Restart();
        }

        else {
        }

        OverlayWindow.Graphics.EndScene();
    }

    public override void Dispose() {
        OverlayWindow.Dispose();
        base.Dispose();
    }

    private void ClearScreen() {
        OverlayWindow.Graphics.BeginScene();
        OverlayWindow.Graphics.ClearScene();
        OverlayWindow.Graphics.EndScene();
    }
}