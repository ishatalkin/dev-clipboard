using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using DeveloperClipboardCore;
using DeveloperClipboardCore.Conventions;
using InputSimulatorEx;
using InputSimulatorEx.Native;
using Clipboard = System.Windows.Clipboard;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseButton = System.Windows.Input.MouseButton;

namespace DeveloperClipboard
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainViewModel Model => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();

            // Спрячем из Alt+Tab
            SourceInitialized += (s, e) =>
            {
                var win = new WindowInteropHelper(this);
                win.Owner = GetDesktopWindow();
            };
        }

        [DllImport("user32.dll", SetLastError = false)]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int HOTKEY_ID = 9009;

        //Modifiers:
        private const uint MOD_NONE = 0x0000; //(none)
        private const uint MOD_ALT = 0x0001; //ALT
        private const uint MOD_CONTROL = 0x0002; //CTRL
        private const uint MOD_SHIFT = 0x0004; //SHIFT

        private const uint MOD_WIN = 0x0008; //WINDOWS

        //CAPS LOCK:
        private const uint VK_F8 = 0x77;

        private IntPtr _windowHandle;
        private HwndSource _source;

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            if (!RegisterHotKey(_windowHandle, HOTKEY_ID, MOD_CONTROL, VK_F8))
            {
                //TODO нормальный exception
                throw new Exception();
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case HOTKEY_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_F8)
                            {
                                OnShowClipboard();
                            }

                            handled = true;
                            break;
                    }

                    break;
            }

            return IntPtr.Zero;
        }

        private void OnShowClipboard()
        {
            Show();
            Activate();

            if (!Clipboard.ContainsText())
                return;

            var text = Clipboard.GetText();

            if (Model.CurrentText == text)
                return;

            Model.CurrentText = text;
            Model.Snippets.Clear();
            Model.Snippets.AddConverted("Скопированный текст", text);
            Snippets.SelectedIndex = 0;
            Snippets.Focus();
            FocusSelectedItem();

            var snippetConverter = new SnippetConverter();
            var tsToCsConverter = new RegexReplaceConverter();
            AddResult(tsToCsConverter.Convert(text, TypeScriptToCSharpConventions.Convertions), "TypeScript to C#");
            AddResult(tsToCsConverter.Convert(text, CSharpToTypeScriptConventions.TypeToInterfaceConventions), "C# to TS");
            AddResult(tsToCsConverter.Convert(text, CSharpToTypeScriptConventions.ParamsConventions), "C# to TS (params)");
            AddResult(tsToCsConverter.Convert(text, CSharpInitConventions.Conventions), "C# to init");
            AddResult(tsToCsConverter.Convert(text, DdlToCSharpConventions.Conventions), "DDL to C#");
            AddResult(snippetConverter.JsonToCsharp(text), "JSON to C#");
            AddResult(snippetConverter.JsonToJava(text), "JSON to Java");
            AddResult(snippetConverter.JsonToPython(text), "JSON to Python");
        }

        private void FocusSelectedItem()
        {
            if (Snippets.SelectedItem != null)
            {
                Snippets.UpdateLayout(); // Pre-generates item containers 
                var listBoxItem = Snippets
                    .ItemContainerGenerator
                    .ContainerFromItem(Snippets.SelectedItem) as ListBoxItem;
                listBoxItem?.Focus();
            }
        }

        private async Task AddResult(Task<ConvertionResult> resultReceiver, string description)
        {
            var codeSnippet = Model.Snippets.Add($"{description}...");
            var result = await resultReceiver;
            if (result.State == ConvertionState.Ok)
            {
                codeSnippet.Description = description;
                codeSnippet.Code = result.Code;
            }
            else
            {
                Model.Snippets.Remove(codeSnippet);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, HOTKEY_ID);
            base.OnClosed(e);
        }

        private void Snippets_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                PasteCode();
            else if (e.Key == Key.Escape)
            {
                Hide();
            }
        }

        private void Snippets_OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
                return;

            PasteCode();
        }

        private void PasteCode()
        {
            try
            {
                var selectedCode = Model.Selected?.Code;

                Console.WriteLine($"Selected: [{selectedCode}]");
                Hide();

                if (selectedCode != null)
                {
                    Clipboard.SetText(selectedCode);
                    InputSimulator sim = new();
                    sim.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            Hide();
            Width = 600;
            Height = 350;
        }

        private void Snippets_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                // Escape по умолчанию сбрасывает выделение
                e.Handled = true;
            }
        }
    }
}