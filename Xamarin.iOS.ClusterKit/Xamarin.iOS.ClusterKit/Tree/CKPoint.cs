using System;
using MapKit;

namespace Xamarin.iOS.ClusterKit.Tree
{
    public class CKPoint
    {
        public MKMapPoint Point { get; set; }

        public IMKAnnotation Annotation { get; set; }

        public CKPoint Next { get; set; }
    }
}
