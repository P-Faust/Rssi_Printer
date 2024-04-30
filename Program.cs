using ManagedNativeWifi;
using System.Data;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

var availableNetworks = NativeWifi.EnumerateBssNetworks();
var enumInterfaces = NativeWifi.EnumerateInterfaces();
var enumInterfacesCon = NativeWifi.EnumerateInterfaceConnections();
string connectedBSSID = GetBSSID();

static string GetBSSID()
{
    Regex bssidFilter = new Regex(@"(..:..:..:..:..:..)");
    Process netsh = new Process();
	netsh.StartInfo.CreateNoWindow= true;
	netsh.StartInfo.FileName = "cmd";

	netsh.StartInfo.Arguments = @"/C ""netsh wlan show interfaces mode=bssid | findstr BSSID """;
	netsh.StartInfo.RedirectStandardOutput = true;
	netsh.StartInfo.UseShellExecute = false;
	netsh.Start();
	string output = netsh.StandardOutput.ReadToEnd();
	netsh.WaitForExit();
    var match = bssidFilter.Match(output);
	if (match.Success)
	{
        return match.Value.ToUpper().ToString();
	}
	else
		return "Fehler bei BSSID Filter";
}
static int GetRssi(IEnumerable<BssNetworkPack> availableNetworks, string bssid)
{
	int rssi = 0;
    foreach (var i in availableNetworks)
    {
		if(i.Bssid.ToString().Equals(bssid))
		{
			rssi = i.SignalStrength;
		}
    }
	return rssi;
}
static void GetEnumNetworks(IEnumerable<BssNetworkPack> bssNetworks)
{
	foreach (var i in bssNetworks)
	{
		Console.WriteLine($"SSID={i.Ssid}");
        Console.WriteLine($"BSSID{i.Bssid}");
		Console.WriteLine($"RSSI={i.SignalStrength}");
		Console.WriteLine($"INTERFACE={i.Interface.Description}");
		Console.WriteLine($"CHANNEL={i.Channel}");
		Console.WriteLine($"BAND={i.Band}");
		Console.WriteLine($"FREQUENCY={i.Frequency}");
		Console.WriteLine();
    }
}
static void GetInterfaces(IEnumerable<InterfaceInfo> enumInterfaces)
{
	foreach (var i in enumInterfaces)
	{
		Console.WriteLine($"ID={i.Id}");
		Console.WriteLine($"STATE={i.State}");
		Console.WriteLine($"DESCRIPTION={i.Description}");
	}
}
static void GetInterfacesCon(IEnumerable<InterfaceConnectionInfo> enumInterfacesCon)
{
	foreach (var i in enumInterfacesCon)
	{
        Console.WriteLine($"ID={i.Id}");
        Console.WriteLine($"PROFILENAME={i.ProfileName}");
        Console.WriteLine($"DESCRIPTION={i.Description}");
    }
}
static void PrintHelp()
{
	Console.WriteLine("Use one of the following parameters e.g. rssi-printer.exe -rssi");
    Console.WriteLine("-rssi\t\tshows rssi value");
    Console.WriteLine("-interfaces\tshows interface information");
    Console.WriteLine("-interfacescon\tshows interface connection information");
    Console.WriteLine("-networks\tshows available network information");
}
static void RssiDebugLoop(IEnumerable<BssNetworkPack> availableNetworks, string connectedBSSID)
{
    while (true)
    {
        Console.WriteLine(GetRssi(availableNetworks, connectedBSSID));
        Thread.Sleep(1000);
    }
}

if (args.Length != 0)
{
	if (args[0] == "-rssi")
	{
		Console.Write(GetRssi(availableNetworks, connectedBSSID));
	}

	if (args[0] == "-interfaces")
	{
		GetInterfaces(enumInterfaces);
	}

	if (args[0] == "-interfacescon")
	{
		GetInterfacesCon(enumInterfacesCon);
	}

	if (args[0] == "-networks")
	{
		GetEnumNetworks(availableNetworks);
	}
	if (args[0] == "-h")
	{
		PrintHelp();
	}
}
else
	PrintHelp();

//Debbuging 
//RssiDebugLoop(availableNetworks, connectedBSSID);