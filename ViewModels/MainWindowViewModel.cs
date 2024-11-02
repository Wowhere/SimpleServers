using ReactiveUI;
using System.Collections.ObjectModel;
using api_corelation.Models;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Templates;
using Avalonia.Interactivity;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;

namespace api_corelation.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<ServerOptions>? _ServerRows;
        public ObservableCollection<ServerOptions>? ServerRows
        {
            get => _ServerRows;
            set => this.RaiseAndSetIfChanged(ref _ServerRows, value);
        }

        private FlatTreeDataGridSource<ServerOptions>? _ServerGridData;
        public FlatTreeDataGridSource<ServerOptions>? ServerGridData
        {
            get => _ServerGridData;
            set => this.RaiseAndSetIfChanged(ref _ServerGridData, value);
        }
        private ObservableCollection<ServerOptions>? _HeadersRows;
        public ObservableCollection<ServerOptions>? HeadersRows
        {
            get => _HeadersRows;
            set => this.RaiseAndSetIfChanged(ref _HeadersRows, value);
        }

        private FlatTreeDataGridSource<ServerOptions>? _HeaderGridData;
        public FlatTreeDataGridSource<ServerOptions>? HeaderGridData
        {
            get => _HeaderGridData;
            set => this.RaiseAndSetIfChanged(ref _HeaderGridData, value);
        }
        private bool _IsStarted = false;
        public bool IsStarted
        {
            get => _IsStarted;
            set => this.RaiseAndSetIfChanged(ref _IsStarted, value);
        }
        public ReactiveCommand<Unit, Unit> AddServerCommand { get; }
        public void AddServer()
        {
            var headers = new string[1] { "" };
            var server = new ServerOptions(8080, "", headers);
            ServerRows.Add(server);
        }
        private void LaunchServer(object sender, RoutedEventArgs e) {
            ToggleButton startButton = (ToggleButton)sender;
            ServerOptions options = (ServerOptions)startButton.DataContext;
            if (options.IsLaunched)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {

                });
            } else
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {

                });
            }
            }
        private ToggleButton LaunchControlInit()
        {
            var AddServerButton = new ToggleButton();
            AddServerButton.Click += LaunchServer;
            return AddServerButton;
        }

        public void TreeDataGridInit()
        {
            TextColumn<ServerOptions, int> PortColumn = new TextColumn<ServerOptions, int>("Port", x => x.Port);
            TextColumn<ServerOptions, string> DirectoryColumn = new TextColumn<ServerOptions, string>("Directory", x => x.WorkingDirectory);
            TextColumn<ServerOptions, string[]> HeadersColumn = new TextColumn<ServerOptions, string[]>("Headers", x => x.Headers);
            TemplateColumn<ServerOptions> ButtonColumn = new TemplateColumn<ServerOptions>("", new FuncDataTemplate<ServerOptions>((a, e) => LaunchControlInit(), supportsRecycling: true));
            ServerGridData = new FlatTreeDataGridSource<ServerOptions>(ServerRows)
            {
                Columns =
                {
                PortColumn, DirectoryColumn, HeadersColumn, ButtonColumn
                }
            };
            ServerGridData.Selection = new TreeDataGridCellSelectionModel<ServerOptions>(ServerGridData);
        }
        public MainWindowViewModel()
        {
            AddServerCommand = ReactiveCommand.Create(AddServer);
            ServerRows = new ObservableCollection<ServerOptions>();
            TreeDataGridInit();
        }
    }
}
