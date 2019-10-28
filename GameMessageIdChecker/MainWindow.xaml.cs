using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Microsoft.WindowsAPICodePack.Dialogs; // NuGetで 作者Aybeの `WindowsAPICodePack-Core`, `WindowsAPICodePack-Shell` 追加☆（＾～＾）

namespace GameMessageIdChecker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void checksDirectory_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new CommonOpenFileDialog
            {
                Title = "Check Directory",
                // フォルダ選択ダイアログの場合は true
                IsFolderPicker = true,
                // ダイアログが表示されたときの初期ディレクトリを指定
                InitialDirectory = "適当なパス",

                // ユーザーが最近したアイテムの一覧を表示するかどうか
                AddToMostRecentlyUsedList = false,
                // ユーザーがフォルダやライブラリなどのファイルシステム以外の項目を選択できるようにするかどうか
                AllowNonFileSystemItems = false,
                // 最近使用されたフォルダが利用不可能な場合にデフォルトとして使用されるフォルダとパスを設定する
                DefaultDirectory = "適当なパス",
                // 存在するファイルのみ許可するかどうか
                EnsureFileExists = true,
                // 存在するパスのみ許可するかどうか
                EnsurePathExists = true,
                // 読み取り専用ファイルを許可するかどうか
                EnsureReadOnly = false,
                // 有効なファイル名のみ許可するかどうか（ファイル名を検証するかどうか）
                EnsureValidNames = true,
                // 複数選択を許可するかどうか
                Multiselect = false,
                // PC やネットワークなどの場所を表示するかどうか
                ShowPlacesList = true
            })
            {
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    var directory = dialog.FileName;

                    // リッチ・テキスト・ボックスは、左と右のふたつがあるぜ☆（＾～＾）
                    FlowDocument leftFlowDoc = new FlowDocument();
                    FlowDocument rightFlowDoc = new FlowDocument();
                    {
                        Paragraph leftParagraph = new Paragraph();
                        Paragraph rightParagraph = new Paragraph();
                        {
                            SearchesDirectory.Go(directory, (string fileEntry) =>
                            {
                                // entry は、ファイルのフルパス☆（＾～＾）
                                // 圧縮ファイル読み込んでも嫌だよな☆（＾～＾） 拡張子は .txt （大文字小文字を区別しない）にしておこうぜ☆（＾～＾）
                                if (System.IO.Path.GetExtension(fileEntry).ToUpper(CultureInfo.CurrentCulture) == ".TXT")
                                {
                                    var msgDoc = MessageDocument.Read(fileEntry);
                                    msgDoc.ScanIdRow((row, line) =>
                                    {
                                        // 左のリッチ・テキスト・ボックスへ☆（＾～＾）WordWrapしたくないので TextBlockで包むぜ☆（＾～＾）
                                        var idRun = new Run(line.Text);
                                        leftParagraph.Inlines.Add(new TextBlock(idRun));
                                        leftParagraph.Inlines.Add(new LineBreak());

                                        // 右のリッチ・テキスト・ボックスへ☆（＾～＾）WordWrapしたくないので TextBlockで包むぜ☆（＾～＾）
                                        var text = string.Format(CultureInfo.CurrentCulture, "{0}:{1}", fileEntry, row);
                                        var addressRun = new Run(text);
                                        addressRun.Foreground = Brushes.LightGray;
                                        rightParagraph.Inlines.Add(new TextBlock(addressRun));
                                        rightParagraph.Inlines.Add(new LineBreak());
                                    });
                                }
                            });

                            // Add the paragraph to the FlowDocument.
                            leftFlowDoc.Blocks.Add(leftParagraph);
                            rightFlowDoc.Blocks.Add(rightParagraph);
                        }

                        // Add initial content to the RichTextBox.
                        leftRichTextBox.Document = leftFlowDoc;
                        rightRichTextBox.Document = rightFlowDoc;
                    }
                }
            }
        }
    }
}
