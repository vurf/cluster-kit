using System;
using CoreLocation;
using MapKit;

namespace Xamarin.iOS.ClusterKit
{
    public class CKAnnotation : MKAnnotation
    {
        private CLLocationCoordinate2D coordinate;

        public CKAnnotation()
        {

        }

        public CKAnnotation(CLLocationCoordinate2D coordinate)
        {
            this.coordinate = coordinate;
        }

        public string Title1 { get; set; }

        public string Subtitle1 { get; set; }

        public CKCluster Cluster { get; set; }

        public override CLLocationCoordinate2D Coordinate => this.coordinate;

        public override void SetCoordinate(CLLocationCoordinate2D value)
        {
            this.WillChangeValue("coordinate");
            this.coordinate = value;
            this.DidChangeValue("coordinate");
        }
    }
}
