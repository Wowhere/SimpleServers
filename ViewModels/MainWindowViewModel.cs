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
using AvaloniaEdit.Document;
using Avalonia;

namespace simpleserver.ViewModels
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
        private void OpenLogViewer(object sender, RoutedEventArgs e)
        {
            var newWindow = new Window() { DataContext = new MainWindowViewModel()};
            newWindow.AttachDevTools();
            Button logButton = (Button)sender;

            

            HttpServerRunner runner = (HttpServerRunner)logButton.DataContext;
            newWindow.Title = "Server on " + runner.port + " port log...";
            var logTextBox = new AvaloniaEdit.TextEditor();
            var logText = new TextDocument(runner.Log.ToString());
            logTextBox.DataContext = newWindow.DataContext;
            logTextBox.Document = logText;
            logTextBox.ShowLineNumbers = true;
            logTextBox.IsReadOnly = true;
            logTextBox.BorderBrush = new SolidColorBrush(Color.Parse("#D70040"));
            var panel = new DockPanel();

            panel.Children.Add(logTextBox);
            newWindow.Content = panel;
            newWindow.Show();
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
        private Button ToggleViewLogButtonInit(HttpServerRunner opt)
        {
            var LogViewButton = new Button();
            LogViewButton.Click += OpenLogViewer;
            LogViewButton.Content = "View log";
            return LogViewButton;
        }

        private Button ToggleHttpServerButtonInit(HttpServerRunner opt)
        {
            var AddServerButton = new Button();
            ToggleHttpServerStyle(opt.IsLaunched, AddServerButton);
            AddServerButton.Click += ToggleHttpServer;
            return AddServerButton;
        }
        private TextBox StatusTextboxInit(HttpServerRunner opt)
        {
            var t = new TextBox();
            t.Text = opt.Status;
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
            var ReadOptions = new TextColumnOptions<HttpServerRunner>
            {
                BeginEditGestures = BeginEditGestures.None,
                TextWrapping = TextWrapping.Wrap,
                CanUserResizeColumn = false,
            };
            TextColumn<HttpServerRunner, string> PortColumn = new TextColumn<HttpServerRunner, string>("Port", x => x.port, (r, v) => r.port = v, options: EditOptions);
            TextColumn<HttpServerRunner, string> DirectoryColumn = new TextColumn<HttpServerRunner, string>("Directory", x => x.folder, (r, v) => r.folder = v, options: EditOptions);
            TextColumn<HttpServerRunner, string> StatusColumn = new TextColumn<HttpServerRunner, string>("Status", x => x.Status, options: ReadOptions);
            TemplateColumn<HttpServerRunner> ButtonColumn = new TemplateColumn<HttpServerRunner>("", new FuncDataTemplate<HttpServerRunner>((a, e) => ToggleHttpServerButtonInit(a), supportsRecycling: true));
            TemplateColumn<HttpServerRunner> LogColumn = new TemplateColumn<HttpServerRunner>("", new FuncDataTemplate<HttpServerRunner>((a, e) => ToggleViewLogButtonInit(a), supportsRecycling: true));
            ServerGridData = new FlatTreeDataGridSource<HttpServerRunner>(ServerRows)
            {
                Columns =
                {
                ButtonColumn, PortColumn, DirectoryColumn, StatusColumn, LogColumn  //HeadersColumn
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
