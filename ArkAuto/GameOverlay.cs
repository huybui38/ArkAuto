using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Overlay.NET;
using Process.NET;
using Process.NET.Memory;


namespace ArkAuto;

public class GameOverlay
{
    private ProcessSharp _processSharp;
    private OverlayPlugin _overlayPlugin;
    List<OverlayItem> items = new List<OverlayItem>();
    private Thread _thread;
    private bool isRunning = true;
    public GameOverlay()
    {
        _thread = new Thread(StartOverlay);
    }

    public void Dispose()
    {
        isRunning = false;
    }

    private void StartOverlay()
    {
        items.Add(new OverlayItem(){ Index = ArkHelper.AUTO_RIGHT, Label = "Auto Right Click: ", Status = false});
        items.Add(new OverlayItem(){ Index = ArkHelper.AUTO_LEFT, Label = "Auto Left Click: ", Status = false});
        items.Add(new OverlayItem(){ Index = ArkHelper.AUTO_E, Label = "Auto E: ", Status = false});
        items.Add(new OverlayItem(){ Index = ArkHelper.AUTO_RUN, Label = "Auto Run: ", Status = false});
        var process = System.Diagnostics.Process.GetProcessesByName("ShooterGame").FirstOrDefault();
        if (process == null) {
            return;
        }
        _processSharp = new ProcessSharp(process, MemoryType.Remote);
        _overlayPlugin = new OverlayPlugin();
        var d3DOverlay =  _overlayPlugin;
        d3DOverlay.Settings.Current.UpdateRate = 1000 / 20;
        _overlayPlugin.Initialize(_processSharp.WindowFactory.MainWindow);
        _overlayPlugin.InitOverlayItem(items);
        _overlayPlugin.Enable();
        
        while (isRunning) {
            _overlayPlugin.Update();
        }
    }

    public void Start()
    {
        _thread.Start();
    }

    public void updateStatus(int type, bool isEnabled)
    {
        if (_overlayPlugin != null)
        {
            _overlayPlugin.UpdateStatusOverlayItem(type, isEnabled);
        }
    }
}