using ReactiveUI;
using System.Collections.ObjectModel;
using api_corelation.Models;
using System.Reactive;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Models.TreeDataGrid;
using System.Xml.Linq;

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
            var server = new ServerOptions(80, "", headers);
            ServerRows.Add(server);
        }
        public void TreeDataGridInit()
        {
            TextColumn<ServerOptions, int> PortColumn = new TextColumn<ServerOptions, int>("Port", x => x.Port);
            TextColumn<ServerOptions, string> DirectoryColumn = new TextColumn<ServerOptions, string>("Directory", x => x.WorkingDirectory);
            TextColumn<ServerOptions, string[]> HeadersColumn = new TextColumn<ServerOptions, string[]>("Headers", x => x.Headers);

            ServerGridData = new FlatTreeDataGridSource<ServerOptions>(ServerRows)
            {
                Columns =
                {
                PortColumn, DirectoryColumn, HeadersColumn
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
