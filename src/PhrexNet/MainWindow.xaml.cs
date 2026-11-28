using System.Windows.Controls;
using PhrexNet.Core;
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
        AdapterPanel.Visibility = module == "Network Discovery" ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        ScopePanel.Visibility = AdapterPanel.Visibility;
        PageTitle.Text = module;
        PageDescription.Text = module == "Command Center"
            ? "Welcome to the local-first infrastructure workspace. Configure an authorized scope before future discovery operations."
            : module == "Network Discovery" ? "Read local adapter configuration without sending probes. Active discovery is not yet available." : $"{module} is planned. Its operational capabilities are not implemented in this preview.";
    }
    private void PreviewScope(object sender, System.Windows.RoutedEventArgs e)
    {
        ScopeResults.Text = "";
        try
        {
            var targets = ScopePreview.Build(ScopeCidr.Text, ScopeExclusions.Text, ScopeConfirmed.IsChecked == true);
            ScopeResults.Text = $"{targets.Count} planned targets (offline preview only)\n" + string.Join(", ", targets);
        }
        catch (ArgumentException ex) { ScopeResults.Text = ex.Message; }
        catch (InvalidOperationException ex) { ScopeResults.Text = ex.Message; }
        finally { ScopeConfirmed.IsChecked = false; }
    }
    private async void RefreshAdapters(object sender, System.Windows.RoutedEventArgs e)
    {
        if (sender is not Button button) return;
        button.IsEnabled = false;
        AdapterResults.Text = "Reading local configuration...";
        try
        {
            var rows = await Task.Run(() => new AdapterInventory(new LocalAdapterSource()).Read());
            AdapterResults.Text = rows.Count == 0 ? "No adapters reported." : string.Join("\n\n", rows.Select(AdapterInventory.Describe));
        }
        catch (Exception) { AdapterResults.Text = "Local adapter configuration could not be read. Retry or check OS permissions."; }
        finally { button.IsEnabled = true; }
    }
}
