namespace GExtract.Tests;

public class MainFormTests
{
    [Fact]
    public void Form_constructs_before_a_rom_is_chosen()
    {
        Exception? error = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var form = new MainForm();
                if (form.Text != "GExtract")
                    throw new InvalidOperationException($"Unexpected title '{form.Text}'.");
            }
            catch (Exception ex)
            {
                error = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(20)), "The form constructor did not finish.");
        Assert.True(error is null, error?.ToString());
    }

    [Fact]
    public void Form_lays_out_the_progress_bar_and_extract_button()
    {
        Exception? error = null;
        var thread = new Thread(() =>
        {
            try
            {
                using var form = new MainForm();
                form.Show();
                form.Refresh();
                Application.DoEvents();

                var progress = Find<ProgressBar>(form);
                var extract = Find<Button>(form, button => button.Text.Contains("Extract", StringComparison.Ordinal));
                var browseButtons = FindAll<Button>(form).Where(button => button.Text == "Browse...").ToList();

                if (progress.Height < 16)
                    throw new InvalidOperationException($"Progress bar height was {progress.Height}.");

                if (progress.Width < 200)
                    throw new InvalidOperationException($"Progress bar width was {progress.Width}.");

                if (extract.Width < 40 || extract.Height < 20)
                    throw new InvalidOperationException($"Extract button size was {extract.Width}x{extract.Height}.");

                if (browseButtons.Count != 2)
                    throw new InvalidOperationException($"Expected 2 browse buttons, found {browseButtons.Count}.");

                var radios = FindAll<RadioButton>(form).ToList();
                if (radios.Count != 3 || radios.Any(radio => radio.Enabled))
                    throw new InvalidOperationException("Extraction choices should be disabled until a ROM is loaded.");

                var romBox = FindAll<TextBox>(form).First();
                romBox.Text = @"D:\Users\Matt\Downloads\gextract\Sample ROMs\Flying Warriors (USA).nes";
                romBox.Focus();
                form.SelectNextControl(romBox, forward: true, tabStopOnly: true, nested: true, wrap: true);
                Application.DoEvents();

                if (!extract.Enabled)
                    throw new InvalidOperationException("Extract should be enabled after a CHR ROM is loaded.");

                var status = FindAll<Label>(form).Single(label => label.Text.StartsWith("Ready", StringComparison.Ordinal));
                if (status.Text != "Ready to save 16 bitmaps.")
                    throw new InvalidOperationException($"Unexpected status '{status.Text}'.");

                form.Close();
            }
            catch (Exception ex)
            {
                error = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();

        Assert.True(thread.Join(TimeSpan.FromSeconds(20)), "The form layout check did not finish.");
        Assert.True(error is null, error?.ToString());
    }

    private static T Find<T>(Control root, Func<T, bool>? match = null)
        where T : Control
    {
        return FindAll(root, match).Single();
    }

    private static IEnumerable<T> FindAll<T>(Control root, Func<T, bool>? match = null)
        where T : Control
    {
        foreach (Control child in root.Controls)
        {
            if (child is T typed && (match is null || match(typed)))
                yield return typed;

            foreach (var nested in FindAll(child, match))
                yield return nested;
        }
    }
}
