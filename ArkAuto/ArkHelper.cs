using System;
using System.Runtime.InteropServices;
using System.Threading;


namespace ArkAuto;

public class ArkHelper
{
    public struct POINT
    {
        public int X;
        public int Y;
    }
    [DllImport("user32.dll")]
    public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int Msg, 
        IntPtr wParam, IntPtr lParam);
    [DllImport("user32.dll")]
    public static extern bool PostMessage(IntPtr  hWnd, uint Msg, IntPtr wParam, IntPtr  lParam);
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    
    private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

    public static int AUTO_LEFT = 0;
    public static int AUTO_RIGHT = 1;
    public static int AUTO_E = 2;
    public static int AUTO_RUN = 3;
    public static string CLASS_NAME = "UnrealWindow";
    public static  string WINDOWS_NAME = "ARK: Survival Evolved";
    private static volatile bool isRunning = true;
    public static IntPtr MakeLParamFromXY(int x, int y) => (IntPtr) (y << 16 | x);
    private Thread[] _threads;
    private static EventWaitHandle[] _waitHandles = new[]
    {
        new ManualResetEvent(initialState: true),
        new ManualResetEvent(initialState: true),
        new ManualResetEvent(initialState: true),
    };

    private GameOverlay _game;
    public ArkHelper()
    {
        _threads = new[]
        {
            new Thread(StartLeft),
            new Thread(StartRight),
            new Thread(StartEating)
        };
        for (int i = 0; i < _threads.Length; i++)
        {
            _waitHandles[i].Reset();
            _threads[i].Start();
        }

        _game = new GameOverlay();
        _game.Start();
    }

    public void Dispose()
    {
        isRunning = false;
        _game.Dispose();
        for (int i = 0; i < _threads.Length; i++)
        {
            _waitHandles[i].Set();
        }
    }
    private void StartRight()
    {
        while (isRunning)
        {
            IntPtr game = FindWindow(CLASS_NAME, WINDOWS_NAME);
            POINT PT;
            PT.X = 0;
            PT.Y = 0;
            ScreenToClient(game, ref PT);
            SendRightClick(game, PT);
            Thread.Sleep(1000);
            _waitHandles[AUTO_RIGHT].WaitOne(); 
        }
    }
    private void StartEating()
    {
        while (isRunning)
        {
            IntPtr game = FindWindow(CLASS_NAME, WINDOWS_NAME);
            SendKey(game);
            Thread.Sleep(50);
            _waitHandles[AUTO_E].WaitOne(); 
        }
    }
    private void StartLeft()
    {
        while (isRunning)
        {
            IntPtr game = FindWindow(CLASS_NAME, WINDOWS_NAME);
            POINT PT;
            PT.X = 0;
            PT.Y = 0;
            ScreenToClient(game, ref PT);
            SendLeftClick(game, PT);
            Thread.Sleep(1000);
            _waitHandles[AUTO_LEFT].WaitOne(); 
        }
    }

    public static void SendKeyBoardDown(IntPtr handle, eVirtualKey key)
    {
        PostMessage(handle, 6, new IntPtr(1), new IntPtr(0));
        PostMessage(handle, 256, new IntPtr((int) key), new IntPtr(0));
    }
    public static void SendKeyBoardUp(IntPtr handle, eVirtualKey key)
    {
        PostMessage(handle, 6, new IntPtr(1), new IntPtr(0));
        PostMessage(handle, 257, new IntPtr((int) key), new IntPtr(0));
    }
    
    public void Run(bool isRunning)
    {
        _game.updateStatus(AUTO_RUN, isRunning);
        IntPtr game = FindWindow(CLASS_NAME, WINDOWS_NAME);
        if (!isRunning)
        {
            SendKeyBoardUp(game, eVirtualKey.VK_LSHIFT);
            SendKeyBoardUp(game, eVirtualKey.VK_W);
        }
        else
        {
            SendKeyBoardDown(game, eVirtualKey.VK_LSHIFT);
            SendKeyBoardDown(game, eVirtualKey.VK_W);
        }
        
    }
    private void SendRightClick(IntPtr handlePtr, POINT point )
    {
        IntPtr lParam =  MakeLParamFromXY(point.X, point.Y);
        PostMessage(handlePtr, 6, new IntPtr(1), lParam);
        PostMessage(handlePtr, 516, new IntPtr(1), lParam);
        PostMessage(handlePtr, 517, new IntPtr(0), lParam);
    }

    private void SendKey(IntPtr handlePtr)
    {
        PostMessage(handlePtr, 0x0104, new IntPtr(0x45) , new IntPtr(0));
    }
    private void SendLeftClick(IntPtr handlePtr, POINT point)
    {
        IntPtr lParam =  MakeLParamFromXY(point.X, point.Y);
        PostMessage(handlePtr, 6, new IntPtr(1), lParam);
        PostMessage(handlePtr, 513, new IntPtr(1), lParam);
        PostMessage(handlePtr, 514, new IntPtr(0), lParam);
    }
    public void RightClick(bool isEnable)
    {
        _game.updateStatus(AUTO_RIGHT, isEnable);
        if (isEnable)
        {
            _waitHandles[AUTO_RIGHT].Set();
        }
        else
        {
            _waitHandles[AUTO_RIGHT].Reset();
        }
       
    }

    public void LeftClick(bool isEnable)
    {
        _game.updateStatus(AUTO_LEFT, isEnable);
        if (isEnable)
        {
            _waitHandles[AUTO_LEFT].Set();
        }
        else
        {
            _waitHandles[AUTO_LEFT].Reset();
        }
    }

    public void Eating(bool isEnable)
    {
        _game.updateStatus(AUTO_E, isEnable);
            if (isEnable)
            {
                _waitHandles[AUTO_E].Set();
            }
            else
            {
                _waitHandles[AUTO_E].Reset();
            }
    }
}