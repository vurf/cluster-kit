using System;

namespace Xamarin.iOS.ClusterKit
{
    public class CKBottomCluster : CKCluster
    {
        public override void AddAnnotation(CKAnnotation annotation)
        {
            if (annotation.Cluster != this)
            {
                var index = this.Annotations.IndexOf(annotation);
                this.Annotations.Insert(index, annotation);
                this.SetCoordinate(this.Annotations[0].Coordinate);
            }
        }

        public override void RemoveAnnotation(CKAnnotation annotation)
        {
            if (annotation.Cluster == this)
            {
                this.Annotations.Remove(annotation);
                annotation.Cluster = null;
                this.SetCoordinate(this.Annotations[0].Coordinate);
            }
        }
    }
}
