using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CoreLocation;
using Foundation;
using MapKit;
using Newtonsoft.Json;
using UIKit;
using Xamarin.iOS.ClusterKit;
using Xamarin.iOS.ClusterKit.Algorithms;

namespace Sample.ClusterKit
{
    public partial class MyViewController : UIViewController, IMKMapViewDelegate
    {
        [Outlet]
        public MKMapView MapView { get; set; }

        public MyViewController() : base("MyViewController", null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var algorithm = new CKNonHierarchicalDistanceBasedAlgorithm();
            algorithm.CellSize = 200;

            this.MapView.GetClusterManager().Algorithm = algorithm;
            this.MapView.GetClusterManager().MarginFactor = 1;

            this.MapView.Delegate = this;

            var paris = new CLLocationCoordinate2D(48.853, 2.35);
            this.MapView.SetCenterCoordinate(paris, false);
            this.LoadData();
        }

        private void LoadData()
        {
            this.LoadFromJson();
        }

        private void LoadFromJson()
        {
            var json = File.ReadAllText("static.json");
            var c = JsonConvert.DeserializeObject<RootObject>(json);
            this.MapView.GetClusterManager().Annotations = c.features
                .Select(
                    x => new CKAnnotation(new CLLocationCoordinate2D(x.geometry.coordinates[1], x.geometry.coordinates[0]))
                    {
                        Title1 = x.properties.title,
                        Subtitle1 = x.properties.subtitle
                    }
            ).ToList();
        }

        private void LoadFromRandom()
        {
            var annotations = new List<CKAnnotation>();
            for (int i = 0; i < 500; i++)
            {
                var ann = new CKAnnotation();
                ann.SetCoordinate(new CLLocationCoordinate2D(GetRandomNumber(0, 1) * 40 - 20, GetRandomNumber(0, 1) * 80 - 40));
                annotations.Add(ann);
            }
        }

        public static Random random = new Random();

        public static double GetRandomNumber(double minimum, double maximum)
        {
            return random.NextDouble() * (maximum - minimum) + minimum;
        }

        [Export("mapView:viewForAnnotation:")]
        public MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            var annotationView = mapView.DequeueReusableAnnotation("annotation") ??
                                        new MKAnnotationView(annotation, "annotation");

            var cluster = annotation as CKCluster;
            if (cluster != null)
            {
                if (cluster.Annotations.Count > 1)
                {
                    annotationView.CanShowCallout = false;
                    annotationView.Image = UIImage.FromBundle("cluster");
                }
                else
                {
                    annotationView.CanShowCallout = true;
                    annotationView.Image = UIImage.FromBundle("marker");
                }
            }

            return annotationView;
        }

        [Export("mapView:regionDidChangeAnimated:")]
        public void RegionChanged(MKMapView mapView, bool animated)
        {
            mapView.GetClusterManager().UpdateClustersIfNeeded();
        }

        [Export("mapView:didSelectAnnotationView:")]
        public void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
        {
            var cluster = view.Annotation as CKCluster;
            if (cluster != null )
            {
                if (cluster.Annotations.Count > 1)
                {
                    var insets = new UIEdgeInsets(20, 20, 20, 20);
                    mapView.ShowCluster(cluster, insets, true);
                }
            }
        }

    }
}

