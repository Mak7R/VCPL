using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using BasicFunctions;
using GlobalRealization;

namespace VCPLBrowser;

static class Tools
{
    public enum MapType : uint
    {
        MAPVK_VK_TO_VSC = 0x0,
        MAPVK_VSC_TO_VK = 0x1,
        MAPVK_VK_TO_CHAR = 0x2,
        MAPVK_VSC_TO_VK_EX = 0x3,
    }

    [DllImport("user32.dll")]
    public static extern int ToUnicode(
        uint wVirtKey,
        uint wScanCode,
        byte[] lpKeyState,
        [Out, MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 4)] 
        StringBuilder pwszBuff,
        int cchBuff,
        uint wFlags);

    [DllImport("user32.dll")]
    public static extern bool GetKeyboardState(byte[] lpKeyState);

    [DllImport("user32.dll")]
    public static extern uint MapVirtualKey(uint uCode, MapType uMapType);

    public static char GetCharFromKey(Key key)
    {
        char ch = ' ';

        int virtualKey = KeyInterop.VirtualKeyFromKey(key);
        byte[] keyboardState = new byte[256];
        GetKeyboardState(keyboardState);

        uint scanCode = MapVirtualKey((uint)virtualKey, MapType.MAPVK_VK_TO_VSC);
        StringBuilder stringBuilder = new StringBuilder(2);

        int result = ToUnicode((uint)virtualKey, scanCode, keyboardState, stringBuilder, stringBuilder.Capacity, 0);
        switch (result)
        {
            case -1: 
                break;
            case 0: 
                break;
            case 1:
            {
                ch = stringBuilder[0];
                break;
            }
            default:
            {
                ch = stringBuilder[0];
                break;
            }
        }
        return ch;
    }
}

