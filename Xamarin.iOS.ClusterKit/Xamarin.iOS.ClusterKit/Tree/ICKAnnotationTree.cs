using System.Collections.Generic;
using MapKit;

namespace Xamarin.iOS.ClusterKit.Tree
{
    public interface ICKAnnotationTree
    {
        ICKAnnotationTreeDelegate Delegate { get; set; }

        List<CKAnnotation> Annotations { get; set; }

        List<CKAnnotation> AnnotationsInRect(MKMapRect rect);
    }
}
