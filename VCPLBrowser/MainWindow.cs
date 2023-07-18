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

namespace VCPLBrowser;

public partial class MainWindow
{
    private void Init()
    {
        Page.Height = this.Height - 65;
        Page.Width = this.Width;
        
        context.Push("Page", new Constant(Page));
        context.Push("Write", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => {
                Label console = (Label)container[args[0]].Get();
                console.Content = (string)console.Content + (string)container[args[1]].Get();
            });
            return false;
        }));
        context.Push("WriteLine", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => {
                Label console = (Label)container[args[0]].Get();
                console.Content = (string)console.Content + container[args[1]].Get().ToString() + '\n';
            });
            return false;
        }));
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
            ((IChangeable)container[args[2]]).Set(acc);
            Thread.Sleep(100);
            return false;
        }));
        context.Push("SetBackground", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => {((Panel)container[args[0]].Get()).Background = new SolidColorBrush(Color.FromRgb(Convert.ToByte(container[args[1]].Get()), Convert.ToByte(container[args[2]].Get()), Convert.ToByte(container[args[3]].Get())));});
            return false;
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
            
            return false;
        }));
        context.Push("Rect", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() => { 
                Rectangle rect = new Rectangle();
                container[reference] = new Variable(rect);
            });
            
            return false;
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
                
                return false;
            }));
        context.Push("AddToCanvas", new FunctionInstance((container, reference, args) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Canvas canvas = (Canvas)container[args[0]].Get();
                    canvas.Children.Add((UIElement)container[args[1]].Get());
                });
                
                return false;
            }));
        context.Push("SetMargin", new FunctionInstance((container, reference, args) =>
        {
            this.Dispatcher.Invoke(() =>
            {
                FrameworkElement el = (FrameworkElement)container[args[0]].Get();
                el.Margin = new Thickness((int)container[args[1]].Get(), (int)container[args[2]].Get(), (int)container[args[3]].Get(),
                    (int)container[args[4]].Get());
            });
            return false;
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
            return false;
        }));
        startContext = this.context.Pack();
    }
}