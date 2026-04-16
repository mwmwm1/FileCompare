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

        }

        private void btnRightDir_Click(object sender, EventArgs e)
        {

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
    }
}
    

