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
            if (lvwLeftDir.SelectedItems.Count == 0) return;

            string name = lvwLeftDir.SelectedItems[0].Text;
            string sourcePath = Path.Combine(txtLeftDir.Text, name);
            string targetPath = Path.Combine(txtRightDir.Text, name);

            try
            {
                // 선택한 항목이 폴더인지 확인
                if (Directory.Exists(sourcePath))
                {
                    CopyDirectory(sourcePath, targetPath); // 폴더 복사 실행
                }
                else
                {
                    File.Copy(sourcePath, targetPath, true); // 파일 복사 실행
                }

                CompareFolders(); // 리스트 갱신 및 색상 표시
            }
            catch (Exception ex)
            {
                MessageBox.Show("복사 중 오류 발생: " + ex.Message);
            }
        }

        private void btnRightDir_Click(object sender, EventArgs e)
        {
            if (lvwLeftDir.SelectedItems.Count == 0) return;

            string name = lvwLeftDir.SelectedItems[0].Text;
            string sourcePath = Path.Combine(txtLeftDir.Text, name);
            string targetPath = Path.Combine(txtRightDir.Text, name);

            try
            {
                // 선택한 항목이 폴더인지 확인
                if (Directory.Exists(sourcePath))
                {
                    CopyDirectory(sourcePath, targetPath); // 폴더 복사 실행
                }
                else
                {
                    File.Copy(sourcePath, targetPath, true); // 파일 복사 실행
                }

                CompareFolders(); // 리스트 갱신 및 색상 표시
            }
            catch (Exception ex)
            {
                MessageBox.Show("복사 중 오류 발생: " + ex.Message);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void CompareFolders()
        {
            lvwLeftDir.Items.Clear();
            lvwRightDir.Items.Clear();

            // 경로가 있는지 체크 (하나라도 있으면 일단 진행)
            string leftPath = txtLeftDir.Text;
            string rightPath = txtRightDir.Text;

            DirectoryInfo leftDir = Directory.Exists(leftPath) ? new DirectoryInfo(leftPath) : null;
            DirectoryInfo rightDir = Directory.Exists(rightPath) ? new DirectoryInfo(rightPath) : null;

            var leftDict = new Dictionary<string, FileSystemInfo>();
            var rightDict = new Dictionary<string, FileSystemInfo>();

            // 정보 수집 (폴더 + 파일 모두 포함)
            if (leftDir != null)
            {
                foreach (var d in leftDir.GetDirectories()) leftDict[d.Name] = d;
                foreach (var f in leftDir.GetFiles()) leftDict[f.Name] = f;
            }
            if (rightDir != null)
            {
                foreach (var d in rightDir.GetDirectories()) rightDict[d.Name] = d;
                foreach (var f in rightDir.GetFiles()) rightDict[f.Name] = f;
            }

            // 왼쪽 리스트뷰 그리기
            if (leftDir != null) FillList(lvwLeftDir, leftDict, rightDict);
            // 오른쪽 리스트뷰 그리기
            if (rightDir != null) FillList(lvwRightDir, rightDict, leftDict);
        }

        // 리스트뷰 아이템 생성 및 색상 결정 보조 함수
        private void FillList(ListView lvw, Dictionary<string, FileSystemInfo> source, Dictionary<string, FileSystemInfo> target)
        {
            foreach (var name in source.Keys)
            {
                FileSystemInfo s = source[name];
                ListViewItem item = new ListViewItem(s.Name);

                if (s is DirectoryInfo) item.SubItems.Add("<폴더>"); // 폴더 표시
                else item.SubItems.Add((((FileInfo)s).Length / 1024).ToString("N0") + " KB");

                item.SubItems.Add(s.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                // 색상 규칙 적용
                if (target.Count > 0) // 반대편 폴더가 선택된 상태일 때만 비교
                {
                    if (!target.ContainsKey(name)) item.ForeColor = Color.Purple; // 단독
                    else if (s.LastWriteTime > target[name].LastWriteTime) item.ForeColor = Color.Red; // New
                    else if (s.LastWriteTime < target[name].LastWriteTime) item.ForeColor = Color.Gray; // Old
                }
                lvw.Items.Add(item);
            }
        }
        private void CopyDirectory(string sourceDir, string destDir)
        {
            // 1. 대상 폴더가 없으면 생성
            DirectoryInfo dir = new DirectoryInfo(sourceDir);
            if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);

            // 2. 현재 폴더 내의 모든 파일 복사
            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destDir, file.Name);
                file.CopyTo(targetFilePath, true); // true: 덮어쓰기 허용
            }

            // 3. 하위 폴더 탐색 (재귀 호출)
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                string targetSubDir = Path.Combine(destDir, subDir.Name);
                CopyDirectory(subDir.FullName, targetSubDir); // 자기 자신을 다시 호출
            }
        }

        private void FillListViewWithSub(ListView lvw, Dictionary<string, FileSystemInfo> source, Dictionary<string, FileSystemInfo> target)
        {
            foreach (var name in source.Keys)
            {
                FileSystemInfo sInfo = source[name];
                ListViewItem item = new ListViewItem(sInfo.Name);

                // 크기 열 표시 (폴더면 "폴더", 파일이면 크기)
                if (sInfo is DirectoryInfo) item.SubItems.Add("<폴더>");
                else item.SubItems.Add((((FileInfo)sInfo).Length / 1024).ToString("N0") + " KB");

                item.SubItems.Add(sInfo.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                // 색상 규칙 적용 (이미지 규칙 준수)
                if (!target.ContainsKey(name)) item.ForeColor = Color.Purple; // 단독
                else if (sInfo.LastWriteTime > target[name].LastWriteTime) item.ForeColor = Color.Red; // New
                else if (sInfo.LastWriteTime < target[name].LastWriteTime) item.ForeColor = Color.Gray; // Old

                lvw.Items.Add(item);
            }
        }

        private void CopyFile(string sourcePath, string targetPath)
        {
            try
            {
                FileInfo sourceFile = new FileInfo(sourcePath);
                FileInfo targetFile = new FileInfo(targetPath);

                // 1. 대상 폴더에 파일이 이미 있는지 확인
                if (targetFile.Exists)
                {
                    // 메시지 구성 (이미지 예시와 동일하게 경로와 날짜 포함)
                    string msg = "대상에 동일한 이름의 파일이 이미 있습니다.\n";

                    if (sourceFile.LastWriteTime > targetFile.LastWriteTime)
                        msg += "원본 파일이 더 신규 파일입니다. 덮어쓰시겠습니까?\n\n";
                    else
                        msg += "대상 파일이 더 신규 파일이거나 같습니다. 덮어쓰시겠습니까?\n\n";

                    // 주소(경로)와 날짜 정보 추가
                    msg += $"원본: {sourceFile.FullName}\n";
                    msg += $"수정일: {sourceFile.LastWriteTime:yyyy-MM-dd HH:mm:ss}\n\n";

                    msg += $"대상: {targetFile.FullName}\n";
                    msg += $"수정일: {targetFile.LastWriteTime:yyyy-MM-dd HH:mm:ss}";

                    // 과제 요구사항: "확인" 받아 진행여부 결정
                    if (MessageBox.Show(msg, "덮어쓰기 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    {
                        return; // 사용자가 '아니오'를 누르면 중단
                    }
                }

                // 2. 파일 복사 실행 (true는 덮어쓰기 허용)
                File.Copy(sourcePath, targetPath, true);

                // 3. 복사 후 리스트뷰와 색상 갱신
                CompareFolders();

                MessageBox.Show("파일 복사가 완료되었습니다.", "완료", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"복사 중 오류 발생: {ex.Message}", "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ExecuteCopy(string sourcePath, string targetPath)
        {
            try
            {
                if (Directory.Exists(sourcePath)) // 대상이 폴더인 경우
                {
                    if (MessageBox.Show("폴더와 하위 내용을 모두 복사하시겠습니까?", "폴더 복사", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        CopyDirectory(sourcePath, targetPath);
                    }
                }
                else if (File.Exists(sourcePath)) // 대상이 파일인 경우
                {
                    FileInfo sFile = new FileInfo(sourcePath);
                    FileInfo tFile = new FileInfo(targetPath);

                    if (tFile.Exists) // 덮어쓰기 확인
                    {
                        string msg = $"동일 파일 존재. 덮어쓰시겠습니까?\n\n원본: {sFile.FullName}\n수정일: {sFile.LastWriteTime}\n\n대상: {tFile.FullName}\n수정일: {tFile.LastWriteTime}";
                        if (MessageBox.Show(msg, "확인", MessageBoxButtons.YesNo) == DialogResult.No) return;
                    }
                    File.Copy(sourcePath, targetPath, true);
                }
                CompareFolders(); // 리스트 즉시 갱신
            }
            catch (Exception ex) { MessageBox.Show("오류: " + ex.Message); }
        }
        private void CopyFolder(string sourcePath, string targetPath)
        {
            // 대상 폴더가 없으면 생성
            if (!Directory.Exists(targetPath)) Directory.CreateDirectory(targetPath);

            // 모든 파일 복사
            foreach (string file in Directory.GetFiles(sourcePath))
            {
                string dest = Path.Combine(targetPath, Path.GetFileName(file));
                File.Copy(file, dest, true);
            }

            // 모든 하위 폴더 복사 (재귀 호출)
            foreach (string folder in Directory.GetDirectories(sourcePath))
            {
                string dest = Path.Combine(targetPath, Path.GetFileName(folder));
                CopyFolder(folder, dest);
            }
        }
    }
}
    

