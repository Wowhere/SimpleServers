using ReactiveUI;
using System.Collections.ObjectModel;
using simpleserver.Models;
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
using EmbedIO.Actions;
using EmbedIO.Routing;
using EmbedIO.WebApi;
using EmbedIO;
using System.Threading;
using System;
using System;
using System.Threading.Tasks;
using System.Threading;
using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.WebApi;
using EmbedIO.Files;
using EmbedIO.Authentication;
using EmbedIO.Routing;
using System.Collections.Generic;

namespace simpleserver.ViewModels
{
    public class TestController : WebApiController
    {
        [Route(HttpVerbs.Get, "/t")]
        public string Test()
        {
            HttpContext.Response.Headers.Clear();
            HttpContext.Response.Headers.Add("Server1: lol123");
            return "testtest";
        }
        public TestController()
        {
        }

    }
    public class HttpServerRunner
    {
        public string port = "8080";
        public string folder = "";
        public string Status = "lol";
        public bool IsLaunched = false;
        public bool IsSecure = false;
        CancellationTokenSource ctSource;
        private WebServer server;
        public HttpServerRunner()
        {
        }
        public void Start()
        {
            try
            {
                ctSource = new CancellationTokenSource();
                var server = new WebServer(o => o
                    .WithUrlPrefix("http://*:" + port)
                    .WithMode(HttpListenerMode.EmbedIO))
                    .WithLocalSessionManager()
                    .WithStaticFolder("/", folder, true, m => m.WithContentCaching())
                    .WithModule(new ActionModule("/", HttpVerbs.Any, ctx => ctx.SendDataAsync(new { Message = "Error" })))
                    .WithWebApi("/", m => m.WithController<TestController>());
                Status = "Running";
                IsLaunched = true;
                server.RunAsync(ctSource.Token).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                IsLaunched = false;
                Status = ex.Message.ToString();
            }
        }
        public void Stop()
        {
            ctSource.Cancel();
            IsLaunched = false;
            Status = "Stopped";
        }
    }
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
        private Label StatusTextboxInit(HttpServerRunner opt)
        {
            var t = new Label();
            t.Content = opt.Status;
            return t;
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
            var TextColumnLength = new GridLength(1, GridUnitType.Star);
            var TemplateColumnLength = new GridLength(125, GridUnitType.Pixel);
            var EditOptions = new TextColumnOptions<HttpServerRunner>
            {
                BeginEditGestures = BeginEditGestures.Tap,
                MinWidth = new GridLength(80, GridUnitType.Pixel)
            };
            TextColumn<HttpServerRunner, string> PortColumn = new TextColumn<HttpServerRunner, string>("Port", x => x.port, (r, v) => r.port = v, options: EditOptions);
            TextColumn<HttpServerRunner, string> DirectoryColumn = new TextColumn<HttpServerRunner, string>("Directory", x => x.folder, (r, v) => r.folder = v, options: EditOptions);
            //TemplateColumn<HttpServerRunner> StatusColumn = new TemplateColumn<HttpServerRunner>("Status", new FuncDataTemplate<HttpServerRunner>((a, e) => StatusTextboxInit(a), supportsRecycling: true));
            //TemplateColumn<HttpServerRunner> StatusColumn = new TemplateColumn<HttpServerRunner>("Status", new FuncDataTemplate<HttpServerRunner>((a, e) => StatusTextboxInit(a), supportsRecycling: true));
            //TextColumn<HttpServerRunner, string> StatusColumn = new TextColumn<HttpServerRunner, string>("Status", x => x.status, (r, v) => r.status = v, width: TextColumnLength);
            TemplateColumn<HttpServerRunner> StatusColumn = new TemplateColumn<HttpServerRunner>("Status", "statusText", width: TemplateColumnLength);
            TemplateColumn<HttpServerRunner> ButtonColumn = new TemplateColumn<HttpServerRunner>("", new FuncDataTemplate<HttpServerRunner>((a, e) => ToggleHttpServerButtonInit(a), supportsRecycling: true));
            ServerGridData = new FlatTreeDataGridSource<HttpServerRunner>(ServerRows)
            {
                Columns =
                {
                ButtonColumn, PortColumn, DirectoryColumn, StatusColumn  //HeadersColumn
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
