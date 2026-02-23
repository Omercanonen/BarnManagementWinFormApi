using Business.Abstract.ApiServices;
using Business.Constants;
using Business.DTOs;
using System.Text;

namespace Client.Pages
{
    public partial class PurchaseWorker : UserControl
    {
        private readonly IWorkerApiService _workerApi;
        private WorkerDto? _selectedWorker;

        public event EventHandler? OnBalanceChanged;

        private const decimal WORKER_PRICE = 1000m;

        public PurchaseWorker(IWorkerApiService workerApi)
        {
            InitializeComponent();
            _workerApi = workerApi;

            labelWorkerPrice.Text = $"Price: {WORKER_PRICE:C2}";
            ResetDetailPanel();

            listBoxWorkers.SelectedIndexChanged += ListBoxWorkers_SelectedIndexChanged;

            this.Load += async (s, e) => await LoadWorkersAsync();
        }

        private async Task LoadWorkersAsync()
        {
            try
            {
                var list = await _workerApi.GetListAsync();

                listBoxWorkers.BeginUpdate();
                listBoxWorkers.DataSource = null;
                listBoxWorkers.Items.Clear();

                var displayList = list.Select(w => new WorkerListItem(w)).ToList();

                listBoxWorkers.DataSource = displayList;
                listBoxWorkers.EndUpdate();

                if (!list.Any())
                {
                    ResetDetailPanel();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Messages.Error.WorkerLoadFailed}: {ex.Message}", Messages.Titles.Error, MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }

        private void ListBoxWorkers_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listBoxWorkers.SelectedItem is WorkerListItem item)
            {
                _selectedWorker = item.Dto;
                UpdateDetailPanel();
            }
            else
            {
                ResetDetailPanel();
            }
        }

        private void UpdateDetailPanel()
        {
            if (_selectedWorker == null) return;

            var sb = new StringBuilder();
            sb.AppendLine($"Level: {_selectedWorker.Level}");
            sb.AppendLine($"Capacity: {_selectedWorker.Capacity} units");
            sb.AppendLine($"Speed: {_selectedWorker.IntervalSeconds} sec");

            labelInfo.Text = sb.ToString();

            if (_selectedWorker.CanUpgrade)
            {
                labelUpgrade.Text = _selectedWorker.UpgradeCost.ToString("C2");
                buttonUpgrade.Text = $"Upgrade ({_selectedWorker.UpgradeCost:C2})";
                buttonUpgrade.Enabled = true;
            }
            else
            {
                buttonUpgrade.Text = "Max level";
                buttonUpgrade.Enabled = false;
            }

            buttonSell.Text = $"Sell ({_selectedWorker.SellPrice:C2})";
            buttonSell.Enabled = true;
        }

        private void ResetDetailPanel()
        {
            _selectedWorker = null;
            labelInfo.Text = "Select a worker for details.";
            buttonUpgrade.Text = "Upgrade";
            buttonUpgrade.Enabled = false;
            buttonSell.Text = "Sell";
            buttonSell.Enabled = false;
        }


        private async void buttonBuyWorker_Click(object? sender, EventArgs e)
        {
            try
            {
                buttonBuyWorker.Enabled = false;
                this.Cursor = Cursors.WaitCursor;

                await _workerApi.BuyAsync();

                MessageBox.Show(Messages.Info.WorkerPurchased, Messages.Titles.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadWorkersAsync();
                OnBalanceChanged?.Invoke(this, EventArgs.Empty); // Bakiyeyi güncelle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Messages.Error.WorkerPurchaseFailed}:\n{ex.Message}",Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                buttonBuyWorker.Enabled = true;
                this.Cursor = Cursors.Default;
            }
        }

        private async void buttonUpgrade_Click(object? sender, EventArgs e)
        {
            if (_selectedWorker == null) return;

            try
            {
                this.Cursor = Cursors.WaitCursor;

                await _workerApi.UpgradeAsync(_selectedWorker.Id);

                MessageBox.Show(Messages.Info.WorkerUpgraded, Messages.Titles.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadWorkersAsync();
                OnBalanceChanged?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Messages.Error.WorkerUpgradeFailed,Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void buttonSell_Click(object? sender, EventArgs e)
        {
            if (_selectedWorker == null) return;

            var result = MessageBox.Show(
                $"Are you sure to sell this worker?\nRefund: {_selectedWorker.SellPrice:C2}",
                "Okey",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    this.Cursor = Cursors.WaitCursor;

                    await _workerApi.SellAsync(_selectedWorker.Id);

                    await LoadWorkersAsync();
                    OnBalanceChanged?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"{Messages.Error.WorkerSellFailed}:\n{ex.Message}", Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    this.Cursor = Cursors.Default;
                }
            }
        }

        private class WorkerListItem
        {
            public WorkerDto Dto { get; }

            public WorkerListItem(WorkerDto dto)
            {
                Dto = dto;
            }

            public override string ToString()
            {
                return $"Worker #{Dto.Id} - Lv.{Dto.Level} (Speed: {Dto.IntervalSeconds}s)";
            }
        }
    }
}