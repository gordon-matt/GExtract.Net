namespace GExtract;

public partial class MainForm : Form
{
    private NesRom? _rom;
    private string? _loadedPath;
    private bool _outputIsAutomatic = true;
    private bool _suppressOutputEvent;
    private bool _extracting;
    private bool _closeAfterCancel;
    private CancellationTokenSource? _cancellation;

    public MainForm()
    {
        InitializeComponent();
        UpdateDetails();
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        if (_extracting)
        {
            e.Cancel = true;
            _closeAfterCancel = true;
            _cancellation?.Cancel();
            statusLabel.Text = "Cancelling...";
            return;
        }

        base.OnFormClosing(e);
    }

    private void browseRomButton_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Open NES ROM",
            Filter = "NES ROMs (*.nes)|*.nes|All files (*.*)|*.*",
            CheckFileExists = true
        };

        var existingDirectory = Path.GetDirectoryName(romPathTextBox.Text.Trim());
        if (!string.IsNullOrEmpty(existingDirectory) && Directory.Exists(existingDirectory))
            dialog.InitialDirectory = existingDirectory;

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        romPathTextBox.Text = dialog.FileName;
        TryLoadRom(showErrors: true);
    }

    private void browseOutputButton_Click(object sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Choose the folder for bitmap files",
            UseDescriptionForTitle = true
        };

        if (Directory.Exists(outputTextBox.Text.Trim()))
            dialog.SelectedPath = outputTextBox.Text.Trim();

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        SetOutputPath(dialog.SelectedPath, automatic: false);
    }

    private void romPathTextBox_TextChanged(object sender, EventArgs e)
    {
        var path = romPathTextBox.Text.Trim();
        if (!string.Equals(path, _loadedPath, StringComparison.OrdinalIgnoreCase))
        {
            _rom = null;
            _loadedPath = null;
            UpdateDetails();
        }
    }

    private void romPathTextBox_Leave(object sender, EventArgs e)
    {
        TryLoadRom(showErrors: false);
    }

    private void outputTextBox_TextChanged(object sender, EventArgs e)
    {
        if (!_suppressOutputEvent)
            _outputIsAutomatic = false;

        UpdateExtractEnabled();
    }

    private void extractionMode_CheckedChanged(object sender, EventArgs e)
    {
        UpdateExtractEnabled();
    }

    private void MainForm_DragEnter(object sender, DragEventArgs e)
    {
        bool hasFile = e.Data?.GetDataPresent(DataFormats.FileDrop) == true;
        e.Effect = hasFile ? DragDropEffects.Copy : DragDropEffects.None;
    }

    private void MainForm_DragDrop(object sender, DragEventArgs e)
    {
        if (_extracting)
            return;

        if (e.Data?.GetData(DataFormats.FileDrop) is not string[] files || files.Length == 0)
            return;

        romPathTextBox.Text = files[0];
        TryLoadRom(showErrors: true);
    }

    private bool TryLoadRom(bool showErrors)
    {
        var path = romPathTextBox.Text.Trim();
        if (path.Length == 0)
        {
            _rom = null;
            _loadedPath = null;
            UpdateDetails();
            return false;
        }

        if (string.Equals(path, _loadedPath, StringComparison.OrdinalIgnoreCase) && _rom is not null)
            return true;

        if (!File.Exists(path))
        {
            _rom = null;
            _loadedPath = null;
            UpdateDetails();
            if (showErrors)
                ShowError($"Couldn't find {path}");

            return false;
        }

        try
        {
            _rom = NesRom.Load(path);
            _loadedPath = path;
            SuggestOutputFolder();
            UpdateDetails();
            return true;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            _rom = null;
            _loadedPath = null;
            UpdateDetails();
            if (showErrors)
                ShowError(ex.Message);

            return false;
        }
    }

    private void SuggestOutputFolder()
    {
        if (_rom is null)
            return;

        if (!_outputIsAutomatic && outputTextBox.Text.Trim().Length > 0)
            return;

        var directory = Path.GetDirectoryName(_rom.FilePath);
        if (string.IsNullOrEmpty(directory))
            return;

        var folderName = Path.GetFileNameWithoutExtension(_rom.FilePath) + " graphics";
        SetOutputPath(Path.Combine(directory, folderName), automatic: true);
    }

    private void SetOutputPath(string path, bool automatic)
    {
        _suppressOutputEvent = true;
        outputTextBox.Text = path;
        _outputIsAutomatic = automatic;
        _suppressOutputEvent = false;
        UpdateExtractEnabled();
    }

    private void UpdateDetails()
    {
        if (_rom is null)
        {
            detailsLabel.Text = string.IsNullOrWhiteSpace(romPathTextBox.Text)
                ? "No ROM loaded."
                : "ROM not loaded.";
        }
        else if (!_rom.HasInesHeader)
        {
            var name = Path.GetFileName(_rom.FilePath);
            detailsLabel.Text = _rom.TileCount > 0
                ? $"{name}{Environment.NewLine}No iNES header. The file can be read as {_rom.TileCount:N0} raw tiles."
                : $"{name}{Environment.NewLine}The file is smaller than one tile (16 bytes).";
        }
        else if (_rom.ChrTileCount > 0)
        {
            var trainer = _rom.HasTrainer ? " · trainer" : string.Empty;
            detailsLabel.Text =
                $"{Path.GetFileName(_rom.FilePath)}{Environment.NewLine}" +
                $"Mapper {_rom.Mapper} · PRG {FormatSize(_rom.PrgSize)} · CHR {FormatSize(_rom.ChrSize)}{trainer}{Environment.NewLine}" +
                $"CHR starts at 0x{_rom.ChrOffset:X6} · {_rom.ChrTileCount:N0} tiles";
        }
        else
        {
            detailsLabel.Text =
                $"{Path.GetFileName(_rom.FilePath)}{Environment.NewLine}" +
                $"Mapper {_rom.Mapper} · PRG {FormatSize(_rom.PrgSize)} · no CHR ROM{Environment.NewLine}" +
                "Use entire file to search PRG data for tiles.";
        }

        UpdateModeOptions();
        UpdateExtractEnabled();
    }

    private void UpdateModeOptions()
    {
        bool hasChr = _rom is { ChrTileCount: > 0 };
        bool hasTiles = _rom is { TileCount: > 0 };

        chrBanksRadioButton.Enabled = hasChr;
        allChrRadioButton.Enabled = hasChr;
        entireFileRadioButton.Enabled = hasTiles;

        if (hasChr && _rom is not null)
        {
            chrBanksRadioButton.Text = $"CHR banks - {_rom.ChrBankCount:N0} bitmap files";
            allChrRadioButton.Text = $"All CHR graphics - 1 bitmap, {_rom.ChrTileCount:N0} tiles";
        }
        else
        {
            chrBanksRadioButton.Text = "CHR banks";
            allChrRadioButton.Text = "All CHR graphics";
        }

        entireFileRadioButton.Text = hasTiles && _rom is not null
            ? $"Entire file - 1 bitmap, {_rom.TileCount:N0} tiles"
            : "Entire file";

        if (!hasChr && (chrBanksRadioButton.Checked || allChrRadioButton.Checked))
            entireFileRadioButton.Checked = hasTiles;
        else if (hasChr && !chrBanksRadioButton.Checked && !allChrRadioButton.Checked && !entireFileRadioButton.Checked)
            chrBanksRadioButton.Checked = true;
    }

    private void UpdateExtractEnabled()
    {
        if (_extracting)
        {
            extractButton.Enabled = true;
            return;
        }

        extractButton.Enabled = _rom is not null
            && SelectedMode() is ExtractionMode mode
            && ModeAvailable(mode)
            && outputTextBox.Text.Trim().Length > 0;

        UpdateReadyStatus();
    }

    private void UpdateReadyStatus()
    {
        if (_extracting)
            return;

        if (_rom is null)
        {
            statusLabel.Text = "Open a NES ROM.";
            return;
        }

        if (outputTextBox.Text.Trim().Length == 0)
        {
            statusLabel.Text = "Choose an output folder.";
            return;
        }

        if (SelectedMode() is not ExtractionMode mode || !ModeAvailable(mode))
        {
            statusLabel.Text = _rom.TileCount == 0
                ? "The file is smaller than one tile (16 bytes)."
                : "Choose what to extract.";
            return;
        }

        int fileCount = mode == ExtractionMode.ChrBanks ? _rom.ChrBankCount : 1;
        var noun = fileCount == 1 ? "bitmap" : "bitmaps";
        statusLabel.Text = $"Ready to save {fileCount:N0} {noun}.";
    }

    private ExtractionMode? SelectedMode()
    {
        if (chrBanksRadioButton.Checked)
            return ExtractionMode.ChrBanks;

        if (allChrRadioButton.Checked)
            return ExtractionMode.AllChr;

        if (entireFileRadioButton.Checked)
            return ExtractionMode.EntireFile;

        return null;
    }

    private bool ModeAvailable(ExtractionMode mode)
    {
        if (_rom is null)
            return false;

        return mode == ExtractionMode.EntireFile
            ? _rom.TileCount > 0
            : _rom.ChrTileCount > 0;
    }

    private async void extractButton_Click(object sender, EventArgs e)
    {
        if (_extracting)
        {
            _cancellation?.Cancel();
            statusLabel.Text = "Cancelling...";
            return;
        }

        if (!TryLoadRom(showErrors: true) || _rom is null || SelectedMode() is not ExtractionMode mode)
            return;

        string output;
        try
        {
            output = Path.GetFullPath(outputTextBox.Text.Trim());
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException)
        {
            ShowError("The output folder path is not valid.");
            return;
        }

        if (File.Exists(output))
        {
            ShowError("The output path is a file. Choose a folder.");
            return;
        }

        var rom = _rom;
        _cancellation = new CancellationTokenSource();
        var token = _cancellation.Token;
        var progress = new Progress<ExtractionProgress>(OnProgress);
        string? completionStatus = null;

        SetBusy(true);
        progressBar.Maximum = Math.Max(GraphicsExtractor.CountTiles(rom, mode), 1);
        progressBar.Value = 0;
        statusLabel.Text = "Extracting...";

        try
        {
            var files = await Task.Run(
                () => GraphicsExtractor.Extract(rom, output, mode, progress, token),
                token);

            SetProgress(progressBar.Maximum);
            var message = DescribeResult(files, rom, mode);
            completionStatus = message.Replace(Environment.NewLine, " ");
            ShowInfo(message);
        }
        catch (OperationCanceledException)
        {
            progressBar.Value = 0;
            completionStatus = "Cancelled.";
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException or ArgumentException)
        {
            completionStatus = "Extraction failed.";
            ShowError(ex.Message);
        }
        finally
        {
            SetBusy(false);
            if (completionStatus is not null && !IsDisposed)
                statusLabel.Text = completionStatus;

            _cancellation.Dispose();
            _cancellation = null;

            if (_closeAfterCancel && !IsDisposed)
                Close();
        }
    }

    private void OnProgress(ExtractionProgress progress)
    {
        if (IsDisposed || !_extracting)
            return;

        if (progress.TotalTiles > 0)
        {
            progressBar.Maximum = progress.TotalTiles;
            SetProgress(progress.CompletedTiles);
        }

        statusLabel.Text = progress.Status;
    }

    private void SetProgress(int value)
    {
        int maximum = progressBar.Maximum;
        value = Math.Clamp(value, progressBar.Minimum, maximum);

        if (value <= progressBar.Minimum || value >= maximum)
        {
            progressBar.Value = value;
            return;
        }

        // The native progress bar paints one step behind the assigned value.
        progressBar.Value = value + 1;
        progressBar.Value = value;
    }

    private void SetBusy(bool busy)
    {
        _extracting = busy;
        romPathTextBox.Enabled = !busy;
        browseRomButton.Enabled = !busy;
        outputTextBox.Enabled = !busy;
        browseOutputButton.Enabled = !busy;
        modeGroupBox.Enabled = !busy;
        extractButton.Text = busy ? "Cancel" : "&Extract";
        UseWaitCursor = busy;

        if (busy)
            extractButton.Enabled = true;
        else
            UpdateExtractEnabled();
    }

    private static string DescribeResult(IReadOnlyList<string> files, NesRom rom, ExtractionMode mode)
    {
        if (files.Count == 0)
            return "No files were written.";

        var folder = Path.GetDirectoryName(files[0]) ?? string.Empty;
        var noun = files.Count == 1 ? "bitmap" : "bitmaps";
        var message = $"Saved {files.Count:N0} {noun} to:{Environment.NewLine}{folder}";

        if (mode == ExtractionMode.EntireFile)
        {
            int leftover = rom.Data.Length % NesTileBitmap.BytesPerTile;
            if (leftover > 0)
                message += $"{Environment.NewLine}{leftover} trailing bytes were shorter than a tile and were skipped.";
        }

        return message;
    }

    private static string FormatSize(int bytes)
    {
        if (bytes >= 1024 * 1024 && bytes % (1024 * 1024) == 0)
            return $"{bytes / (1024 * 1024)} MB";

        if (bytes >= 1024 && bytes % 1024 == 0)
            return $"{bytes / 1024} KB";

        return $"{bytes:N0} bytes";
    }

    private void ShowError(string message)
    {
        MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void ShowInfo(string message)
    {
        MessageBox.Show(this, message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
