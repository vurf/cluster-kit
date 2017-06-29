using System;
using CoreLocation;
using MapKit;
using System.Collections.Generic;

namespace Xamarin.iOS.ClusterKit
{
    public class CKCluster : MKAnnotation
    {
        protected List<CKAnnotation> annotations;
        private CLLocationCoordinate2D coordinate;

        public CKCluster()
        {
            this.annotations = new List<CKAnnotation>();
            this.coordinate = new CLLocationCoordinate2D();
        }

        public override CLLocationCoordinate2D Coordinate => this.coordinate;

        public override void SetCoordinate(CLLocationCoordinate2D value)
        {
            this.WillChangeValue("coordinate");
            this.coordinate = value;
            this.DidChangeValue("coordinate");
        }

        public List<CKAnnotation> Annotations
        {
            get { return this.annotations; }
        }

        public virtual void AddAnnotation(CKAnnotation annotation)
        {
            annotation.Cluster = this;
            this.Annotations.Add(annotation);
        }

        public virtual void RemoveAnnotation(CKAnnotation annotation)
        {
            if (annotation.Cluster == this)
            {
                this.Annotations.Remove(annotation);
                annotation.Cluster = null;
            }
        }

        public static CKCluster ClusterWithCoordinate(CLLocationCoordinate2D coordinate)
        {
            var cluster = new CKCluster();
            cluster.SetCoordinate(coordinate);
            return cluster;
        }

        public override string Title
        {
            get
            {
                if (this.Annotations.Count == 1)
                {
                    return string.IsNullOrEmpty(this.Annotations[0].Title1) == false 
                                 ? this.Annotations[0].Title1 
                                     : this.Annotations[0].Coordinate.Latitude.ToString();
                }

                return null;
            }
        }

        public override string Subtitle
        {
            get
            {
                if (this.Annotations.Count == 1)
                {
                    return string.IsNullOrEmpty(this.Annotations[0].Subtitle1) == false 
                                 ? this.Annotations[0].Subtitle1 
                                     : this.Annotations[0].Coordinate.Latitude.ToString();
                }

                return null;
            }
        }
    }
}