using System;
using UIKit;

namespace Xamarin.iOS.ClusterKit
{
    public interface ICKClusterManagerDelegate
    {
        bool ClusterManager(CKClusterManager manager, CKAnnotation annotation);

        void ClusterManager(CKClusterManager manager, Action animations, UICompletionHandler finished);
    }
}
