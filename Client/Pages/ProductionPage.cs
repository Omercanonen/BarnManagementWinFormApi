using Business.Constants;
using Business.DTOs;


namespace Client.Pages
{
    public partial class ProductionPage : UserControl
    {
        private readonly IProductionApiService _productionApi;

        private readonly System.Windows.Forms.Timer _uiTimer;
        private int _secondsElapsed = 0;

        private List<AccumulatedProductDto> _currentPendingList = new();

        public ProductionPage(IProductionApiService productionApi)
        {
            InitializeComponent();
            _productionApi = productionApi;

            if (progressBarProduction != null)
            {
                progressBarProduction.Minimum = 0;
                progressBarProduction.Maximum = 10;
                progressBarProduction.Value = 0;
            }

            _uiTimer = new System.Windows.Forms.Timer();
            _uiTimer.Interval = 1000;
            _uiTimer.Tick += UiTimer_Tick;

            buttonCollect.Click += buttonCollect_Click;

            this.Load += ProductionPage_Load;
        }

        private async void ProductionPage_Load(object? sender, EventArgs e)
        {
            await RefreshAnimalsAsync();
            await RefreshReadyToCollectAsync();

            _uiTimer.Start();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);
            if (this.Visible && !this.DesignMode)
                _uiTimer.Start();
            else
                _uiTimer.Stop();
        }

        private async void UiTimer_Tick(object? sender, EventArgs e)
        {
            _secondsElapsed++;

            if (progressBarProduction != null)
            {
                if (_secondsElapsed <= progressBarProduction.Maximum)
                    progressBarProduction.Value = _secondsElapsed;
            }

            if (_secondsElapsed >= 10)
            {
                _secondsElapsed = 0;
                if (progressBarProduction != null) progressBarProduction.Value = 0;

                await RefreshReadyToCollectAsync();
            }
        }

        private async Task RefreshAnimalsAsync()
        {
            try
            {
 
                var list = await _productionApi.GetPotentialAsync();

                listBoxAnimals.BeginUpdate();
                listBoxAnimals.Items.Clear();
                foreach (var item in list)
                {
                    listBoxAnimals.Items.Add($"{item.SpeciesName},{item.Count}");
                }
                listBoxAnimals.EndUpdate();
            }
            catch (Exception ex)
            {
            }
        }

        private async Task RefreshReadyToCollectAsync()
        {
            try
            {
                _currentPendingList = await _productionApi.GetPendingAsync();

                listBoxReadyToCollect.BeginUpdate();
                listBoxReadyToCollect.Items.Clear();

                foreach (var p in _currentPendingList)
                {
                    listBoxReadyToCollect.Items.Add($"{p.ProductName}: {p.TotalQuantity}");
                }

                listBoxReadyToCollect.EndUpdate();

                buttonCollect.Enabled = _currentPendingList.Count > 0;
            }
            catch (Exception ex)
            {

            }
        }

        private async void buttonCollect_Click(object? sender, EventArgs e)
        {
            if (_currentPendingList.Count == 0) return;

            buttonCollect.Enabled = false;
            this.Cursor = Cursors.WaitCursor;

            try
            {
                await _productionApi.CollectAsync(_currentPendingList);

                MessageBox.Show(Messages.Info.ProductionCollectCompleted, Messages.Titles.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);

                _currentPendingList.Clear();
                listBoxReadyToCollect.Items.Clear();
                await RefreshReadyToCollectAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Messages.Error.ProductionCollectFailed}\n{ex.Message}", Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                buttonCollect.Enabled = true;
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }
    }
}