﻿using System;
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
using GlobalRealization.Memory;
using VCPL.Compilator;
using VCPL.Compilator.Contexts;
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
    private string? Input;
    private void Init()
    {
        CompilerCodeConvertor.AddCodeConvertor("CLite", _codeConvertor);
        CompilerCodeConvertor.SplitCode = (string code) => { return code.Split("\r\n"); }; 

        Page.Height = this.Height - 65;
        Page.Width = this.Width;
        
        context.Push(new ContextItem("Page", Page, Modificator.Constant));
        context.Push(new ContextItem("Write", new Function((args) =>
        {
            this.Dispatcher.Invoke(() => {
                Label console = args[0].Get<Label>();
                console.Content = (string)console.Content + args[1].Get()?.ToString();
            });
        }), Modificator.Function));
        context.Push(new ContextItem("WriteLine", new Function((args) =>
        {
            this.Dispatcher.Invoke(() => {
                Label console = args[0].Get<Label>();
                console.Content = (string)console.Content + args[1].Get()?.ToString() + '\n';
            });
        }), Modificator.Function));
        
        context.Push(new ContextItem("ReadLine", new Function(((args) =>
        {
            Label console = args[0].Get<Label>();
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

            if (args.Length == 2) args[1].Set(Input);
        })), Modificator.Function));
        
        context.Push(new ContextItem("Move", new Function((args) =>
        {
            Canvas field = args[0].Get<Canvas>();
            Rectangle rect = args[1].Get<Rectangle>();
            object oacc = args[2].Get<object>();
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
            args[2].Set(acc);
            Thread.Sleep(100);
        }), Modificator.Function));
        context.Push(new ContextItem("SetBackground", new Function((args) =>
        {
            this.Dispatcher.Invoke(
                () => {
                    args[0].Get<Panel>().Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(args[1].Get()), Convert.ToByte(args[2].Get()), Convert.ToByte(args[3].Get())));
                });
        }), Modificator.Function));
        context.Push(new ContextItem("Label", new Function((args) =>
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
                label.Content = args[1].Get();
                args[0].Get<Canvas>().Children.Add(label);
                args[2].Set(label);
            });
        }), Modificator.Function));
        context.Push(new ContextItem("Rect", new Function((args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                Rectangle rect = new Rectangle();
                args[0].Set(rect);
            });
        }), Modificator.Function));
        context.Push(new ContextItem("SetRectWHRGB", new Function((args) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Rectangle rect = args[0].Get<Rectangle>();
                    rect.Width = args[1].Get<int>();
                    rect.Height = args[2].Get<int>();
                    rect.Fill = new SolidColorBrush(Color.FromRgb(Convert.ToByte(args[3].Get()), Convert.ToByte(args[4].Get()), Convert.ToByte(args[5].Get())));
                });
            }), Modificator.Function));
        context.Push(new ContextItem("AddToCanvas", new Function((args) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Canvas canvas = args[0].Get<Canvas>();
                    canvas.Children.Add(args[1].Get<UIElement>());
                });
            }), Modificator.Function));
        context.Push(new ContextItem("SetMargin", new Function((args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                FrameworkElement el = args[0].Get<FrameworkElement>();
                el.Margin = new Thickness(args[1].Get<int>(), args[2].Get<int>(), args[3].Get<int>(), args[4].Get<int>());
            });
        }), Modificator.Function));
        context.Push(new ContextItem("SetOnClick", new Function((args) =>
        {
            Rectangle rectangle = args[0].Get<Rectangle>();
            this.Dispatcher.Invoke(() =>
            {
                rectangle.MouseDown += (sender, eventArgs) =>
                {
                    rectangle.Fill = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(255),
                        (byte)new Random().Next(255), (byte)new Random().Next(255)));
                };
            });
        }), Modificator.Function));
        this.context.Pack();
    }
}