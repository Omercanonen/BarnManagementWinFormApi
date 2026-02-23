using Business.Abstract.ApiServices;
using Business.Constants;
using Business.DTOs;
using System.Text;

namespace Client.Pages
{
    public partial class InventoryPage : UserControl
    {
        private readonly IInventoryApiService _inventoryApi;
        public event EventHandler? OnBalanceChanged;

        private int _selectedProductId = 0;
        private decimal _selectedUnitPrice = 0m;
        private int _selectedStock = 0;

        public InventoryPage(IInventoryApiService inventoryApi)
        {
            InitializeComponent();
            _inventoryApi = inventoryApi;

            //listBoxInventory.SelectedIndexChanged += listBoxInventory_SelectedIndexChanged;
            //numericUpDownSellQuantity.ValueChanged += numericUpDownSellQuantity_ValueChanged;
            //buttonSell.Click += buttonSell_Click;
            //buttonSellAll.Click += buttonSellAll_Click; 
            //buttonExport.Click += buttonExport_ClickAsync;

            ResetSellPanel();

            this.Load += async (s, e) => await LoadInventoryAsync();
        }

        private async Task LoadInventoryAsync()
        {
            try
            {
                var items = await _inventoryApi.GetInventoryAsync();

                listBoxInventory.BeginUpdate();
                listBoxInventory.Items.Clear();

                foreach (var i in items)
                {
                    listBoxInventory.Items.Add(new InventoryListItem(i));
                }
                listBoxInventory.EndUpdate();

                if (listBoxInventory.Items.Count == 0)
                {
                    ResetSellPanel();
                }
                else
                {
                    listBoxInventory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show( $"{Messages.Error.DataLoadError}\n{ex.Message}", Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void listBoxInventory_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (listBoxInventory.SelectedItem is not InventoryListItem selected)
            {
                ResetSellPanel();
                return;
            }

            _selectedProductId = selected.Dto.ProductId;
            _selectedUnitPrice = selected.Dto.UnitPrice;
            _selectedStock = selected.Dto.StockQuantity;

            labelUnitPrice.Text = _selectedUnitPrice.ToString("C2");
            labelStock.Text = _selectedStock.ToString();

            numericUpDownSellQuantity.Minimum = 1;
            numericUpDownSellQuantity.Maximum = _selectedStock > 0 ? _selectedStock : 1;
            numericUpDownSellQuantity.Value = 1;

            UpdateTotalPriceLabel();

            buttonSell.Enabled = _selectedStock > 0;
            // buttonSellAll.Enabled = _selectedStock > 0;
        }

        private void numericUpDownSellQuantity_ValueChanged(object? sender, EventArgs e)
        {
            UpdateTotalPriceLabel();
        }

        private void UpdateTotalPriceLabel()
        {
            int qty = (int)numericUpDownSellQuantity.Value;
            decimal total = _selectedUnitPrice * qty;
            labelTotalPrice.Text = total.ToString("C2");
        }

        private async void buttonSell_Click(object? sender, EventArgs e)
        {
            if (_selectedProductId <= 0) return;

            int qty = (int)numericUpDownSellQuantity.Value;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                buttonSell.Enabled = false;

                await _inventoryApi.SellAsync(_selectedProductId, qty);
                OnBalanceChanged?.Invoke(this, EventArgs.Empty);

                MessageBox.Show( Messages.Info.SaleSuccess, Messages.Titles.Info, MessageBoxButtons.OK, MessageBoxIcon.Information);

                await LoadInventoryAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Messages.Error.SellFailed}\n{ex.Message}", Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                buttonSell.Enabled = true;
            }
        }

        private async void buttonSellAll_Click(object? sender, EventArgs e)
        {
            if (_selectedProductId <= 0) return;

            if ((MessageBox.Show(Messages.Warning.AreYouSure, Messages.Titles.Warning, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes))
                return;

            try
            {
                this.Cursor = Cursors.WaitCursor;
                await _inventoryApi.SellAllAsync(_selectedProductId);

                OnBalanceChanged?.Invoke(this, EventArgs.Empty);

                MessageBox.Show(Messages.Info.AllStockSold, Messages.Titles.Info, MessageBoxButtons.OK, MessageBoxIcon.Information);
                await LoadInventoryAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Messages.Error.SellFailed}\n{ex.Message}",Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private async void buttonExport_ClickAsync(object? sender, EventArgs e)
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;

                string json = await _inventoryApi.ExportHistoryAsync();

                using var sfd = new SaveFileDialog
                {
                    Title = Messages.Info.ExportSalesTitle,
                    Filter = "JSON File (*.json)|*.json",   
                    FileName = $"SalesHistory_{DateTime.Now:yyyyMMdd_HHmm}.json"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    await File.WriteAllTextAsync(sfd.FileName, json, Encoding.UTF8);

                    MessageBox.Show(Messages.Info.SalesExportSuccess, Messages.Titles.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Messages.Error.ExportFailed}\n{ex.Message}", Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
            }
        }

        private void ResetSellPanel()
        {
            _selectedProductId = 0;
            labelUnitPrice.Text = "0.00";
            labelStock.Text = "0";
            labelTotalPrice.Text = "0.00";
            numericUpDownSellQuantity.Value = 1;
            buttonSell.Enabled = false;
        }

        private sealed class InventoryListItem
        {
            public InventoryItemDto Dto { get; }
            public InventoryListItem(InventoryItemDto dto) => Dto = dto;
            public override string ToString() => $"{Dto.ProductName} (Stok: {Dto.StockQuantity})";
        }
    }
}