using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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

                    // Create a FlowDocument to contain content for the RichTextBox.
                    FlowDocument myFlowDoc = new FlowDocument();
                    {
                        // Create a paragraph and add the Run and Bold to it.
                        Paragraph myParagraph = new Paragraph();
                        {
                            SearchesDirectory.Go(directory, (string fileEntry) =>
                            {
                                // entry は、ファイルのフルパス☆（＾～＾）
                                // 圧縮ファイル読み込んでも嫌だよな☆（＾～＾） 拡張子は .txt （大文字小文字を区別しない）にしておこうぜ☆（＾～＾）
                                if (System.IO.Path.GetExtension(fileEntry).ToUpper(CultureInfo.CurrentCulture) == ".TXT")
                                {
                                    var fileRun = new Run(fileEntry);
                                    fileRun.Foreground = Brushes.LightGray;
                                    myParagraph.Inlines.Add(fileRun);
                                    myParagraph.Inlines.Add(new LineBreak());

                                    var msgDoc = MessageDocument.Read(fileEntry);
                                    foreach(var key in msgDoc.Keys)
                                    {
                                        var keyRun = new Run(key);
                                        myParagraph.Inlines.Add(keyRun);
                                        myParagraph.Inlines.Add(new LineBreak());
                                    }
                                }
                            });

                            // Add the paragraph to the FlowDocument.
                            myFlowDoc.Blocks.Add(myParagraph);
                        }

                        // Add initial content to the RichTextBox.
                        richTextBox.Document = myFlowDoc;
                    }
                }
            }
        }
    }
}
