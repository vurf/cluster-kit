using System;
using MapKit;
using UIKit;
using CoreLocation;
using Foundation;

namespace Xamarin.iOS.ClusterKit
{
    [Register("CKMapView")]
    public class CKMapView : MKMapView, ICKMap
    {
        private CKClusterManager clusterManager;

        public CKMapView(IntPtr handle) : base(handle)
        {

        }

        public CKClusterManager ClusterManager
        {
            get
            {
                if (clusterManager == null)
                {
                    clusterManager = new CKClusterManager();
                    clusterManager.SetMap(this);
                }

                return clusterManager;
            }
        }

        public double Zoom => Math.Log(360 * ((this.Frame.Width / 256) / this.Region.Span.LongitudeDelta), 2);

        public void ShowCluster(CKCluster cluster, bool animated)
        {
            this.ShowCluster(cluster, UIEdgeInsets.Zero, animated);
        }

        public void ShowCluster(CKCluster cluster, UIEdgeInsets insets, bool animated)
        {
            MKMapRect zoomRect = MKMapRect.Null;

            foreach (var annotation in cluster.Annotations)
            {
                var pointRect = new MKMapRect();
                pointRect.Origin = MKMapPoint.FromCoordinate(annotation.Coordinate);
                pointRect.Size = new MKMapSize(0.1d, 0.1d);
                zoomRect = MKMapRect.Union(zoomRect, pointRect);
            }

            this.SetVisibleMapRect(zoomRect, insets, animated);
        }

        public void MoveCluster(CKCluster cluster, CLLocationCoordinate2D from, CLLocationCoordinate2D to, UICompletionHandler completion)
        {
            cluster.SetCoordinate(from);

            if (this.ClusterManager.Delegate != null)
            {
                this.ClusterManager.Delegate.ClusterManager(this.ClusterManager, () => { cluster.SetCoordinate(to); }, completion);
            }
            else
            {
                UIView.Animate(this.ClusterManager.AnimationDuration,
                               0,
                               this.ClusterManager.AnimationOptions,
                               () => { cluster.SetCoordinate(to); },
                               () => completion?.Invoke(true));
            }
        }

        public void AddCluster(CKCluster cluster)
        {
            this.AddAnnotation(cluster);
        }

        public void RemoveCluster(CKCluster cluster)
        {
            this.RemoveAnnotation(cluster);
        }

        public void SelectCluster(CKCluster cluster, bool animated)
        {
            this.SelectAnnotation(cluster, animated);
        }

        public void DeselectCluster(CKCluster cluster, bool animated)
        {
            this.DeselectAnnotation(cluster, animated);
        }
    }
}
