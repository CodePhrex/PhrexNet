using System.Windows.Controls;
namespace PhrexNet;
public partial class MainWindow : System.Windows.Window
{
    private static readonly string[] Modules = ["Command Center", "Network Discovery", "Device Intelligence", "Port Scanner", "Network Topology", "Firewall Intelligence", "Vulnerability Lab", "Remote Access Center", "Packet Analyzer", "Network Monitoring", "DNS Intelligence", "DHCP Intelligence", "Active Directory", "Wireless Lab", "SNMP Explorer", "IP Address Management", "Configuration Manager", "Asset Management", "Reporting Center", "Network AI"];
    public MainWindow()
    {
        InitializeComponent();
        Navigation.ItemsSource = Modules;
        Navigation.SelectedIndex = 0;
    }
    private void Navigate(object sender, SelectionChangedEventArgs e)
    {
        if (Navigation.SelectedItem is not string module || PageTitle is null || PageDescription is null) return;
        PageTitle.Text = module;
        PageDescription.Text = module == "Command Center"
            ? "Welcome to the local-first infrastructure workspace. Configure an authorized scope before future discovery operations."
            : $"{module} is planned. Its operational capabilities are not implemented in this preview.";
    }
}