public partial class MainWindow
{
    private bool isEnter = false;
    private string Input;
    private void Init()
    {
        Page.Height = this.Height - 65;
        Page.Width = this.Width;
        
        context.Push("Page", new Constant(Page));
        context.Push("Write", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => {
                Label console = (Label)container[args[0]].Get();
                console.Content = (string)console.Content + container[args[1]].Get().ToString();
            });
        }));
        context.Push("WriteLine", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => {
                Label console = (Label)container[args[0]].Get();
                console.Content = (string)console.Content + container[args[1]].Get().ToString() + '\n';
            });
        }));
        
        context.Push("ReadLine", new FunctionInstance(((context, result, args) =>
        {
            Label console = (Label)context[args[0]].Get();
            this.Input = "";
            System.Windows.Input.KeyEventHandler Event = (object sender, KeyEventArgs eventArgs) =>
            {
                if (eventArgs.Key == Key.Enter) { this.isEnter = true; return; }

                if (eventArgs.Key == Key.Back)
                {
                    if (this.Input != string.Empty && this.Input.Length > 0)
                    {
                        this.Input = this.Input.Substring(0, this.Input.Length - 1);
                        console.Content = ((string)console.Content).Substring(0, ((string)console.Content).Length-1);
                    }
                    return;
                }
                char key = Tools.GetCharFromKey(eventArgs.Key);
                console.Content = (string)console.Content + key;
                this.Input += key;
            };
            this.KeyDown += Event;
            while (!isEnter) ;
            this.Dispatcher.Invoke(() => {
                console.Content = (string)console.Content + '\n';
            });
            this.KeyDown -= Event;
            this.isEnter = false;
            if (result != Pointer.NULL) if (context[result] is IChangeable changeable) changeable.Set(Input);
        })));
        
        context.Push("Move", new FunctionInstance((container, reference, args) =>
        {
            Canvas field = (Canvas)container[args[0]].Get();
            Rectangle rect = (Rectangle)container[args[1]].Get();
            object oacc = container[args[2]].Get();
            double acc = oacc is int ? (double)(int)oacc : (double)oacc;
            foreach (var key in this.pressedKeys)
            {
                switch (key)
                {
                    case Key.A:
                        this.Dispatcher.Invoke(() =>
                        {
                            rect.Margin = new Thickness(
                                rect.Margin.Left - 20,
                                rect.Margin.Top,
                                rect.Margin.Right,
                                rect.Margin.Bottom);
                        });
                        break;
                    case Key.D:
                        this.Dispatcher.Invoke(() =>
                        {
                            rect.Margin = new Thickness(rect.Margin.Left + 20, rect.Margin.Top,
                                rect.Margin.Right,
                                rect.Margin.Bottom);
                            
                        });
                        break;
                    case Key.Space:
                        this.Dispatcher.Invoke(() =>
                        {
                            if (rect.Margin.Top + rect.Height < field.Height)
                            {
                                return;
                            }
                            else
                            {
                                acc -= 50;
                            }
                        });
                        break;
                    case Key.Escape:
                        return;
                    default: break;
                }
            }

            this.Dispatcher.Invoke(() => { 
                rect.Margin = new Thickness(rect.Margin.Left, rect.Margin.Top + acc, rect.Margin.Right,
                    rect.Margin.Bottom);
                    
                if (rect.Margin.Top + rect.Height >= field.Height)
                {
                    rect.Margin = new Thickness(rect.Margin.Left, field.Height - rect.Height, rect.Margin.Right,
                        rect.Margin.Bottom);
                    acc = 0;
                }
                else
                {
                    acc += 10;
                }
                
            });
            ((IChangeable)container[args[2]]).Set(acc);
            Thread.Sleep(100);
        }));
        context.Push("SetBackground", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(
                () => {
                    ((Panel)container[args[0]].Get()).Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(container[args[1]].Get()), Convert.ToByte(container[args[2]].Get()), Convert.ToByte(container[args[3]].Get())));
                });
        }));
        context.Push("Label", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                Label label = new Label();
                label.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                label.FontSize = 16;
                label.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                label.Width = 600;
                label.Height = 400;
                label.Margin = new Thickness(20, 20, 0, 0);
                label.Content = container[args[1]].Get();
                ((Canvas)container[args[0]].Get()).Children.Add(label);
                container[reference] = new Variable(label);
            });
        }));
        context.Push("Rect", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => { 
                Rectangle rect = new Rectangle();
                container[reference] = new Variable(rect);
            });
        }));
        context.Push("SetRectWHRGB", new FunctionInstance((container, reference, args) =>
            {
                this.Dispatcher.Invoke(() => 
                { 
                    Rectangle rect = (Rectangle)container[args[0]].Get();
                    rect.Width = (int)container[args[1]].Get();
                    rect.Height = (int)container[args[2]].Get();
                    rect.Fill = new SolidColorBrush(Color.FromRgb(Convert.ToByte(container[args[3]].Get()), Convert.ToByte(container[args[4]].Get()), Convert.ToByte(container[args[5]].Get())));
                });
            }));
        context.Push("AddToCanvas", new FunctionInstance((container, reference, args) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Canvas canvas = (Canvas)container[args[0]].Get();
                    canvas.Children.Add((UIElement)container[args[1]].Get());
                });
            }));
        context.Push("SetMargin", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                FrameworkElement el = (FrameworkElement)container[args[0]].Get();
                el.Margin = new Thickness((int)container[args[1]].Get(), (int)container[args[2]].Get(), (int)container[args[3]].Get(),
                    (int)container[args[4]].Get());
            });
        }));
        context.Push("SetOnClick", new FunctionInstance((container, reference, args) =>
        {
            Rectangle rectangle = (Rectangle)container[args[0]].Get();
            this.Dispatcher.Invoke(() =>
            {
                rectangle.MouseDown += (sender, eventArgs) =>
                {
                    rectangle.Fill = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(255),
                        (byte)new Random().Next(255), (byte)new Random().Next(255)));
                };
            });
        }));
        startContext = this.context.Pack();
    }
}