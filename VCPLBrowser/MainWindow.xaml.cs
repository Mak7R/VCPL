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
        private DataContainer startContext;
        private bool runThread = false;
        
        public MainWindow()
        {
            InitializeComponent();

            this.Init();
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
                    foreach (string line in CodeInput.Text.Split("\r\n"))
                    {
                        if (BasicString.IsNoDataString(line)) continue;
                        codeLines.Add(CodeLineConvertor.SyntaxCLite(line));
                    }
                }
                catch(SyntaxException se)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(this, se.Message, "SyntaxException", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    });
                    return;
                }
                
                try
                {
                    main = Compilator.Compilate(codeLines, context);
                }
                catch (CompilationException ce)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show(this, ce.Message, "CompilationException", MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    });
                    return;
                }

                program = new Thread((object? obj) =>
                {
                    try
                    {
                        try
                        {
                            CopyFunction copyMain = main.GetCopyFunction(); 
                            copyMain.Run(startContext, 0, new int[0]); // think about args
                        }
                        catch (RuntimeException re)
                        {
                            this.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show(this, re.Message, "RuntimeException", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                            });
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