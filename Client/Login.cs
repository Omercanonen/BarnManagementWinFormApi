using Business.Abstract.ApiServices;
using Business.Constants;
using Business.DTOs.Auth;
using Business.Security;
using Core.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Client
{
    public partial class Login : Form
    {
        private readonly IAuthApiService _authApi;
        private readonly ISessionContext _session;
        private readonly ILoggerService _logger;
        private readonly IServiceProvider _serviceProvider;

        public Login(
            IAuthApiService authApi,
            ISessionContext session,
            ILoggerService logger,
            IServiceProvider serviceProvider)
        {
            _authApi = authApi;
            _session = session;
            _logger = logger;
            _serviceProvider = serviceProvider;

            InitializeComponent();
            this.AcceptButton = buttonLogin;
        }

        public async void buttonLogin_Click(object sender, EventArgs e)
        {
            buttonLogin.Enabled = false;
            buttonLogin.Text = "Logging";

            string username = textBoxUsername.Text.Trim();
            string password = textBoxPassword.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show(Messages.Error.InvalidInput, Messages.Titles.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                buttonLogin.Enabled = true;
                buttonLogin.Text = "Login";
                return;
            }

            try
            {
                var dto = new LoginRequestDto
                {
                    UserName = username,
                    Password = password
                };

                var result = await _authApi.LoginAsync(dto);

                _session.AccessToken = result.AccessToken;
                _session.Expiration = result.Expiration;
                _session.UserName = result.UserName;

                _logger.LogInfo($"User '{result.UserName}' logged in successfully via API.");

                var mainForm = _serviceProvider.GetRequiredService<MainForm>();
                mainForm.SetSession(_session);

                this.Hide();
                mainForm.ShowDialog();
                this.Close();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred during API login.", ex);
                MessageBox.Show($"{Messages.Error.LoginFailed}\n\n{ex.Message}",
                    Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonLogin.Enabled = true;
                buttonLogin.Text = "Login";
            }
        }
    }
}
