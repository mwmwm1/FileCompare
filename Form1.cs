namespace FileCompare
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCopyFromLeft_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // 텍스트박스에 경로 표시
                    txtLeftDir.Text = fbd.SelectedPath;
                    // 비교 함수 호출 (아래 2번에서 만들 함수)
                    CompareFolders();
                }
            }

        }

        private void btnCopyFromRight_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // 텍스트박스에 경로 표시
                    txtRightDir.Text = fbd.SelectedPath;
                    // 비교 함수 호출
                    CompareFolders();
                }
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void lvwLeftDir_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lvwRightDir_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnLeftDir_Click(object sender, EventArgs e)
        {
            if (lvwLeftDir.SelectedItems.Count == 0)
            {
                MessageBox.Show("복사할 파일을 왼쪽에서 선택해주세요.");
                return;
            }

            string fileName = lvwLeftDir.SelectedItems[0].Text;
            string sourceFullPath = Path.Combine(txtLeftDir.Text, fileName);
            string targetFullPath = Path.Combine(txtRightDir.Text, fileName);

            CopyFile(sourceFullPath, targetFullPath);
        }

        private void btnRightDir_Click(object sender, EventArgs e)
        {
            if (lvwRightDir.SelectedItems.Count == 0)
            {
                MessageBox.Show("복사할 파일을 오른쪽에서 선택해주세요.");
                return;
            }

            string fileName = lvwRightDir.SelectedItems[0].Text;
            string sourceFullPath = Path.Combine(txtRightDir.Text, fileName);
            string targetFullPath = Path.Combine(txtLeftDir.Text, fileName);

            CopyFile(sourceFullPath, targetFullPath);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void CompareFolders()
        {
            // 리스트뷰 초기화
            lvwLeftDir.Items.Clear();
            lvwRightDir.Items.Clear();

            // 경로 정보 가져오기
            string leftPath = txtLeftDir.Text;
            string rightPath = txtRightDir.Text;

            DirectoryInfo leftDir = Directory.Exists(leftPath) ? new DirectoryInfo(leftPath) : null;
            DirectoryInfo rightDir = Directory.Exists(rightPath) ? new DirectoryInfo(rightPath) : null;

            // 비교를 위한 딕셔너리 준비
            var leftDict = new Dictionary<string, FileInfo>();
            var rightDict = new Dictionary<string, FileInfo>();

            if (leftDir != null) foreach (var f in leftDir.GetFiles()) leftDict[f.Name] = f;
            if (rightDir != null) foreach (var f in rightDir.GetFiles()) rightDict[f.Name] = f;

            // --- 왼쪽 리스트뷰 채우기 (경로가 있을 때만) ---
            if (leftDir != null)
            {
                foreach (var fName in leftDict.Keys)
                {
                    FileInfo f = leftDict[fName];
                    ListViewItem item = new ListViewItem(f.Name);
                    item.SubItems.Add((f.Length / 1024).ToString("N0") + " KB");
                    item.SubItems.Add(f.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                    // 오른쪽 폴더도 선택된 상태라면 색상 비교 실행
                    if (rightDir != null)
                    {
                        if (!rightDict.ContainsKey(fName)) item.ForeColor = Color.Purple;
                        else if (f.LastWriteTime > rightDict[fName].LastWriteTime) item.ForeColor = Color.Red;
                        else if (f.LastWriteTime < rightDict[fName].LastWriteTime) item.ForeColor = Color.Gray;
                    }
                    lvwLeftDir.Items.Add(item);
                }
            }

            // --- 오른쪽 리스트뷰 채우기 (경로가 있을 때만) ---
            if (rightDir != null)
            {
                foreach (var fName in rightDict.Keys)
                {
                    FileInfo f = rightDict[fName];
                    ListViewItem item = new ListViewItem(f.Name);
                    item.SubItems.Add((f.Length / 1024).ToString("N0") + " KB");
                    item.SubItems.Add(f.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                    // 왼쪽 폴더도 선택된 상태라면 색상 비교 실행
                    if (leftDir != null)
                    {
                        if (!leftDict.ContainsKey(fName)) item.ForeColor = Color.Purple;
                        else if (f.LastWriteTime > leftDict[fName].LastWriteTime) item.ForeColor = Color.Red;
                        else if (f.LastWriteTime < leftDict[fName].LastWriteTime) item.ForeColor = Color.Gray;
                    }
                    lvwRightDir.Items.Add(item);
                }
            }
        }
        private void CopyFile(string sourcePath, string targetPath)
        {
            try
            {
                FileInfo sourceFile = new FileInfo(sourcePath);
                FileInfo targetFile = new FileInfo(targetPath);

                // 1. 대상 폴더에 동일한 이름의 파일이 있는지 확인
                if (targetFile.Exists)
                {
                    // 수정된 날짜 비교 (이미지 13페이지 내용 반영)
                    string msg = $"대상에 동일한 이름의 파일이 이미 있습니다.\n";

                    if (sourceFile.LastWriteTime > targetFile.LastWriteTime)
                        msg += "원본 파일이 더 신규 파일입니다. 덮어쓰시겠습니까?\n\n";
                    else
                        msg += "대상 파일이 더 신규 파일이거나 같습니다. 그래도 덮어쓰시겠습니까?\n\n";

                    msg += $"원본: {sourceFile.LastWriteTime}\n";
                    msg += $"대상: {targetFile.LastWriteTime}";

                    // 사용자에게 "확인" 받아 진행여부 결정
                    if (MessageBox.Show(msg, "덮어쓰기 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return; // '아니오' 누르면 복사 중단
                    }
                }

                // 2. 파일 복사 실행
                File.Copy(sourcePath, targetPath, true);
                MessageBox.Show("복사가 완료되었습니다.");

                // 3. 복사 후 리스트 갱신 (색상 다시 계산)
                CompareFolders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"복사 중 오류 발생: {ex.Message}");
            }
        }
    }
}
    

