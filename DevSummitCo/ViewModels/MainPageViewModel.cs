using System.Linq;
using System.Windows.Input;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Mapping;
using Esri.ArcGISRuntime.Symbology;
using Esri.ArcGISRuntime.UI;
using Prism.Commands;
using Prism.Navigation;
using Xamarin.Forms;

namespace DevSummitCo.ViewModels
{
  public class MainPageViewModel : ViewModelBase
  {
    private Map _map;
    private GraphicsOverlayCollection _graphicOverlyaCollection;
    private Viewpoint _newViewpoint;
    private Viewpoint _actualViewpoint;

    /// <summary>
    /// 
    /// </summary>
    public Map Map
    {
      get => _map;
      set => SetProperty(ref _map, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public GraphicsOverlayCollection GraphicsOverlayCollection
    {
      get => _graphicOverlyaCollection;
      set => SetProperty(ref _graphicOverlyaCollection, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public Viewpoint NewViewpoint
    {
      get => _newViewpoint;
      set => SetProperty(ref _newViewpoint, value);
    }

    /// <summary>
    /// 
    /// </summary>
    public Viewpoint ActualViewpoint
    {
      get => _actualViewpoint;
      set => SetProperty(ref _actualViewpoint, value);
    }
    /// <summary>
    /// 
    /// </summary>
    public ICommand AddGraphicsCommand { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public ICommand CenterMapCommand { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public ICommand ClearEventsCommand { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public ICommand UpdateViewpointCommand { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    private MapPoint CentralPoint { get; set; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="navigationService"></param>
    public MainPageViewModel(INavigationService navigationService)
                  : base(navigationService)
    {
      Title = "Demo Mac Nugets EsriDevSummit Colombia";

      Map = new Map(Basemap.CreateStreets());
      GraphicsOverlayCollection = new GraphicsOverlayCollection()
            {
                new GraphicsOverlay() { Id = "Hotel Cosmos 100"},
                new GraphicsOverlay() { Id = "Eventos"}
            };
      CentralPoint = new MapPoint(-74.054424, 4.685715,
          SpatialReferences.Wgs84);

      var graphic = new Graphic()
      {
        Geometry = CentralPoint,
        Symbol = new SimpleMarkerSymbol
        {
          Color = Color.DarkGreen,
          Style = SimpleMarkerSymbolStyle.Diamond,
          Size = 20
        }
      };

      var textGraphic = new Graphic()
      {
        Geometry = CentralPoint,
        Symbol = new TextSymbol
        {
          Text = "Hotel Cosmos 100",
          Color = Color.Black,
          Size = 15,
          OffsetY = 20
        }
      };

      GraphicsOverlayCollection.First().Graphics.Add(graphic);
      GraphicsOverlayCollection.First().Graphics.Add(textGraphic);
      var viewpoint = new Viewpoint(CentralPoint, 5000);

      Map.InitialViewpoint = viewpoint;
      NewViewpoint = Map.InitialViewpoint;

      AddGraphicsCommand = new DelegateCommand<MapPoint>(AddGraphicsAction);
      ClearEventsCommand = new DelegateCommand(ClearEventsAction);
      CenterMapCommand = new DelegateCommand(CenterMapAction);
      UpdateViewpointCommand = new DelegateCommand<Viewpoint>(UpdateViewpoint);

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="viewpoint"></param>
    private void UpdateViewpoint(Viewpoint viewpoint)
    {
      if (viewpoint != null)
      {
        ActualViewpoint = viewpoint.TargetGeometry is MapPoint ?
          new Viewpoint(
            GeometryEngine.Project(viewpoint.TargetGeometry, SpatialReferences.Wgs84) as MapPoint,
            viewpoint.TargetScale) :
          new Viewpoint(
            GeometryEngine.Project(viewpoint.TargetGeometry, SpatialReferences.Wgs84),
            viewpoint.Camera);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="location"></param>
    private void AddGraphicsAction(MapPoint location)
    {
      var graphic = new Graphic()
      {
        Geometry = location,
        Symbol = new SimpleMarkerSymbol
        {
          Color = Color.Blue,
          Style = SimpleMarkerSymbolStyle.Cross,
          Size = 20
        }
      };
      var graphicOverlay = GraphicsOverlayCollection.FirstOrDefault(go => go.Id == "Eventos");
      if (graphicOverlay != null)
      {
        graphicOverlay.Graphics.Add(graphic);
      }
    }

    /// <summary>
    /// 
    /// </summary>
    private void ClearEventsAction()
    {
      var graphicOverlay = GraphicsOverlayCollection.FirstOrDefault(go => go.Id == "Eventos");
      if (graphicOverlay != null)
      {
        graphicOverlay.Graphics.Clear();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    private void CenterMapAction()
    {
      NewViewpoint = new Viewpoint(CentralPoint, 5000);
    }

  }
}
