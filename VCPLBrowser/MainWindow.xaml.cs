using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BasicFunctions;
using GlobalRealization;
using Microsoft.Win32;
using VCPL.CodeConvertion;
using VCPL.Compilator;
using VCPL.Еnvironment;
using System.IO;
using VCPL.Exceptions;
using VCPL.Stacks;

namespace VCPLBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FilePath = "";

        private string ChosenSyntax = "CLite"; // create choosing syntax
        private Thread? program;

        private readonly ICodeConvertor _codeConvertor = new CLiteConvertor();
        private readonly ILogger vcplLogger;
        private readonly ReleaseEnvironment releaseEnvironment;
        private readonly DebugEnvironment debugEnvironment;
        private AbstractEnvironment environment;

        public MainWindow()
        {
            InitializeComponent();

            vcplLogger = new VCPLBrowserLogger((message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    LogOutput.Text += message;
                });
            });
            releaseEnvironment = new ReleaseEnvironment(vcplLogger);
            debugEnvironment = new DebugEnvironment(vcplLogger);

            environment = releaseEnvironment;

            releaseEnvironment.SplitCode = (string code) => { return code.Split("\r\n"); };
            releaseEnvironment.envCodeConvertorsContainer.AddCodeConvertor("CLite", _codeConvertor);

            debugEnvironment.SplitCode = (string code) => { return code.Split("\r\n"); };
            debugEnvironment.envCodeConvertorsContainer.AddCodeConvertor("CLite", _codeConvertor);
        }

        private bool ReadFile(string path, out string code)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path))
                {
                    code = sr.ReadToEnd();
                    return true;
                }
            }
            catch (Exception e)
            {
                vcplLogger.Log(e.Message);
                code = string.Empty;
                return false;
            }
        }

        public void InitByFile(string filePath)
        {
            this.FilePath = filePath;

            string dir = System.IO.Path.GetDirectoryName(filePath) ?? string.Empty;
            releaseEnvironment.CurrentDirectory = dir;
            debugEnvironment.CurrentDirectory = dir;

            if (ReadFile(FilePath, out string readedData))
            {
                CodeInput.Text = readedData;
                UpdateTitle();
            }
            else
            {
                MessageBox.Show(this, "File was not open", "File was not open", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
        
        private void SaveToFile()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(FilePath))
                {
                    sw.Write(CodeInput.Text);
                }
                MessageBox.Show(this, "File was saved", "File was succesful saved", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception ex)
            {
                MessageBox.Show(this, "File was not saved", ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            if (this.FilePath != null)
            {
                this.SaveToFile();
            }
            else
            {
                OnSaveAsClick(this, e);
            }
        }
        
        private void OnSaveAsClick(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            if (saveFileDialog.ShowDialog() == true)
            {
                this.FilePath = saveFileDialog.FileName;
                this.SaveToFile();
                UpdateTitle();
            }
        }

        private void OpenFromFile()
        {
            if (ReadFile(FilePath, out string readedData))
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
                this.FilePath = openFileDialog.FileName;
                string dir = System.IO.Path.GetDirectoryName(FilePath) ?? string.Empty;
                releaseEnvironment.CurrentDirectory = dir;
                debugEnvironment.CurrentDirectory = dir;
                this.OpenFromFile();
                UpdateTitle();
            }
        }

        private void OnEnviromentClick(object sender, RoutedEventArgs e) 
        {
            MenuItem s = (MenuItem)sender;
            
            if ((string)s.Header == "Release")
            {
                environment = releaseEnvironment;
            }
            else if ((string)s.Header == "Debug")
            {
                environment = debugEnvironment;
            }
            else
            {
                throw new Exception("Incorrect enviroment");
            }
            ChosenEnviroment.Header = s.Header;
        }

        private void UpdateTitle()
        {
            this.Title = this.FilePath == string.Empty ? "VCPLBrowser" : $"VCPLBrowser ({BasicString.GetNameFromPath(this.FilePath)})";
        }

        private void OnRunStopClick(object sender, RoutedEventArgs e)
        {
            if (program != null) { program.Interrupt(); return; }
            if (environment is DebugEnvironment dbg)
            {
                dbg.Run();
                Stop.IsEnabled = true;
            }
            program = new Thread(() =>
            {
                try
                {
                    ICompilator compilator = new Compilator_IIDL(environment);
                    CompileStack cStack = CreateBasicStack();
                    RuntimeStack rtStack = cStack.Pack();
                    environment.RuntimeStack = rtStack;
                    try
                    {
                        string code = string.Empty;
                        Dispatcher.Invoke(() => code = CodeInput.Text);
                        ElementaryFunction main = compilator.CompilateMain(cStack, code, this.ChosenSyntax, Array.Empty<string>());
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        main.Invoke(Array.Empty<IPointer>());
                    }
                    catch (SyntaxException se)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(this, se.Message, "Syntax Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                    catch (CompilationException ce)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(this, ce.Message, "Compilation Exception", MessageBoxButton.OK, MessageBoxImage.Error);
                        });
                    }
                    catch (RuntimeException re)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            MessageBox.Show(this, re.Message, "Runtime Exception", MessageBoxButton.OK,
                                MessageBoxImage.Error);
                        });
                    }
                }
                catch (ThreadInterruptedException) { }
                Dispatcher.Invoke(() =>
                {
                    Page.Visibility = Visibility.Hidden;
                    Page.Children.Clear();
                    CodeInput.Visibility = Visibility.Visible;
                    RunStop.Header = "Run";
                    program = null;
                    Continue.IsEnabled = false;
                    Stop.IsEnabled = false;
                    GoDown.IsEnabled = false;
                    GoUp.IsEnabled = false;
                    GoThrough.IsEnabled = false;
                });
            })
            {
                IsBackground = true
            };
            program.SetApartmentState(ApartmentState.STA);
            program.Start();

            CodeInput.Visibility = Visibility.Hidden;
            Page.Visibility = Visibility.Visible;
            ((MenuItem)sender).Header = "Stop";
        }

        private List<Key> pressedKeys = new List<Key>();
        public List<Key> PressedKeys
        {
            get { return this.pressedKeys; }
        }
        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
             if (!pressedKeys.Contains(e.Key)) pressedKeys.Add(e.Key);
        }

        private void MainWindow_OnKeyUp(object sender, KeyEventArgs e)
        {
            pressedKeys.Remove(e.Key);
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            if (environment is DebugEnvironment dbg)
            {
                dbg.Run();
                Stop.IsEnabled = true;
                Continue.IsEnabled = false;
                GoThrough.IsEnabled = false;
                GoUp.IsEnabled = false;
                GoDown.IsEnabled = false;
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (environment is DebugEnvironment dbg)
            {
                dbg.Stop();
                Stop.IsEnabled = false;
                Continue.IsEnabled = true;
                GoThrough.IsEnabled = true;
                GoUp.IsEnabled = true;
                GoDown.IsEnabled = true;
            }
        }

        private void GoUp_Click(object sender, RoutedEventArgs e)
        {
            if (environment is DebugEnvironment dbg)
            {
                dbg.GoUp();
            }
        }

        private void GoDown_Click(object sender, RoutedEventArgs e)
        {
            if (environment is DebugEnvironment dbg)
            {
                dbg.GoDown();
            }
        }

        private void GoThrough_Click(object sender, RoutedEventArgs e)
        {
            if (environment is DebugEnvironment dbg)
            {
                dbg.GoThrough();
            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            LogOutput.Text = "";
        }

        private bool isShow = true;
        private void ShowHideConsole_Click(object sender, RoutedEventArgs e)
        {
            if (isShow)
            {
                Grid.SetRowSpan(Page, 2);
                Grid.SetRowSpan(CodeInput, 2);
                LogOutput.Visibility = Visibility.Hidden;
                ((MenuItem)sender).Header = "Show";
            }
            else
            {
                Grid.SetRowSpan(Page, 1);
                Grid.SetRowSpan(CodeInput, 1);
                LogOutput.Visibility = Visibility.Visible;
                ((MenuItem)sender).Header = "Hide";
            }
            isShow = !isShow;
        }
    }
}