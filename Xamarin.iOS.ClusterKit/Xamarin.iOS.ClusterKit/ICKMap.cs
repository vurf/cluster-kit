using System;
using CoreLocation;
using MapKit;
using UIKit;

namespace Xamarin.iOS.ClusterKit
{
    public interface ICKMap
    {
        CKClusterManager ClusterManager { get; }

        MKMapRect VisibleMapRect { get; set; }

        double Zoom { get; }

        void SelectCluster(CKCluster cluster, bool animated);

        void DeselectCluster(CKCluster cluster, bool animated);

        void AddCluster(CKCluster cluster);

        void RemoveCluster(CKCluster cluster);

        void MoveCluster(CKCluster cluster, CLLocationCoordinate2D from, CLLocationCoordinate2D to, UICompletionHandler completion);
    }
}
