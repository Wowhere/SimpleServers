using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Selection;
using Avalonia.Controls.Templates;
using Avalonia.Media;
using ReactiveUI;
using simpleserver.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace simpleserver.ViewModels
{
    public class HeaderWindowViewModel : ViewModelBase
    {
        public HttpServerRunner ObjectConfig;
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
        public ReactiveCommand<Unit, Unit> AddHeaderCommand { get; }
        public void AddHeader()
        {
            var headers = new string[1] { "" };
            var server = new HttpServerRunner();
            HeadersRows.Add(server);
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
            TextColumn<HttpServerRunner, string> HeaderName = new TextColumn<HttpServerRunner, string>("Header", x => x.port, (r, v) => r.port = v, options: EditOptions);
            TextColumn<HttpServerRunner, string> HeaderValue = new TextColumn<HttpServerRunner, string>("Value", x => x.folder, (r, v) => r.folder = v, options: EditOptions);

            HeaderGridData = new FlatTreeDataGridSource<HttpServerRunner>(HeadersRows)
            {
                Columns =
                {
                HeaderName, HeaderValue
                }
            };
            HeaderGridData.Selection = new TreeDataGridCellSelectionModel<HttpServerRunner>(HeaderGridData);
        }
        public HeaderWindowViewModel(HttpServerRunner obj)
        {
            ObjectConfig = obj;
            AddHeaderCommand = ReactiveCommand.Create(AddHeader);
            HeadersRows = new ObservableCollection<HttpServerRunner>();
            TreeDataGridInit();
        }
    }
}
