using Business.Abstract.ApiServices;
using Business.Constants;
using Business.DTOs;
using Business.Security;
using Client.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Client
{
    public partial class MainForm : Form
    {
        private readonly IServiceProvider _sp;
        private readonly IUserApiService _userApi;
        private readonly IBarnApiService _barnApi;
        private readonly ISessionContext _session;

        private UserMeResponseDto? _me;
        private MyBarnResponseDto? _myBarn;

        public MainForm(IServiceProvider sp, IUserApiService userApi, IBarnApiService barnApi, ISessionContext session)
        {
            InitializeComponent();

            _sp = sp;
            _userApi = userApi;
            _barnApi = barnApi;
            _session = session;

            this.Shown += MainForm_Shown;
        }

        public void SetSession(ISessionContext session)
        {
            labelUserName.Text = $"User: {session.UserName}";
        }

        private async void MainForm_Shown(object? sender, EventArgs e)
        {
            try
            {
                await LoadSessionDataAsync();

                labelUserName.Text = $"User: {_me?.UserName ?? _session.UserName}";
                labelBarnName.Text = _myBarn?.BarnName ?? "No Barn";
                labelBalance.Text = _myBarn != null
                    ? (_myBarn.BarnBalance ?? 0).ToString("C2")
                    : "0.00";


                ApplyAuthorizationToUi();

                if (_myBarn == null)
                {

                    return;
                }

                LoadPage<HomePage>();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"MainForm API Error\n\n{ex.Message}",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }


        public async Task RefreshBalanceAsync()
        {
            try
            {
                _myBarn = await _barnApi.MyAsync();

                labelBalance.Text = _myBarn != null
                    ? (_myBarn.BarnBalance ?? 0).ToString("C2")
                    : "0.00";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Messages.Error.BalanceRefreshFailed}: {ex.Message}");
            }
        }

        private void ApplyAuthorizationToUi()
        {

            var role = _me?.Role ?? "";
            bool isAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase);

            if (buttonAddUser != null)
                buttonAddUser.Visible = isAdmin;


            buttonAddUser.Enabled = isAdmin;
        }

        private async Task LoadSessionDataAsync(CancellationToken ct = default)
        {
            _me = await _userApi.MeAsync(ct);

            try
            {
                _myBarn = await _barnApi.MyAsync(ct);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Barn/My ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _myBarn = null;
            }
        }

        private void LoadPage<T>() where T : UserControl
        {
            try
            {
                if (panelContent.Controls.Count > 0)
                {
                    panelContent.Controls[0].Dispose();
                    panelContent.Controls.Clear();
                }

                var page = _sp.GetRequiredService<T>();

                if (page is InventoryPage inventoryPage)
                {
                    inventoryPage.OnBalanceChanged += async (s, e) => await RefreshBalanceAsync();
                }

                if (page is PurchasePage purchasePage)
                {
                    purchasePage.OnBalanceChanged += async (s, e) => await RefreshBalanceAsync();
                }

                if (page is PurchaseWorker workerPage)
                {
                    workerPage.OnBalanceChanged += async (s, e) => await RefreshBalanceAsync();
                }

                page.Dock = DockStyle.Fill;
                page.Visible = true;

                panelContent.Controls.Add(page);
                page.BringToFront();

                if (page is HomePage homePage)
                {
                    _ = homePage.LoadAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"PageLoadError\n\n{ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonHome_Click_1(object sender, EventArgs e) => LoadPage<HomePage>();
        private void buttonAddUser_Click(object sender, EventArgs e) => LoadPage<AddUserForm>();
        private void buttonInventory_Click_1(object sender, EventArgs e) => LoadPage<InventoryPage>();
        private void buttonProduction_Click_1(object sender, EventArgs e) => LoadPage<ProductionPage>();
        private void buttonPurchase_Click_1(object sender, EventArgs e) => LoadPage<PurchasePage>();
        private void buttonWorker_Click(object sender, EventArgs e) => LoadPage<PurchaseWorker>();
     
    }
}