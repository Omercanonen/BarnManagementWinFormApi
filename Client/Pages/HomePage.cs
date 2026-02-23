using Business.Abstract.ApiServices;

namespace Client.Pages
{
    public partial class HomePage : UserControl
    {
        private readonly IUserApiService _userApi;
        private readonly IBarnApiService _barnApi;
        private readonly IAnimalApiService _animalApi;
        public HomePage(IUserApiService userApi, IBarnApiService barnApi, IAnimalApiService animalApi)
        {
            _userApi = userApi;
            _barnApi = barnApi;
            _animalApi = animalApi;

            InitializeComponent();

        }

        public async Task LoadAsync()
        {
            dataGridViewHome.Visible = true;

            try
            {
                var me = await _userApi.MeAsync();

                if (string.Equals(me.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    await LoadAdminDashboardAsync();
                }
                else
                {
                    await LoadUserDashboardAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"HomePage load error: {ex.Message}");
            }
        }

        private async Task LoadAdminDashboardAsync()
        {
            var activeBarns = await _barnApi.GetAllActiveAsync();
            dataGridViewHome.DataSource = activeBarns;
        }

        private async Task LoadUserDashboardAsync()
        {
            var myAnimals = await _animalApi.GetMyAnimalsAsync();
            dataGridViewHome.DataSource = myAnimals;
        }
    }
}