namespace NetClient.Interfaces
{
    public interface INetworkClient
    {
        List<string> GetRemoteIPv4Addresses(string hostNameOrAddress);
    }
}
