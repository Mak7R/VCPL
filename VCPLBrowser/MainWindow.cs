using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Microsoft.Win32;

namespace VCPLBrowser;

public partial class MainWindow
{
    private void Init()
    {

        Page.Height = this.Height - 65;
        Page.Width = this.Width;
        
        context.dataContext.Push("Page", Page);
        context.functionsContext.Add("Write", (container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => {
                Label console = (Label)container[args[0]];
                console.Content = (string)console.Content + (string)container[args[1]];
            });
            return false;
        });
        context.functionsContext.Add("WriteLine", (container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => {
                Label console = (Label)container[args[0]];
                console.Content = (string)console.Content + container[args[1]].ToString() + '\n';
            });
            return false;
        });
        context.functionsContext.Add("Console", (container, reference, args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                Label console = new Label();
                console.FontSize = 16;
                console.Margin = new Thickness(0,0,0,0);
                console.Width = Page.Width;
                console.Height = Page.Height;
                ((Canvas)container[args[0]]).Children.Add(console);
                container[reference] = console;
            });
            return false;
        });
        context.functionsContext.Add("Move", (container, reference, args) =>
        {
            Canvas field = (Canvas)container[args[0]];
            Rectangle rect = (Rectangle)container[args[1]];
            double acc = 0;
            while (true)
            {
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
                            return false;
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
                
                Thread.Sleep(200);
            }
        });
        context.functionsContext.Add("SetBackground", (container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => {((Panel)container[args[0]]).Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(container[args[1]]), Convert.ToByte(container[args[2]]), Convert.ToByte(container[args[3]])));});
            return false;
        });
        context.functionsContext.Add("Label", (container, reference, args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                Label label = new Label();
                label.FontSize = 16;
                label.Foreground = new SolidColorBrush(Color.FromRgb(0, 0, 0));
                label.Width = 200;
                label.Height = 100;
                label.Margin = new Thickness(20, 20, 0, 0);
                label.Content = container[args[1]];
                ((Canvas)container[args[0]]).Children.Add(label);
                container[reference] = label;
            });
            
            return false;
        });
        context.functionsContext.Add("Rect", (container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => { 
                Rectangle rect = new Rectangle();
                container[reference] = rect;
            });
            
            return false;
        });
        context.functionsContext.Add("SetRectWHRGB", (container, reference, args) =>
            {
                this.Dispatcher.Invoke(() => 
                { 
                    Rectangle rect = (Rectangle)container[args[0]];
                    rect.Width = (int)container[args[1]];
                    rect.Height = (int)container[args[2]];
                    rect.Fill = new SolidColorBrush(Color.FromRgb(Convert.ToByte(container[args[3]]), Convert.ToByte(container[args[4]]), Convert.ToByte(container[args[5]])));
                });
                
                return false;
            });
        context.functionsContext.Add("AddToCanvas", (container, reference, args) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Canvas canvas = (Canvas)container[args[0]];
                    canvas.Children.Add((UIElement)container[args[1]]);
                });
                
                return false;
            });
        context.functionsContext.Add("SetMargin", (container, reference, args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                FrameworkElement el = (FrameworkElement)container[args[0]];
                el.Margin = new Thickness((int)container[args[1]], (int)container[args[2]], (int)container[args[3]],
                    (int)container[args[4]]);
            });
            return false;
        });
        context.functionsContext.Add("SetOnClick", (container, reference, args) =>
        {
            Rectangle rectangle = (Rectangle)container[args[0]];
            this.Dispatcher.Invoke(() =>
            {
                rectangle.MouseDown += (sender, eventArgs) =>
                {
                    rectangle.Fill = new SolidColorBrush(Color.FromRgb((byte)new Random().Next(255),
                        (byte)new Random().Next(255), (byte)new Random().Next(255)));
                };
            });
            return false;
        });
        
        
        startContext = this.context.dataContext.Pack();
    }
}