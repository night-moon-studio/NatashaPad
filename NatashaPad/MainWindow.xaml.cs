using System;
using System.Windows;
using System.Windows.Threading;

namespace NatashaPad
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly INScriptEngine _scriptEngine;
        private readonly DumperResolver _dumperResolver;
        private readonly object _outputLock = new object();

        public MainWindow(DumperResolver dumperResolver)
        {
            _scriptEngine = new CSharpScriptEngine((str) =>
            {
                if (str is null)
                    return;

                //https://stackoverflow.com/questions/1644079/change-wpf-controls-from-a-non-main-thread-using-dispatcher-invoke
                if (Application.Current.Dispatcher.CheckAccess())
                {
                    txtOutput.AppendText(str);
                    txtOutput.AppendText(Environment.NewLine);
                }
                else
                {
                    Application.Current.Dispatcher.BeginInvoke(
                        DispatcherPriority.Background,
                        new Action(() =>
                        {
                            txtOutput.AppendText(str);
                            txtOutput.AppendText(Environment.NewLine);
                        }));
                }
            });
            _dumperResolver = dumperResolver;
            InitializeComponent();
            txtInput.Text = "\"Hello NatashaPad\"";
            txtOutput.AppendText("Output");
        }

        private async void BtnRun_OnClick(object sender, RoutedEventArgs e)
        {
            var input = txtInput.Text?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            txtOutput.Text = string.Empty;

            try
            {
                if (input.EndsWith(";"))
                {
                    // statements, execute
                    await _scriptEngine.Execute(input, new NScriptOptions());
                }
                else
                {
                    // expression, eval
                    var result = await _scriptEngine.Eval(txtInput.Text, new NScriptOptions());

                    if (null == result)
                    {
                        txtOutput.AppendText("(null)");
                    }
                    else
                    {
                        var dumpedResult = _dumperResolver.Resolve(result.GetType())
                            .Dump(result);
                        txtOutput.AppendText(dumpedResult);
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "执行发生异常");
            }
        }
    }
}