using Business.Abstract.ApiServices;
using Business.Constants;
using Business.DTOs;

namespace Client.Pages
{
    public partial class PurchasePage : UserControl
    {
        public event EventHandler? OnPurchaseCompleted;
        public event EventHandler? OnBalanceChanged;

        private readonly IAnimalApiService _animalApi;

        private List<AnimalSpeciesDto> _speciesList = new();

        public PurchasePage(IServiceProvider serviceProvider, IAnimalApiService animalApi)
        {
            InitializeComponent();
            _animalApi = animalApi;

            comboBoxAnimalSpecies.SelectedIndexChanged += comboBoxAnimalSpecies_SelectedIndexChanged;
            this.Load += PurchasePage_Load;
        }

        private async void PurchasePage_Load(object? sender, EventArgs e)
        {
            await LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            try
            {
                _speciesList = await _animalApi.GetSpeciesAsync();

                comboBoxAnimalSpecies.DataSource = null;
                comboBoxAnimalSpecies.DisplayMember = "Name";
                comboBoxAnimalSpecies.ValueMember = "Id";
                comboBoxAnimalSpecies.DataSource = _speciesList;
                comboBoxAnimalSpecies.SelectedIndex = -1;

                labelPrice.Text = "0.00";

                comboBoxAnimalGender.Items.Clear();
                comboBoxAnimalGender.Items.Add("Male");
                comboBoxAnimalGender.Items.Add("Female");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{Messages.Error.DataLoadError} {ex.Message}", Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void comboBoxAnimalSpecies_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxAnimalSpecies.SelectedItem is AnimalSpeciesDto selected)
            {
                labelPrice.Text = selected.Price.ToString("C2");
            }
            else
            {
                labelPrice.Text = "0.00";
            }
        }

        private async void buttonAnimalPurchase_Click(object sender, EventArgs e)
        {
            if (comboBoxAnimalSpecies.SelectedIndex == -1 || comboBoxAnimalGender.SelectedIndex == -1)
            {
                MessageBox.Show(Messages.Error.InvalidInput, Messages.Titles.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(textBoxName.Text))
            {
                MessageBox.Show(Messages.Warning.EnterAnimalName, Messages.Titles.Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                this.Cursor = Cursors.WaitCursor;
                buttonAnimalPurchase.Enabled = false;

                var selectedSpecies = (AnimalSpeciesDto)comboBoxAnimalSpecies.SelectedItem;

                var purchaseDto = new PurchaseAnimalDto
                {
                    AnimalName = textBoxName.Text.Trim(),
                    AnimalGender = comboBoxAnimalGender.SelectedItem.ToString()!,
                    AnimalSpeciesId = selectedSpecies.Id,
                    BarnId = 0
                };

                await _animalApi.PurchaseAnimalAsync(purchaseDto);
                OnBalanceChanged?.Invoke(this, EventArgs.Empty);

                MessageBox.Show(Messages.Info.AnimalsPurchased, Messages.Titles.Success, MessageBoxButtons.OK, MessageBoxIcon.Information);

                textBoxName.Clear();
                comboBoxAnimalSpecies.SelectedIndex = -1;
                comboBoxAnimalGender.SelectedIndex = -1;
                labelPrice.Text = "0.00";

                OnPurchaseCompleted?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {

                MessageBox.Show($"{Messages.Error.PurchaseFailed}:\n{ex.Message}", Messages.Titles.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                this.Cursor = Cursors.Default;
                buttonAnimalPurchase.Enabled = true;
            }
        }
    }
}

