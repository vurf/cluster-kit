using System;
using CoreLocation;

namespace Xamarin.iOS.ClusterKit
{
    public class CKNearestCentroidCluster : CKCentroidCluster
    {
        protected CLLocationCoordinate2D center;

        public override void AddAnnotation(CKAnnotation annotation)
        {
            if (annotation.Cluster != this)
            {
                annotation.Cluster = this;
                this.Annotations.Add(annotation);
                this.center = this.CoordinateByAddingAnnotation(annotation);
                this.SetCoordinate(this.CoordinateByDistanceSort());
            }
        }

        public override void RemoveAnnotation(CKAnnotation annotation)
        {
            if (annotation.Cluster == this)
            {
                this.Annotations.Remove(annotation);
                annotation.Cluster = null;
                this.center = this.CoordinateByRemovingAnnotation(annotation);
                this.SetCoordinate(this.CoordinateByDistanceSort());
            }
        }

        private CLLocationCoordinate2D CoordinateByDistanceSort()
        {
            this.Annotations.Sort(new Comparison<CKAnnotation>((CKAnnotation x, CKAnnotation y) =>
            {
                double d1 = CKDistance.GetDistance(this.center, x.Coordinate);
                double d2 = CKDistance.GetDistance(this.center, y.Coordinate);
                if (d1 > d2)
                {
                    return 1;
                }

                if (d1 < d2)
                {
                    return -1;
                }

                return 0;
            }));

            return this.Annotations[0].Coordinate;
        }
    }
}
