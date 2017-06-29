using System;
using CoreLocation;
using MapKit;

namespace Xamarin.iOS.ClusterKit
{
    public class CKDistance
    {
        public static double GetDistance(CLLocationCoordinate2D from, CLLocationCoordinate2D to)
        {
            MKMapPoint a = MKMapPoint.FromCoordinate(from);
            MKMapPoint b = MKMapPoint.FromCoordinate(to);
            return (a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y);
        }
    }
}
