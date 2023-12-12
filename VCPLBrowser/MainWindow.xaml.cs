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
using FileController;
using VCPL;
using VCPL.CodeConvertion;
using VCPL.Compilator;

namespace VCPLBrowser
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string FilePath = null;
        private Function main = null;
        private Thread program = null;
        private Context context = new Context(null, BasicContext.BasicContextList).NewContext();
        private RuntimeContext startContext;
        private bool runThread = false;
        private AssemblyLoadContext assemblyLoadContext = new AssemblyLoadContext("AssemblyContext", true);
        private ICodeConvertor _codeConvertor = new CLiteConvertor();
        
        public MainWindow()
        {
            InitializeComponent();

            this.Init();
        }

        public MainWindow(string filePath)
        {
            InitializeComponent();
            this.FilePath = filePath;
            if (FileCodeEditor.ReadCodeS(FilePath, out string readedData))
            {
                CodeInput.Text = readedData;
                UpdateTitle();
            }
            else
            {
                MessageBox.Show(this, "File was not open", "File was not open", MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            this.Init();
        }
        
        private void SaveToFile()
        {
            bool ok = FileCodeEditor.WriteCodeS(FilePath, CodeInput.Text);
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
            if (this.FilePath != null)
            {
                this.SaveToFile();
            }
            else
            {
                OnSaveAsClick(this, null);
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
            if (FileCodeEditor.ReadCodeS(FilePath, out string readedData))
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

        private void UpdateTitle()
        {
            this.Title = this.FilePath == string.Empty ? "VCPLBrowser" : $"VCPLBrowser ({BasicString.GetNameFromPath(this.FilePath)})";
        }

        private bool isRun = false;
        private void OnRunStopClick(object sender, RoutedEventArgs e)
        {
            if (isRun) program.Interrupt();
            else
            {
                /// compilation should goes in new thread

                List<ICodeLine> codeLines = new List<ICodeLine>();
                try
                {
                    foreach (string line in CodeInput.Text.Split("\r\n"))
                    {
                        if (BasicString.IsNoDataString(line)) continue;
                        codeLines.Add(_codeConvertor.Convert(line));
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

                ICompilator compilator = new Compilator_DF_A();
                
                try
                {
                    compilator.ReloadAssemblyLoadContext();
                    main = compilator.Compilate(codeLines, context);
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
                            FunctionInstance copyMain = (FunctionInstance)main.Get();
                            copyMain.Invoke(context.Pack(), Pointer.NULL, new Pointer[0]); // think about args
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
                    catch (ThreadInterruptedException)
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
            throw new NotImplementedException();
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