using System;

namespace Xamarin.iOS.ClusterKit.Tree
{
    public interface ICKAnnotationTreeDelegate
    {
        bool AnnotationTree(ICKAnnotationTree annotationTree, CKAnnotation annotation);
    }
}
