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
using Avalonia.Platform.Storage;
using Splat;
using DynamicData;

namespace api_corelation.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<HttpServerRunner>? _ServerRows;
        public ObservableCollection<HttpServerRunner>? ServerRows
        {
            get => _ServerRows;
            set => this.RaiseAndSetIfChanged(ref _ServerRows, value);
        }

        private FlatTreeDataGridSource<HttpServerRunner>? _ServerGridData;
        public FlatTreeDataGridSource<HttpServerRunner>? ServerGridData
        {
            get => _ServerGridData;
            set => this.RaiseAndSetIfChanged(ref _ServerGridData, value);
        }
        private ObservableCollection<HttpServerRunner>? _HeadersRows;
        public ObservableCollection<HttpServerRunner>? HeadersRows
        {
            get => _HeadersRows;
            set => this.RaiseAndSetIfChanged(ref _HeadersRows, value);
        }

        private FlatTreeDataGridSource<HttpServerRunner>? _HeaderGridData;
        public FlatTreeDataGridSource<HttpServerRunner>? HeaderGridData
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
            var server = new HttpServerRunner();
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
            HttpServerRunner runner = (HttpServerRunner)startButton.DataContext;
            if (!runner.IsLaunched)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    runner.Start();
                    ToggleHttpServerStyle(runner.IsLaunched, startButton);
                });
            } else
            {
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    runner.Stop();
                    ToggleHttpServerStyle(runner.IsLaunched, startButton);
                });
            }
        }
        private Button ToggleHttpServerButtonInit(HttpServerRunner opt)
        {
            var AddServerButton = new Button();
            ToggleHttpServerStyle(opt.IsLaunched, AddServerButton);
            AddServerButton.Click += ToggleHttpServer;
            return AddServerButton;
        }
        private Button FolderPickerInit()
        {
            
            var btn = new Button();
            return btn;
        }
        private DockPanel FolderPickerCell(string folder)
        {
            var panel = new DockPanel();
            panel.Children.Add(new TextBlock());
            panel.Children.Add(FolderPickerInit());
            panel.HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch;
            return panel;
        }
        public void TreeDataGridInit()
        {
            var EditOptions = new TextColumnOptions<HttpServerRunner>
            {
                BeginEditGestures = BeginEditGestures.Tap,
                MinWidth = new GridLength(80, GridUnitType.Pixel)
            };
            TextColumn<HttpServerRunner, string> PortColumn = new TextColumn<HttpServerRunner, string>("Port", x => x.port, (r, v) => r.port = v, options: EditOptions);
            TextColumn<HttpServerRunner, string> DirectoryColumn = new TextColumn<HttpServerRunner, string>("Directory", x => x.folder, (r, v) => r.folder = v, options: EditOptions);
            //TreeDataGrid here. var HeadersColumn = new TreeDataGrid<HttpServerRunner, string[]>("Headers", x => x.Headers);
            TemplateColumn<HttpServerRunner> ButtonColumn = new TemplateColumn<HttpServerRunner>("", new FuncDataTemplate<HttpServerRunner>((a, e) => ToggleHttpServerButtonInit(a), supportsRecycling: true));
            //TemplateColumn<HttpServerRunner> DirectoryColumn = new TemplateColumn<HttpServerRunner>("", new FuncDataTemplate<HttpServerRunner>((a, e) => FolderPickerCell(a.folder), supportsRecycling: true));
            ServerGridData = new FlatTreeDataGridSource<HttpServerRunner>(ServerRows)
            {
                Columns =
                {
                PortColumn, DirectoryColumn, ButtonColumn //HeadersColumn
                }
            };
            ServerGridData.Selection = new TreeDataGridCellSelectionModel<HttpServerRunner>(ServerGridData);
        }
        public MainWindowViewModel()
        {
            AddServerCommand = ReactiveCommand.Create(AddServer);
            ServerRows = new ObservableCollection<HttpServerRunner>();
            TreeDataGridInit();
        }
    }
}
