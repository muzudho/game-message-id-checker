namespace GameMessageIdChecker
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Windows;
    using Microsoft.WindowsAPICodePack.Dialogs; // NuGetで 作者Aybeの `WindowsAPICodePack-Core`, `WindowsAPICodePack-Shell` 追加☆（＾～＾）

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

                    var sortsModelList = new List<SortsModel>();

                    SearchesDirectory.Go(directory, (string fileEntry) =>
                    {
                        // entry は、ファイルのフルパス☆（＾～＾）
                        // 圧縮ファイル読み込んでも嫌だよな☆（＾～＾） 拡張子は .txt （大文字小文字を区別しない）にしておこうぜ☆（＾～＾）
                        if (System.IO.Path.GetExtension(fileEntry).ToUpper(CultureInfo.CurrentCulture) == ".TXT")
                        {
                            var msgDoc = MessageDocument.Read(fileEntry);
                            msgDoc.ScanIdRow((row, line) =>
                            {
                                var sortsModel = new SortsModel(fileEntry, row, line.Text, false);
                                sortsModelList.Add(sortsModel);
                            });
                        }
                    });

                    // ソート。
                    {
                        sortsModelList.Sort((a, b) =>
                        {
                            // 大文字の Z のあとに 小文字の a が続いても見づらいんで、大文字小文字は無視するぜ☆（＾～＾）
                            // こういうのは自然順ソートといって a, b と並んでいるとき a < b なら降順だぜ☆（＾～＾）逆にすれば昇順☆（＾ｑ＾）
                            return string.Compare(a.Name, b.Name, System.StringComparison.OrdinalIgnoreCase);
                        });
                    }

                    // 重複確認。１つ前と同じ名前なら重複☆（＾～＾）
                    var previousName = string.Empty;
                    for (int i = 0; i < sortsModelList.Count; i++)
                    {
                        var sortsModel = sortsModelList[i];
                        var name = sortsModel.Name;
                        if (i != 0 && previousName == name)
                        {
                            // 重複あり。
                            sortsModelList[i] = new SortsModel(sortsModel.File, sortsModel.Row, sortsModel.Name, true);
                        }

                        previousName = name;
                    }

                    // リッチ・テキスト・ボックスは使いにくかったので、ふつうのテキストボックスを使うぜ☆（＾～＾）
                    // 左と右のふたつがあるぜ☆（＾～＾）
                    this.leftTextBox.Text = string.Empty;
                    this.rightTextBox.Text = string.Empty;

                    foreach (var sortsModel in sortsModelList)
                    {
                        // 左のテキスト・ボックスへ☆（＾～＾）
                        this.leftTextBox.Text += $"{sortsModel.Name}\n";

                        // 右のテキスト・ボックスへ☆（＾～＾）
                        var text = string.Format(CultureInfo.CurrentCulture, "{0}{1}>{2}:{3}", sortsModel.Duplicated?"*":" ", sortsModel.File, sortsModel.Row, sortsModel.Name);
                        this.rightTextBox.Text += $"{text}\n";
                    }
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
            // Trace.WriteLine("size:" + size);
            for (var i = 0; i < size; i++)
            {
                // 最後は空文字列？
                var left = leftLines[i];
                var right = rightLines[i];
                // Trace.WriteLine($"left: {left}");
                // Trace.WriteLine($"right: {right}");

                // 最初の１文字目は重複フラグなので無視☆（＾～＾）
                var duplicatedFlagOver = 1;

                // ファイルパスに含まれない文字を区切りに使うぜ☆（＾～＾）
                if (0<right.Length)
                {
                    var firstColon = right.IndexOf(">", duplicatedFlagOver, System.StringComparison.Ordinal);
                    // Trace.WriteLine($"firstColon: {firstColon}");
                    if (0 < firstColon)
                    {
                        var secondColon = right.IndexOf(":", firstColon, System.StringComparison.Ordinal);
                        // Trace.WriteLine($"secondColon: {secondColon}");
                        if (0 < secondColon)
                        {
                            var file = right.Substring(duplicatedFlagOver, firstColon - duplicatedFlagOver);
                            // Trace.WriteLine($"file: {file}");
                            var nextIdx = firstColon + 1;
                            var rowText = right.Substring(nextIdx, secondColon - nextIdx);
                            // Trace.WriteLine($"rowText: {rowText}");
                            if (int.TryParse(rowText, out int row))
                            {
                                var oldName = right.Substring(secondColon + 1);
                                // Trace.WriteLine($"oldName: {oldName}");

                                var renamesModel = new RenamesModel(left, file, row, oldName);
                                renamesModelList.Add(renamesModel);
                                // Trace.WriteLine($"Trace: {renamesModel.ToDisplay()}");
                            }
                        }
                    }
                }
            }

            // Trace.WriteLine("renamesModelList.Count:" + renamesModelList.Count);

            // 行置換☆（＾～＾）
            // １行更新するだけでファイルを開け閉めするのは非効率だが　最初は簡単に実装しようぜ☆（＾～＾）
            // ファイル順、行順にソートしなおして、ループの回数を減らせだぜ☆（＾～＾）
            foreach (var renamesModel in renamesModelList)
            {
                ReplacesLine.Go(renamesModel);
            }
        }
    }
}
