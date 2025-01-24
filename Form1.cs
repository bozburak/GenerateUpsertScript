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

            var headers = csvLines[0].Split(',');
            var mergeScripts = new List<string>();

            int totalLines = csvLines.Length - 1; // Baþlýk satýrýný çýkar
            int processedLines = 0;

            foreach (var line in csvLines.Skip(1))
            {
                var values = line.Split(',').Select(v => v.Trim('"')).ToArray();
                var primaryKeyValue = values[0];

                var updateColumns = headers
                    .Skip(1)
                    .Select((header, index) => $"{header} = '{values[index + 1]}'")
                    .ToArray();

                var insertColumns = string.Join(", ", headers);
                var insertValues = string.Join(", ", values.Select(v => $"'{v}'"));

                string mergeScript = $@"
MERGE {targetTableName} AS target
USING (SELECT {primaryKeyValue} AS {primaryKeyColumn}) AS source
ON (target.{primaryKeyColumn} = source.{primaryKeyColumn})
WHEN MATCHED THEN
    UPDATE SET {string.Join(", ", updateColumns)}
WHEN NOT MATCHED THEN
    INSERT ({insertColumns})
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
    }
}