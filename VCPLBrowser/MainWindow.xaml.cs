﻿using System;
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
using BasicFunctions;
using GlobalRealization;
using Microsoft.Win32;

using VCPL;

namespace VCPLBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string filePath = null;
        private Function main = null;
        private Thread program = null;
        private VCPL.Context context = new Context(new TempContainer(BasicContext.BasicData), BasicContext.ElementaryFunctions);
        private bool runThread = false;
        
        public MainWindow()
        {
            InitializeComponent();

            Page.Height = this.Height - 65;
            Page.Width = this.Width;
            
            context.dataContext.Push("Page", Page);
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
        }

        

        private void SaveToFile()
        {
            bool ok = FileCodeEditor.WriteCodeS(filePath, CodeInput.Text);
            if (ok)
            {
                MessageBox.Show(this, "File was saved succesful", "File was saved succesful", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(this, "File was not saved", "File was not saved", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        
        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (this.filePath != null)
            {
                this.SaveToFile();
            }
            else
            {
                OnSaveClick(this, null);
            }
        }
        
        private void OnSaveAsClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                this.filePath = saveFileDialog.FileName;
                this.SaveToFile();
            }
            UpdateTitle();
        }

        private void OpenFromFile()
        {
            if (FileCodeEditor.ReadCodeS(filePath, out string readedData))
            {
                CodeInput.Text = readedData;
                MessageBox.Show(this, "File was open succesful", "File was open succesful", MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(this, "File was not open", "File was not open", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        
        private void OnOpenClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            
            if (openFileDialog.ShowDialog() == true)
            {
                this.filePath = openFileDialog.FileName;
                this.OpenFromFile();
            }
            
            UpdateTitle();
        }

        private void UpdateTitle()
        {
            this.Title = this.filePath == string.Empty ? "VCPLBrowser" : $"VCPLBrowser ({BasicString.GetNameFromPath(this.filePath)})";
        }

        private bool isRun = false;
        private void OnRunStopClick(object sender, RoutedEventArgs e)
        {
            if (isRun) program.Interrupt();
            else
            {
                /// compilation should goes in new thread

                List<CodeLine> codeLines = new List<CodeLine>();
                try
                {
                    foreach (string line in CodeInput.Text.Split('\n'))
                    {
                        if (BasicString.IsNoDataString(line)) continue;
                        codeLines.Add(CodeLineConvertor.SyntaxCLite(line));
                    }
                }
                catch(SyntaxException se)
                {
                    Debug.WriteLine(se.Message);
                    return;
                }

                try
                {
                    main = Compilator.Compilate(codeLines, context);
                }
                catch (CompilationException ce)
                {
                    Debug.WriteLine(ce.Message);
                    return;
                }

                program = new Thread((object? obj) =>
                {
                    try
                    {
                        try
                        {
                            CopyFunction copyMain = main.GetCopyFunction(); ///
                            copyMain.Run(null, 0, new int[0]); // think about args
                        }
                        catch (RuntimeException re)
                        {
                            Debug.WriteLine(re.Message);
                        }
                        ((MainWindow)obj).Dispatcher.Invoke(() =>
                        {
                            ((MainWindow)obj).Page.Visibility = Visibility.Hidden;
                            ((MainWindow)obj).Page.Children.Clear(); /// ???
                            ((MainWindow)obj).CodeInput.Visibility = Visibility.Visible;
                            ((MainWindow)obj).RunStop.Header = "Run";
                            ((MainWindow)obj).isRun = false;
                        });
                    }
                    catch (ThreadInterruptedException e)
                    {
                        ((MainWindow)obj).Dispatcher.Invoke(() =>
                        {
                            ((MainWindow)obj).Page.Visibility = Visibility.Hidden;
                            ((MainWindow)obj).Page.Children.Clear(); /// ???
                            ((MainWindow)obj).CodeInput.Visibility = Visibility.Visible;
                            ((MainWindow)obj).RunStop.Header = "Run";
                            ((MainWindow)obj).isRun = false;
                        });
                    }
                });
                program.IsBackground = true;
                program.SetApartmentState(ApartmentState.STA);
                
                program.Start(this);
                // message compilation successful
                
                CodeInput.Visibility = Visibility.Hidden;
                Page.Visibility = Visibility.Visible;
                ((MenuItem)sender).Header = "Stop";
                isRun = true;
            }
        }

        private void RunWithoutCompilation_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.main == null)
            {
                MessageBox.Show(this, "Main was not compilated yet", "Main was not compilated yet", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            else
            {
                program = new Thread((object? obj) =>
                {
                    try
                    {
                        CopyFunction copyMain = main.GetCopyFunction();
                        try
                        {
                            copyMain.Run(null, 0, new int[0]); // think about args
                        }
                        catch (RuntimeException re)
                        {
                            Debug.WriteLine(re.Message);
                        }
                        ((MainWindow)obj).Dispatcher.Invoke(() =>
                        {
                            ((MainWindow)obj).Page.Visibility = Visibility.Hidden;
                            ((MainWindow)obj).Page.Children.Clear(); /// ???
                            ((MainWindow)obj).CodeInput.Visibility = Visibility.Visible;
                            ((MainWindow)obj).RunStop.Header = "Run";
                            ((MainWindow)obj).isRun = false;
                        });
                    }
                    catch (ThreadInterruptedException e)
                    {
                        ((MainWindow)obj).Dispatcher.Invoke(() =>
                        {
                            ((MainWindow)obj).Page.Visibility = Visibility.Hidden;
                            ((MainWindow)obj).Page.Children.Clear(); /// ???
                            ((MainWindow)obj).CodeInput.Visibility = Visibility.Visible;
                            ((MainWindow)obj).RunStop.Header = "Run";
                            ((MainWindow)obj).isRun = false;
                        });
                    }
                });
                program.IsBackground = true;
                program.SetApartmentState(ApartmentState.STA);
                
                program.Start(this);
                // message compilation successful
                
                CodeInput.Visibility = Visibility.Hidden;
                Page.Visibility = Visibility.Visible;
                ((MenuItem)sender).Header = "Stop";
                isRun = true;
            }
        }

        private List<Key> pressedKeys = new List<Key>();
        public List<Key> PressedKeys
        {
            get { return this.pressedKeys; }
        }
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
             if (!pressedKeys.Contains(e.Key))pressedKeys.Add(e.Key);
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.Key);
        }
    }
}