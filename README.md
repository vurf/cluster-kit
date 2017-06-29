# cluster-kit
Xamarin.iOS.ClusterKit

It's rewritten library ClusterKit from objc to C#.



https://www.cocoacontrols.com/controls/clusterkit  
https://github.com/hulab/ClusterKit

<p align="center">
  <img src="Resources/git_banner.png" width=434 />
</p>

----------------

ClusterKit is an elegant and efficiant clustering controller for maps. Its flexible architecture make it very customizable, you can use your own algorithm and even your own map provider. 

## Features

+ Native supports of **MapKit**.
+ Comes with 2 clustering algorithms, a Grid Based Algorithm and a Non Hierarchical Distance Based Algorithm.
+ Annotations are stored in a [QuadTree](https://en.wikipedia.org/wiki/Quadtree) for efficient region queries.
+ Cluster center can be switched to Centroid, Nearest Centroid, Bottom.
+ Written in C#.

<p align="center" margin=20>
    <img src="Resources/apple_maps.gif" alt="Apple Plan" style="padding:20px;">
</p>

## Installation

### Nuget

ClusterKit is unavailable through [Nuget](http://cocoapods.org).

```ruby
'Xamarin.iOS.ClusterKit'
```

## Usage

### MapKit

##### Configure the cluster manager

```C#
var algorithm = new CKNonHierarchicalDistanceBasedAlgorithm();
this.MapView.ClusterManager.Algorithm = algorithm;
this.MapView.ClusterManager.Annotations = annotations;
```

##### Handle interactions in the map view's delegate

```C#
[Export("mapView:regionDidChangeAnimated:")]
public void RegionChanged(MKMapView mapView, bool animated)
{
    if (mapView is CKMapView clusterMap)
    {
        clusterMap.ClusterManager.UpdateClustersIfNeeded();
    }
}

[Export("mapView:didSelectAnnotationView:")]
public void DidSelectAnnotationView(MKMapView mapView, MKAnnotationView view)
{
    var clusterMapView = mapView as CKMapView;
    var cluster = view.Annotation as CKCluster;
    if (cluster != null && clusterMapView != null)
    {
        if (cluster.Annotations.Count > 1)
        {
            var insets = new UIEdgeInsets(20, 20, 20, 20);
            clusterMapView.ShowCluster(cluster, insets, true);

        }
    }
}
```

## Credits

Assets by [Hugo des Gayets](https://dribbble.com/hugodesgayets).

## License

ClusterKit is available under the MIT license. See the [LICENSE](LICENSE) file for more info.
