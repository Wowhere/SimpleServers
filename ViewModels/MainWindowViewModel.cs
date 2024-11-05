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
using Avalonia.Data;
using Avalonia.Styling;
using Avalonia.Media;
using DynamicData.Kernel;

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
            var server = new ServerOptions("8080", "", headers);
            ServerRows.Add(server);
        }
        private void ToggleHttpServerStyle(bool status, Button button)
        {
            if (status)
            {
                button.Styles.Add(new Style()
                {
                    Setters = {
                    new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.Parse("#D70040")) ),
                    new Setter(Button.ContentProperty, "Stop Server" )
                }
                });
            }
            else
            {
                button.Styles.Add(new Style()
                {
                    Setters = {
                    new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.Parse("#097969")) ),
                    new Setter(Button.ContentProperty, "Start Server" )
                }
                });
            }
        }
        private void ToggleHttpServer(object sender, RoutedEventArgs e) {
            Button startButton = (Button)sender;
            ServerOptions options = (ServerOptions)startButton.DataContext;
            if (!options.IsLaunched)
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {                    
                    options.server.Start();
                });
            } else
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    options.server.Stop();
                });
            }
            options.IsLaunched = !options.IsLaunched;
            ToggleHttpServerStyle(options.IsLaunched, startButton);
        }
        private Button ToggleHttpServerButtonInit(ServerOptions opt)
        {
            var AddServerButton = new Button();
            ToggleHttpServerStyle(opt.IsLaunched, AddServerButton);
            AddServerButton.Click += ToggleHttpServer;
            return AddServerButton;
        }

        public void TreeDataGridInit()
        {
            var EditOptions = new TextColumnOptions<ServerOptions>
            {
                BeginEditGestures = BeginEditGestures.Tap,
                MinWidth = new GridLength(80, GridUnitType.Pixel)
            };
            TextColumn<ServerOptions, string> PortColumn = new TextColumn<ServerOptions, string>("Port", x => x.Port, (r, v) => r.Port = v, options: EditOptions);
            TextColumn<ServerOptions, string> DirectoryColumn = new TextColumn<ServerOptions, string>("Directory", x => x.WorkingDirectory, (r, v) => r.WorkingDirectory = v, options: EditOptions);
            //TreeDataGrid here. var HeadersColumn = new TreeDataGrid<ServerOptions, string[]>("Headers", x => x.Headers);
            TemplateColumn<ServerOptions> ButtonColumn = new TemplateColumn<ServerOptions>("", new FuncDataTemplate<ServerOptions>((a, e) => ToggleHttpServerButtonInit(a), supportsRecycling: true));
            ServerGridData = new FlatTreeDataGridSource<ServerOptions>(ServerRows)
            {
                Columns =
                {
                PortColumn, DirectoryColumn, ButtonColumn //HeadersColumn
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
