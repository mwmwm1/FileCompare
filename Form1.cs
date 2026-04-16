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
                // 2. 사용자가 폴더를 선택하고 '확인'을 눌렀을 때만 실행
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // 경로를 텍스트박스에 표시
                    txtLeftDir.Text = fbd.SelectedPath;

                    // 리스트뷰 초기화 (기존 목록 지우기)
                    lvwLeftDir.Items.Clear();

                    // 3. 선택된 폴더 내의 파일 정보를 가져옴
                    DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);

                    // 4. 각 파일 정보를 ListViewItem으로 만들어 추가
                    foreach (FileInfo file in di.GetFiles())
                    {
                        // 첫 번째 컬럼: 파일 이름
                        ListViewItem item = new ListViewItem(file.Name);

                        // 두 번째 컬럼: 크기 (KB 단위로 변환 후 쉼표 포맷)
                        long sizeKB = file.Length / 1024;
                        item.SubItems.Add(sizeKB.ToString("N0") + " KB");

                        // 세 번째 컬럼: 수정일 (날짜 포맷 지정)
                        item.SubItems.Add(file.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                        // 리스트뷰에 최종 추가
                        lvwLeftDir.Items.Add(item);
                    }
                }
            }
        }

        private void btnCopyFromRight_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                // 2. 사용자가 폴더를 선택하고 '확인'을 눌렀을 때만 실행
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    // 경로를 텍스트박스에 표시
                    txtRightDir.Text = fbd.SelectedPath;

                    // 리스트뷰 초기화 (기존 목록 지우기)
                    lvwRightDir.Items.Clear();

                    // 3. 선택된 폴더 내의 파일 정보를 가져옴
                    DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);

                    // 4. 각 파일 정보를 ListViewItem으로 만들어 추가
                    foreach (FileInfo file in di.GetFiles())
                    {
                        // 첫 번째 컬럼: 파일 이름
                        ListViewItem item = new ListViewItem(file.Name);

                        // 두 번째 컬럼: 크기 (KB 단위로 변환 후 쉼표 포맷)
                        long sizeKB = file.Length / 1024;
                        item.SubItems.Add(sizeKB.ToString("N0") + " KB");

                        // 세 번째 컬럼: 수정일 (날짜 포맷 지정)
                        item.SubItems.Add(file.LastWriteTime.ToString("yyyy-MM-dd HH:mm"));

                        // 리스트뷰에 최종 추가
                        lvwRightDir.Items.Add(item);
                    }
                }
            }
        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
