using System;
using System.Collections.Generic;
using System.Runtime.Loader;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BasicFunctions;
using GlobalRealization;
using Microsoft.Win32;
using VCPL;
using VCPL.CodeConvertion;
using VCPL.Compilator;
using VCPL.Еnvironment;
using System.Collections;
using System.DirectoryServices;
using VCPL.Compilator.Stacks;
using System.IO;
using System.Diagnostics;
using System.Windows.Shapes;
using VCPL.Exceptions;

namespace VCPLBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FilePath = "";

        private ElementaryFunction? main;
        private Thread? program;

        private readonly ICodeConvertor _codeConvertor = new CLiteConvertor();
        private readonly ILogger vcplLogger;
        private readonly ReleaseEnvironment releaseEnvironment;
        private readonly DebugEnvironment debugEnvironment;
        private AbstractEnvironment enviriment;

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

            enviriment = releaseEnvironment;

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
                Debug.WriteLine(e.Message);
                code = string.Empty;
                return false;
            }
        }

        public void InitByFile(string filePath)
        {
            this.FilePath = filePath;
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
                this.OpenFromFile();
                UpdateTitle();
            }
        }

        private void OnEnviromentClick(object sender, RoutedEventArgs e) // can be better
        {
            MenuItem s = (MenuItem)sender;
            
            if ((string)s.Header == "Release")
            {
                enviriment = releaseEnvironment;
            }
            else if ((string)s.Header == "Debug")
            {
                enviriment = debugEnvironment;
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
            program = new Thread(() =>
            {
                try
                {
                    ICompilator compilator = new Compilator_IIDL(enviriment);
                    CompileStack cStack = CreateBasicStack();
                    RuntimeStack rtStack = cStack.Pack();
                    enviriment.RuntimeStack = rtStack;
                    try
                    {
                        string code = string.Empty;
                        Dispatcher.Invoke(() => code = CodeInput.Text);
                        main = compilator.CompilateMain(cStack, code, "CLite", Array.Empty<string>());
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        debugEnvironment.Run();
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

        private void RunWithoutCompilation_OnClick(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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
            if (enviriment is DebugEnvironment dbg)
            {
                dbg.Run();
            }
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            if (enviriment is DebugEnvironment dbg)
            {
                dbg.Stop();
            }
        }

        private void GoUp_Click(object sender, RoutedEventArgs e)
        {
            if (enviriment is DebugEnvironment dbg)
            {
                dbg.GoUp();
            }
        }

        private void GoDown_Click(object sender, RoutedEventArgs e)
        {
            if (enviriment is DebugEnvironment dbg)
            {
                dbg.GoDown();
            }
        }

        private void GoThrough_Click(object sender, RoutedEventArgs e)
        {
            if (enviriment is DebugEnvironment dbg)
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