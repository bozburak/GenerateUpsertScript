using MaterialSkin;
using MaterialSkin.Controls;

namespace GenerateUpsertScript
{
    public partial class BOZUPSERTTOOL : MaterialForm
    {
        private string selectedFilePath;

        public BOZUPSERTTOOL()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Primary.BlueGrey800, Primary.BlueGrey900, Primary.BlueGrey500, Accent.LightBlue200, TextShade.WHITE);
        }

        private void materialButton1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Filter = "CSV Dosyalarý (*.csv)|*.csv";
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    selectedFilePath = openFileDialog.FileName;
                }
            }
        }

        private async void materialButton2_Click(object sender, EventArgs e)
        {
            // Formu devre dýþý býrak
            this.Enabled = false;

            var csvLines = File.ReadAllLines(selectedFilePath);
            string targetTableName = materialTextBox21.Text;
            string primaryKeyColumn = materialTextBox22.Text;

            var headers = csvLines[0].Split(';').Select(h => h.Trim().ToLowerInvariant()).ToArray();
            var mergeScripts = new List<string>();

            int totalLines = csvLines.Length - 1; // Baþlýk satýrýný çýkar
            int processedLines = 0;

            // primaryKeyColumn'un headers dizisindeki indeksini bul
            int primaryKeyIndex = Array.FindIndex(headers, header => header.Equals(primaryKeyColumn, StringComparison.OrdinalIgnoreCase));

            foreach (var line in csvLines.Skip(1))
            {
                var values = line.Split(';').Select(v => v.Trim('"')).ToArray();
                var primaryKeyValue = values[primaryKeyIndex];

                var insertValues = string.Join(", ", values.Select(v => IsNumeric(v) || v.Equals("NULL", StringComparison.OrdinalIgnoreCase) ? v : $"'{v}'"));
                var updateSet = string.Join(", ", headers.Select((h, i) => $"{h} = {(IsNumeric(values[i]) || values[i].Equals("NULL", StringComparison.OrdinalIgnoreCase) ? values[i] : $"'{values[i]}'")}"));

                string mergeScript = $@"
MERGE {targetTableName} AS target
USING (SELECT {(IsNumeric(primaryKeyValue) ? primaryKeyValue : $"'{primaryKeyValue}'")} AS {primaryKeyColumn}) AS source
ON (target.{primaryKeyColumn} = source.{primaryKeyColumn})
WHEN MATCHED THEN
    UPDATE SET {updateSet}
WHEN NOT MATCHED THEN
    INSERT ({string.Join(", ", headers)})
    VALUES ({insertValues});
";
                mergeScripts.Add(mergeScript);

                // Ýþlem ilerlemesini güncelle
                processedLines++;
                int progressPercentage = (processedLines * 100) / totalLines;
                materialProgressBar1.Value = progressPercentage;
                materialLabel4.Text = $"%{progressPercentage}";

                // UI güncellemeleri için kýsa bir gecikme ekle
                await Task.Delay(10);
            }

            string allScripts = string.Join(Environment.NewLine, mergeScripts);
            Clipboard.SetText(allScripts);
            MessageBox.Show("Script Baþarýyla Kopyalandý");

            // Formu tekrar etkinleþtir
            this.Enabled = true;
        }

        private bool IsNumeric(string value)
        {
            return long.TryParse(value, out _) || int.TryParse(value, out _) || decimal.TryParse(value, out _);
        }
    }
}