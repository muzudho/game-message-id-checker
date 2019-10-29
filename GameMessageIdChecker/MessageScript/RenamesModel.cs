namespace GameMessageIdChecker
{
    public class RenamesModel
    {
        public string NewName { get; private set; }
        public string File { get; private set; }
        public int Row { get; private set; }
        public string OldName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newName">新しい名前</param>
        /// <param name="file">ファイル・パス</param>
        /// <param name="row">先頭が1の行番号</param>
        /// <param name="oldName">元の名前</param>
        public RenamesModel(string newName, string file, int row, string oldName)
        {
            this.NewName = newName;
            this.File = file;
            this.Row = row;
            this.OldName = oldName;
        }

        /// <summary>
        /// デバッグ表示用のメソッドがあると便利だぜ☆（＾～＾）
        /// </summary>
        /// <returns></returns>
        public string ToDisplay()
        {
            return $"Trace: 新しい名前={this.NewName} ファイル={this.File} 行={this.Row} 元={this.OldName}";
        }
    }
}
