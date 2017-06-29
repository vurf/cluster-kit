using System;
using MapKit;
using UIKit;
using CoreLocation;
using ObjCRuntime;

namespace Xamarin.iOS.ClusterKit
{
    [Category(typeof(MKMapView))]
    public static class MKMapViewExt
    {
        private static CKClusterManager clusterManager;

        public static double GetZoom(this MKMapView map)
        {
            return Math.Log(360 * ((map.Bounds.Width / 256) / map.Region.Span.LongitudeDelta), 2);
        }

        public static CKClusterManager GetClusterManager(this MKMapView map)
        {
            if (clusterManager == null)
            {
                clusterManager = new CKClusterManager();
                clusterManager.SetMap(map);
            }

            return clusterManager;
        }

        public static void ShowCluster(this MKMapView map, CKCluster cluster, bool animated)
        {
            ShowCluster(map, cluster, UIEdgeInsets.Zero, animated);
        }

        public static void ShowCluster(this MKMapView map, CKCluster cluster, UIEdgeInsets insets, bool animated)
        {
            MKMapRect zoomRect = MKMapRect.Null;

            foreach (var annotation in cluster.Annotations)
            {
                var pointRect = new MKMapRect();
                pointRect.Origin = MKMapPoint.FromCoordinate(annotation.Coordinate);
                pointRect.Size = new MKMapSize(0.1d, 0.1d);
                zoomRect = MKMapRect.Union(zoomRect, pointRect);
            }

            map.SetVisibleMapRect(zoomRect, insets, animated);
        }

        public static void MoveCluster(this MKMapView map, CKCluster cluster, CLLocationCoordinate2D from, CLLocationCoordinate2D to, UICompletionHandler completion)
        {
            cluster.SetCoordinate(from);

            if (GetClusterManager(map).Delegate != null)
            {
                GetClusterManager(map).Delegate.ClusterManager(GetClusterManager(map), () => { cluster.SetCoordinate(to); }, completion);
            }
            else
            {
                UIView.Animate(GetClusterManager(map).AnimationDuration,
                               0,
                               GetClusterManager(map).AnimationOptions,
                               () => { cluster.SetCoordinate(to); },
                               () => completion?.Invoke(true));
            }
        }

        public static void AddCluster(this MKMapView map, CKCluster cluster)
        {
            map.AddAnnotation(cluster);
        }

        public static void RemoveCluster(this MKMapView map, CKCluster cluster)
        {
            map.RemoveAnnotation(cluster);
        }

        public static void SelectCluster(this MKMapView map, CKCluster cluster, bool animated)
        {
            map.SelectAnnotation(cluster, animated);
        }

        public static void DeselectCluster(this MKMapView map, CKCluster cluster, bool animated)
        {
            map.DeselectAnnotation(cluster, animated);
        }
    }
}
