using System;
using CoreLocation;

namespace Xamarin.iOS.ClusterKit
{
    public class CKCentroidCluster : CKCluster
    {
        public override void AddAnnotation(CKAnnotation annotation)
        {
            base.AddAnnotation(annotation);
            this.SetCoordinate(this.CoordinateByAddingAnnotation(annotation));
        }

        public override void RemoveAnnotation(CKAnnotation annotation)
        {
            if (annotation.Cluster == this)
            {
                base.RemoveAnnotation(annotation);
                this.SetCoordinate(this.CoordinateByRemovingAnnotation(annotation));
            }
        }

        protected CLLocationCoordinate2D CoordinateByAddingAnnotation(CKAnnotation annotation)
        {
            if (this.Annotations.Count < 2)
            {
                return annotation.Coordinate;
            }

            double lat = this.Coordinate.Latitude * (this.Annotations.Count - 1);
            double lon = this.Coordinate.Longitude * (this.Annotations.Count - 1);
            lat += annotation.Coordinate.Latitude;
            lon += annotation.Coordinate.Longitude;
            return new CLLocationCoordinate2D(lat / this.Annotations.Count, lon / this.Annotations.Count);
        }

        protected CLLocationCoordinate2D CoordinateByRemovingAnnotation(CKAnnotation annotation)
        {
            if (this.Annotations.Count < 1)
            {
                return new CLLocationCoordinate2D();
            }

            double lat = this.Coordinate.Latitude * (this.Annotations.Count + 1);
            double lon = this.Coordinate.Longitude * (this.Annotations.Count + 1);
            lat -= annotation.Coordinate.Latitude;
            lon -= annotation.Coordinate.Longitude;
            return new CLLocationCoordinate2D(lat / this.Annotations.Count, lon / this.Annotations.Count);
        }
    }
}
