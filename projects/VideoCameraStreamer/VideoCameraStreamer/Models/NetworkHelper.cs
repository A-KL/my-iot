namespace VideoCameraStreamer.Models
{
    using System.Linq;
    using Windows.Networking.Connectivity;

    public static  class NetworkHelper
    {
        public static string GetLocalIp()
        {
            var icp = NetworkInformation.GetInternetConnectionProfile();

            if (icp?.NetworkAdapter == null) return null;
            var hostname =
                NetworkInformation.GetHostNames()
                    .SingleOrDefault(
                        hn =>
                            hn.IPInformation?.NetworkAdapter != null && hn.IPInformation.NetworkAdapter.NetworkAdapterId
                            == icp.NetworkAdapter.NetworkAdapterId);

            return hostname?.CanonicalName;
        }
    }
}
