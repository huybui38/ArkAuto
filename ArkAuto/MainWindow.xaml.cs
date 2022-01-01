using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ArkAuto
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private GlobalHook hook = null;
        private ArkHelper _arkHelper = null;
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool isAutoRun = false;
        private bool isAutoLeft = false;
        private bool isAutoRight = false;
        private bool isEating = false;
        
        private void Window_OnLoaded(object sender, RoutedEventArgs e)
        {
            hook = new GlobalHook(HandleKeyPress);
            _arkHelper = new ArkHelper();
            
        }


        private void HandleKeyPress(eKeyCode code)
        {
            if (code == eKeyCode.F1)
            {
                isAutoRun = !isAutoRun;
                _arkHelper.Run(isAutoRun);
            }

            if (code == eKeyCode.F2)
            {
                isAutoLeft = !isAutoLeft;
                _arkHelper.LeftClick(isAutoLeft);
            }

            if (code == eKeyCode.F3)
            {
                isAutoRight = !isAutoRight;
                _arkHelper.RightClick(isAutoRight);
            }

            if (code == eKeyCode.F4)
            {
                isEating = !isEating;
                _arkHelper.Eating(isEating);
            }
        }
        
        private void Window_OnClosing(object? sender, CancelEventArgs e)
        {
            if (hook != null)
            {
                hook.Unhook();
            }
            _arkHelper.Dispose();
        }
    }
    
}