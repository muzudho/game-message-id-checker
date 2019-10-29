using System.Collections.Generic;
using System.Diagnostics;
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

                    // リッチ・テキスト・ボックスは使いにくかったので、ふつうのテキストボックスを使うぜ☆（＾～＾）
                    // 左と右のふたつがあるぜ☆（＾～＾）
                    this.leftTextBox.Text = string.Empty;
                    this.rightTextBox.Text = string.Empty;

                    SearchesDirectory.Go(directory, (string fileEntry) =>
                    {
                        // entry は、ファイルのフルパス☆（＾～＾）
                        // 圧縮ファイル読み込んでも嫌だよな☆（＾～＾） 拡張子は .txt （大文字小文字を区別しない）にしておこうぜ☆（＾～＾）
                        if (System.IO.Path.GetExtension(fileEntry).ToUpper(CultureInfo.CurrentCulture) == ".TXT")
                        {
                            var msgDoc = MessageDocument.Read(fileEntry);
                            msgDoc.ScanIdRow((row, line) =>
                            {
                                // 左のテキスト・ボックスへ☆（＾～＾）
                                this.leftTextBox.Text += $"{line.Text}\n";

                                // 右のテキスト・ボックスへ☆（＾～＾）
                                var text = string.Format(CultureInfo.CurrentCulture, "{0}>{1}:{2}", fileEntry, row, line.Text);
                                this.rightTextBox.Text += $"{text}\n";
                            });
                        }
                    });
                }
            }
        }

        /// <summary>
        /// 上書きボタン。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void overwritesButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            Trace.WriteLine("leftText:" + this.leftTextBox.Text);
            Trace.WriteLine("rightText:" + this.rightTextBox.Text);
            */

            var leftLines = leftTextBox.Text.Split('\n');
            var rightLines = rightTextBox.Text.Split('\n');

            if (leftLines.Length != rightLines.Length)
            {
                Trace.WriteLine($"Error           | 長さが合いません。左={leftLines.Length} 右={rightLines.Length}");
                return;
            }

            var renamesModelList = new List<RenamesModel>();
            var size = leftLines.Length;
            Trace.WriteLine("size:" + size);
            for (var i = 0; i < size; i++)
            {
                // 最後は空文字列？
                var left = leftLines[i];
                var right = rightLines[i];
                Trace.WriteLine($"left: {left}");
                Trace.WriteLine($"right: {right}");

                // ファイルパスに含まれない文字を区切りに使うぜ☆（＾～＾）
                var firstColon = right.IndexOf(">", System.StringComparison.Ordinal);
                Trace.WriteLine($"firstColon: {firstColon}");
                if (0 < firstColon)
                {
                    var secondColon = right.IndexOf(":", firstColon, System.StringComparison.Ordinal);
                    Trace.WriteLine($"secondColon: {secondColon}");
                    if (0 < secondColon)
                    {
                        var file = right.Substring(0, firstColon);
                        Trace.WriteLine($"file: {file}");
                        var rowText = right.Substring(firstColon + 1, secondColon - (firstColon+1));
                        Trace.WriteLine($"rowText: {rowText}");
                        if (int.TryParse(rowText, out int row))
                        {
                            var oldName = right.Substring(secondColon + 1);
                            Trace.WriteLine($"oldName: {oldName}");

                            var renamesModel = new RenamesModel(left, file, row, oldName);
                            renamesModelList.Add(renamesModel);
                            Trace.WriteLine($"Trace: {renamesModel.ToDisplay()}");
                        }
                    }
                    else
                    {
                        Trace.WriteLine($"Trace: 左={left}");
                    }
                }
                else
                {
                    Trace.WriteLine($"Trace: 左={left}");
                }
            }

            Trace.WriteLine("renamesModelList.Count:" + renamesModelList.Count);

            // 行置換☆（＾～＾）
            // １行更新するだけでファイルを開け閉めするのは非効率だが　最初は簡単に実装しようぜ☆（＾～＾）
            foreach (var renamesModel in renamesModelList)
            {
                ReplacesLine.Go(renamesModel);
            }
        }
    }
}
