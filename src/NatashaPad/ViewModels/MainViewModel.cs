using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;

using NatashaPad.ViewModels.Base;

using Prism.Commands;

namespace NatashaPad.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly INScriptEngine _scriptEngine;
        private readonly DumperResolver _dumperResolver;
        private readonly NScriptOptions _scriptOptions;

        public MainViewModel(DumperResolver dumperResolver,
            INScriptEngine scriptEngine,
            NScriptOptions scriptOptions,
            CommonParam commonParam) : base(commonParam)
        {
            _dumperResolver = dumperResolver;
            _scriptEngine = scriptEngine;
            _scriptOptions = scriptOptions;

            _input = "\"Hello NatashaPad\"";
            _output = "Output";

            DumpOutHelper.OutputAction += Dump;

            RunCommand = new DelegateCommand(async () => await RunAsync());
            UsingManageCommand = new DelegateCommand(UsingManageShow);
        }

        private void Dump(string content)
        {
            if (content is null)
            {
                return;
            }

            //https://stackoverflow.com/questions/1644079/change-wpf-controls-from-a-non-main-thread-using-dispatcher-invoke
            if (Dispatcher.CheckAccess())
            {
                Do();
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Background, (Action)Do);
            }

            void Do() => Output += $"{content}{Environment.NewLine}";
        }

        private string _input;
        public string Input
        {
            get => _input;
            set => SetProperty(ref _input, value);
        }

        private string _output;
        public string Output
        {
            get => _output;
            set => SetProperty(ref _output, value);
        }

        public ICommand RunCommand { get; }
        private async Task RunAsync()
        {
            string input = _input?.Trim();
            if (string.IsNullOrEmpty(input))
            {
                return;
            }
            Output = string.Empty;

            try
            {
                if (input.Contains("static void Main(") || input.EndsWith(";"))
                {
                    // statements, execute
                    await _scriptEngine.Execute(input, _scriptOptions);
                }
                else
                {
                    // expression, eval
                    object result = await _scriptEngine.Eval(input, _scriptOptions);

                    if (null == result)
                    {
                        Output += "(null)";
                    }
                    else
                    {
                        string dumpedResult = _dumperResolver.Resolve(result.GetType())
                            .Dump(result);
                        Output += dumpedResult;
                    }
                }
            }
            catch (Exception exception)
            {
                ShowMessage(exception.Message);
            }
        }

        public ICommand UsingManageCommand { get; }
        private void UsingManageShow()
        {
            UsingManageViewModel vm = Show<UsingManageViewModel>();
        }
        public ICommand RefManageCommand { get; }
    }
}
