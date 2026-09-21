using System.Drawing;
using System.Windows.Forms;
using TianzigeGenerator.Controls;
using TianzigeGenerator.Models;
using TianzigeGenerator.Services;

namespace TianzigeGenerator;

public partial class MainForm : Form
{
    private Project _project = null!;
    private int _selectedPageIndex = -1;
    private bool _updatingControls = false;

    // UI Controls
    private ListBox _pageList = null!;
    private GridPreviewControl _preview = null!;
    private Panel _settingsPanel = null!;
    private TabControl _settingsTabControl = null!;
    private ComboBox _cboPageType = null!;
    private ComboBox _cboPageSize = null!;
    private NumericUpDown _nudCellSize = null!;
    private NumericUpDown _nudCellSpacing = null!;
    private NumericUpDown _nudLineSpacing = null!;
    private NumericUpDown _nudColSpacing = null!;
    private NumericUpDown _nudMarginTop = null!;
    private NumericUpDown _nudMarginBottom = null!;
    private NumericUpDown _nudMarginLeft = null!;
    private NumericUpDown _nudMarginRight = null!;
    private CheckBox _chkDashed = null!;
    private CheckBox _chkDotted = null!;
    private CheckBox _chkDiagonals = null!;
    private Button _btnBorderColor = null!;
    private Button _btnGuideColor = null!;
    private NumericUpDown _nudBorderWidth = null!;
    private NumericUpDown _nudGuideWidth = null!;
    private TextBox _txtTitle = null!;
    private TextBox _txtSubtitle = null!;
    private TextBox _txtAuthor = null!;
    private TextBox _txtNotes = null!;
    private Panel _coverPanel = null!;
    private Panel _gridPanel = null!;
    private Button _btnSelectCoverImage = null!;
    private Button _btnClearCoverImage = null!;
    private FlowLayoutPanel _customTextFlow = null!;
    private Button _btnAddCustomText = null!;
    private Panel _linedPanel = null!;
    private NumericUpDown _nudLineHeight = null!;
    private Button _btnLineColor = null!;
    private CheckBox _chkLandscape = null!;
    private Label _lblStatus = null!;
    private FlowLayoutPanel _settingsFlow = null!;
    private Label _lblPageCount = null!;
    private Button _btnApplyToAll = null!;
    private Button _btnResetDefaults = null!;
    private Button _btnMultiply = null!;
    private Button _btnCellBgColor = null!;
    private Button _btnPageBgColor = null!;
    private NumericUpDown _nudTitleFontSize = null!;
    private NumericUpDown _nudSubtitleFontSize = null!;
    private NumericUpDown _nudAuthorFontSize = null!;
    private NumericUpDown _nudNotesFontSize = null!;
    private NumericUpDown _nudLineWidth = null!;
    private CheckBox _chkShowMarginLine = null!;
    private NumericUpDown _nudCornerRadius = null!;
    private NumericUpDown _nudInnerPadding = null!;
    private CheckBox _chkCellShading = null!;
    private Button _btnCellShadingColor = null!;
    private TextBox _txtColumnHeaders = null!;
    private TextBox _txtPageLabel = null!;
    private NumericUpDown _nudGridRows = null!;
    private NumericUpDown _nudGridCols = null!;

    public MainForm()
    {
        InitializeComponent();
        _project = Project.CreateDefault();
        this.KeyPreview = true;
        this.KeyDown += MainForm_KeyDown;
        RefreshPageList();
        if (_pageList.Items.Count > 0)
        {
            _pageList.SelectedIndex = 0;
        }
    }

    private void MainForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Control)
        {
            switch (e.KeyCode)
            {
                case Keys.N:
                    BtnNew_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.S:
                    BtnSave_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.O:
                    BtnLoad_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.P:
                    BtnGenerate_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.D:
                    BtnDuplicate_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.T:
                    BtnAdd_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Up:
                    BtnMoveUp_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
                case Keys.Down:
                    BtnMoveDown_Click(null, EventArgs.Empty);
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    break;
            }
        }
        else if (e.KeyCode == Keys.Delete)
        {
            BtnRemove_Click(null, EventArgs.Empty);
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private void InitializeComponent()
    {
        this.Text = "田字格生成器 - Tianzige PDF Generator";
        this.Size = new Size(1600, 950);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(1400, 850);
        this.Font = new Font("Segoe UI", 9F);
        this.BackColor = Color.FromArgb(240, 240, 245);

        // Main vertical layout container
        var mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(0),
            Margin = new Padding(0)
        };
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 60)); // Top bar
        mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100)); // Main content
        this.Controls.Add(mainLayout);

        // ===== TOP BAR =====
        var topBar = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(50, 50, 70),
            Margin = new Padding(0),
            Padding = new Padding(20, 0, 20, 0)
        };
        
        var titleLabel = new Label
        {
            Text = "📝 Tianzige PDF Generator",
            Font = new Font("Segoe UI", 14F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Left,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft
        };
        topBar.Controls.Add(titleLabel);
        
        var subtitleLabel = new Label
        {
            Text = "Create beautiful Chinese calligraphy grids",
            Font = new Font("Segoe UI", 9F, FontStyle.Italic),
            ForeColor = Color.FromArgb(180, 180, 200),
            Dock = DockStyle.Left,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(20, 0, 0, 0)
        };
        topBar.Controls.Add(subtitleLabel);
        
        mainLayout.Controls.Add(topBar, 0, 0);

        // ===== MAIN CONTENT AREA (Resizable Splitters) =====
        var contentContainer = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(10),
            BackColor = Color.FromArgb(240, 240, 245)
        };

        var outerSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterWidth = 8,
            BackColor = Color.FromArgb(220, 225, 235),
            FixedPanel = FixedPanel.Panel1
        };

        var innerSplit = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterWidth = 8,
            BackColor = Color.FromArgb(220, 225, 235),
            FixedPanel = FixedPanel.Panel2
        };

        mainLayout.Controls.Add(contentContainer, 0, 1);

        // ===== LEFT PANEL: SETTINGS =====
        var settingsContainer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = new Padding(0),
            Padding = new Padding(0),
            BackColor = Color.White
        };
        settingsContainer.RowStyles.Add(new RowStyle(SizeType.Absolute, 40));
        settingsContainer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // Settings header
        var settingsHeader = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(70, 130, 180),
            Padding = new Padding(15, 0, 0, 0)
        };
        var settingsTitle = new Label
        {
            Text = "⚙️ Page Settings",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        settingsHeader.Controls.Add(settingsTitle);
        settingsContainer.Controls.Add(settingsHeader, 0, 0);

        // Settings content
        _settingsPanel = CreateSettingsPanel();
        _settingsPanel.Dock = DockStyle.Fill;
        settingsContainer.Controls.Add(_settingsPanel, 0, 1);

        outerSplit.Panel1.Controls.Add(settingsContainer);

        // ===== CENTER PANEL: PREVIEW =====
        var previewContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(230, 230, 240),
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        
        // Preview header
        var previewHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 40,
            BackColor = Color.FromArgb(100, 150, 200),
            Padding = new Padding(15, 0, 0, 0)
        };
        var previewTitle = new Label
        {
            Text = "👁️ Live Preview",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };
        previewHeader.Controls.Add(previewTitle);
        previewContainer.Controls.Add(previewHeader);
        
        // Preview content with inner padding
        var previewInner = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(230, 230, 240),
            Padding = new Padding(20)
        };
        _preview = new GridPreviewControl { Dock = DockStyle.Fill };
        previewInner.Controls.Add(_preview);
        previewContainer.Controls.Add(previewInner);
        
        innerSplit.Panel1.Controls.Add(previewContainer);

        // ===== RIGHT PANEL: PAGE LIST =====
        var pageListContainer = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        
        // Page list with border
        var pageListPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(10)
        };
        
        // Draw border
        pageListPanel.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(180, 180, 200), 1);
            var rect = pageListPanel.ClientRectangle;
            rect.Inflate(-1, -1);
            e.Graphics.DrawRectangle(pen, rect);
        };
        
        _pageList = new ListBox
        {
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F),
            BorderStyle = BorderStyle.None,
            IntegralHeight = false,
            BackColor = Color.White
        };
        _pageList.SelectedIndexChanged += PageList_SelectedIndexChanged;
        pageListPanel.Controls.Add(_pageList);
        
        pageListContainer.Controls.Add(pageListPanel);
        
        // Page list header with toolbar
        var pageListHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 90,
            BackColor = Color.FromArgb(80, 140, 190),
            Padding = new Padding(10, 8, 10, 8)
        };
        
        var pagesTitle = new Label
        {
            Text = "📄 Pages",
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.White,
            Dock = DockStyle.Top,
            Height = 25,
            TextAlign = ContentAlignment.MiddleLeft
        };
        pageListHeader.Controls.Add(pagesTitle);
        
        // Page counter
        _lblPageCount = new Label
        {
            Dock = DockStyle.Top,
            Height = 20,
            Text = "0 pages",
            TextAlign = ContentAlignment.MiddleCenter,
            ForeColor = Color.FromArgb(220, 220, 240),
            Font = new Font("Segoe UI", 8.5F, FontStyle.Italic),
            BackColor = Color.FromArgb(80, 140, 190)
        };
        pageListHeader.Controls.Add(_lblPageCount);
        
        // Toolbar buttons
        var toolbar = CreateToolbar();
        toolbar.Dock = DockStyle.Top;
        toolbar.AutoSize = true;
        pageListHeader.Controls.Add(toolbar);
        
        pageListContainer.Controls.Add(pageListHeader);
        
        innerSplit.Panel2.Controls.Add(pageListContainer);
        outerSplit.Panel2.Controls.Add(innerSplit);
        contentContainer.Controls.Add(outerSplit);

        this.Load += (s, e) =>
        {
            try
            {
                outerSplit.Panel1MinSize = 350;
                outerSplit.Panel2MinSize = 670;
                if (outerSplit.Width > outerSplit.Panel1MinSize + outerSplit.Panel2MinSize)
                {
                    int maxOuter = outerSplit.Width - outerSplit.Panel2MinSize;
                    outerSplit.SplitterDistance = Math.Clamp(380, outerSplit.Panel1MinSize, Math.Max(outerSplit.Panel1MinSize, maxOuter));
                }

                innerSplit.Panel1MinSize = 400;
                innerSplit.Panel2MinSize = 250;
                if (innerSplit.Width > innerSplit.Panel1MinSize + innerSplit.Panel2MinSize)
                {
                    int targetDistance = innerSplit.Width - 320;
                    int maxInner = innerSplit.Width - innerSplit.Panel2MinSize;
                    innerSplit.SplitterDistance = Math.Clamp(targetDistance, innerSplit.Panel1MinSize, Math.Max(innerSplit.Panel1MinSize, maxInner));
                }
            }
            catch { /* Ignore initial layout edge cases */ }
        };

        // ===== STATUS BAR =====
        _lblStatus = new Label
        {
            Dock = DockStyle.Bottom,
            Height = 28,
            BackColor = Color.FromArgb(60, 60, 80),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(15, 0, 0, 0),
            Font = new Font("Segoe UI", 9F),
            Text = "Ready"
        };
        this.Controls.Add(_lblStatus);
    }

    private ToolStrip CreateToolbar()
    {
        var toolbar = new ToolStrip
        {
            Dock = DockStyle.Top,
            GripStyle = ToolStripGripStyle.Hidden,
            Renderer = new ToolStripProfessionalRenderer(new CustomColorTable())
        };

        var btnAdd = new ToolStripButton("➕ Add Page") { ForeColor = Color.FromArgb(60, 60, 80) };
        btnAdd.Click += BtnAdd_Click;

        var btnDuplicate = new ToolStripButton("📋 Duplicate") { ForeColor = Color.FromArgb(60, 60, 80) };
        btnDuplicate.Click += BtnDuplicate_Click;

        var btnRemove = new ToolStripButton("❌ Remove") { ForeColor = Color.FromArgb(180, 60, 60) };
        btnRemove.Click += BtnRemove_Click;

        var btnMoveUp = new ToolStripButton("⬆") { ForeColor = Color.FromArgb(60, 60, 80) };
        btnMoveUp.Click += BtnMoveUp_Click;

        var btnMoveDown = new ToolStripButton("⬇") { ForeColor = Color.FromArgb(60, 60, 80) };
        btnMoveDown.Click += BtnMoveDown_Click;

        var sep1 = new ToolStripSeparator();

        var btnSave = new ToolStripButton("💾 Save Project") { ForeColor = Color.FromArgb(60, 60, 80) };
        btnSave.Click += BtnSave_Click;

        var btnLoad = new ToolStripButton("📂 Load Project") { ForeColor = Color.FromArgb(60, 60, 80) };
        btnLoad.Click += BtnLoad_Click;

        var sep2 = new ToolStripSeparator();

        var btnGenerate = new ToolStripButton("📄 Generate PDF") { ForeColor = Color.FromArgb(40, 120, 60), Font = new Font("Segoe UI", 9F, FontStyle.Bold) };
        btnGenerate.Click += BtnGenerate_Click;

        toolbar.Items.AddRange(new ToolStripItem[] { btnAdd, btnDuplicate, btnRemove, btnMoveUp, btnMoveDown, sep1, btnSave, btnLoad, sep2, btnGenerate });
        return toolbar;
    }

    private Panel CreateColorCodedPanel(string title, Color bgColor, Color borderColor, Control innerControl)
    {
        var container = new Panel
        {
            BackColor = bgColor,
            MinimumSize = new Size(310, 0),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 0, 0, 10),
            Padding = new Padding(1)
        };

        container.Paint += (s, e) =>
        {
            using var pen = new Pen(borderColor, 1.5f);
            e.Graphics.DrawRectangle(pen, 0, 0, container.Width - 1, container.Height - 1);
        };

        var flow = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            FlowDirection = FlowDirection.TopDown,
            Padding = new Padding(8, 6, 8, 8),
            BackColor = bgColor,
            MinimumSize = new Size(308, 0)
        };

        var lblHeader = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(40, 50, 70),
            AutoSize = true,
            Padding = new Padding(0, 0, 0, 6)
        };
        flow.Controls.Add(lblHeader);
        flow.Controls.Add(innerControl);

        container.Controls.Add(flow);

        void UpdateWidth()
        {
            if (container.Parent != null && container.Parent.ClientSize.Width > 0)
            {
                int targetWidth = container.Parent.ClientSize.Width - container.Margin.Horizontal - container.Parent.Padding.Horizontal - 6;
                if (targetWidth >= 310)
                {
                    container.Width = targetWidth;
                    flow.Width = targetWidth - container.Padding.Horizontal;
                }
            }
        }

        container.ParentChanged += (s, e) =>
        {
            if (container.Parent != null)
            {
                container.Parent.SizeChanged += (ps, pe) => UpdateWidth();
                UpdateWidth();
            }
        };

        return container;
    }

    private Panel CreateSettingsPanel()
    {
        var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White, Padding = new Padding(2) };

        _settingsTabControl = new TabControl { Dock = DockStyle.Fill };

        // --- TAB 1: LAYOUT ---
        var tpLayout = new TabPage("Layout") { AutoScroll = true, Padding = new Padding(8), BackColor = Color.White };
        var flowLayout = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };

        // Sub-Panel 1: Page Template & Dimensions (Light Sky Blue)
        var layoutInner = new FlowLayoutPanel { AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };
        layoutInner.Controls.Add(CreateLabeledCombo("Page Type:", out _cboPageType, new[] { "Tianzige Grid (田字格)", "Jiugongge Grid (九宫格)", "Essay Grid (作文格)", "Vocabulary (生字本)", "Lined", "Blank", "Custom" }));
        _cboPageType.SelectedIndexChanged += (s, e) => OnSettingsChanged();

        layoutInner.Controls.Add(CreateLabeledCombo("Page Size:", out _cboPageSize, PageSizeInfo.All.Select(p => p.Name).ToArray()));
        _cboPageSize.SelectedIndexChanged += (s, e) => OnPageSizeChanged();

        _chkLandscape = new CheckBox { Text = "Landscape Orientation", AutoSize = true, Padding = new Padding(5, 5, 0, 5) };
        _chkLandscape.CheckedChanged += (s, e) => OnSettingsChanged();
        layoutInner.Controls.Add(_chkLandscape);

        var pageLabelPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(0, 5, 0, 0) };
        pageLabelPanel.Controls.Add(CreateLabeledText("Page Label:", out _txtPageLabel, "", 180));
        _txtPageLabel.TextChanged += (s, e) => OnSettingsChanged();
        layoutInner.Controls.Add(pageLabelPanel);

        flowLayout.Controls.Add(CreateColorCodedPanel("📄 Page Template & Size", Color.FromArgb(235, 243, 252), Color.FromArgb(140, 180, 220), layoutInner));

        // Sub-Panel 2: Margins (mm) (Light Mint Green)
        var marginInner = new FlowLayoutPanel { AutoSize = true, WrapContents = true, FlowDirection = FlowDirection.LeftToRight };
        var m1 = CreateLabeledNud("Top:", out _nudMarginTop, 0, 100, 15, 1);
        var m2 = CreateLabeledNud("Bottom:", out _nudMarginBottom, 0, 100, 15, 1);
        var m3 = CreateLabeledNud("Left:", out _nudMarginLeft, 0, 100, 15, 1);
        var m4 = CreateLabeledNud("Right:", out _nudMarginRight, 0, 100, 15, 1);

        foreach (NumericUpDown nud in new[] { _nudMarginTop, _nudMarginBottom, _nudMarginLeft, _nudMarginRight })
            nud.ValueChanged += (s, e) => OnSettingsChanged();

        marginInner.Controls.Add(m1);
        marginInner.Controls.Add(m2);
        marginInner.Controls.Add(m3);
        marginInner.Controls.Add(m4);

        void UpdateMarginLayout()
        {
            if (marginInner.Parent == null) return;
            int parentWidth = marginInner.Parent.ClientSize.Width - marginInner.Parent.Padding.Horizontal - marginInner.Margin.Horizontal;
            if (parentWidth <= 0) return;

            marginInner.Width = parentWidth;
            int availW = parentWidth - marginInner.Padding.Horizontal - 10;

            if (availW >= 560)
            {
                // 4 items on 1 row (~24% each)
                int itemW = (availW - 18) / 4;
                m1.Width = itemW; m2.Width = itemW; m3.Width = itemW; m4.Width = itemW;
            }
            else if (availW >= 270)
            {
                // 2 items per row (~48% each)
                int itemW = (availW - 10) / 2;
                m1.Width = itemW; m2.Width = itemW; m3.Width = itemW; m4.Width = itemW;
            }
            else
            {
                // 1 item per row (100% stacked)
                m1.Width = availW; m2.Width = availW; m3.Width = availW; m4.Width = availW;
            }
        }

        marginInner.SizeChanged += (s, e) => UpdateMarginLayout();
        marginInner.ParentChanged += (s, e) =>
        {
            if (marginInner.Parent != null)
            {
                marginInner.Parent.SizeChanged += (ps, pe) => UpdateMarginLayout();
                UpdateMarginLayout();
            }
        };

        flowLayout.Controls.Add(CreateColorCodedPanel("📐 Page Margins (mm)", Color.FromArgb(238, 248, 238), Color.FromArgb(140, 200, 140), marginInner));

        // Sub-Panel 3: Global Actions (Neutral Gray)
        var actionInner = new FlowLayoutPanel { AutoSize = true, WrapContents = true, FlowDirection = FlowDirection.LeftToRight };
        _btnApplyToAll = new Button { Text = "Apply to All Pages", Width = 142, Height = 28, BackColor = Color.FromArgb(60, 60, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnApplyToAll.FlatAppearance.BorderSize = 0;
        _btnApplyToAll.Click += BtnApplyToAll_Click;

        _btnResetDefaults = new Button { Text = "Reset Defaults", Width = 142, Height = 28, BackColor = Color.FromArgb(180, 180, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnResetDefaults.FlatAppearance.BorderSize = 0;
        _btnResetDefaults.Click += BtnResetDefaults_Click;

        _btnMultiply = new Button { Text = "Multiply Current Page", Width = 288, Height = 28, BackColor = Color.FromArgb(40, 120, 60), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 5, 0, 0) };
        _btnMultiply.FlatAppearance.BorderSize = 0;
        _btnMultiply.Click += BtnMultiply_Click;

        actionInner.Controls.Add(_btnApplyToAll);
        actionInner.Controls.Add(_btnResetDefaults);
        actionInner.Controls.Add(_btnMultiply);

        flowLayout.Controls.Add(CreateColorCodedPanel("⚡ Page Actions", Color.FromArgb(245, 245, 250), Color.FromArgb(180, 180, 200), actionInner));

        tpLayout.Controls.Add(flowLayout);

        // --- TAB 2: GRID / LINES ---
        var tpGrid = new TabPage("Grid / Lines") { AutoScroll = true, Padding = new Padding(10), BackColor = Color.White };
        var flowGrid = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };

        // Sub-Panel 4: Grid Dimensions (Light Warm Amber)
        var gridFlow = new FlowLayoutPanel { AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };

        var gridSettings = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        gridSettings.Controls.Add(CreateLabeledNud("Cell Size (mm):", out _nudCellSize, 5, 100, 15, 1));
        gridSettings.Controls.Add(CreateLabeledNud("Cell Gap:", out _nudCellSpacing, 0, 50, 0, 0.5m));
        gridSettings.Controls.Add(CreateLabeledNud("Row Gap:", out _nudLineSpacing, 0, 50, 8, 0.5m));
        gridSettings.Controls.Add(CreateLabeledNud("Col Gap:", out _nudColSpacing, 0, 50, 8, 0.5m));
        foreach (NumericUpDown nud in new[] { _nudCellSize, _nudCellSpacing, _nudLineSpacing, _nudColSpacing })
            nud.ValueChanged += (s, e) => OnSettingsChanged();
        gridFlow.Controls.Add(gridSettings);

        var headersPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(0, 5, 0, 0) };
        headersPanel.Controls.Add(CreateLabeledText("Headers:", out _txtColumnHeaders, "", 150));
        _txtColumnHeaders.TextChanged += (s, e) => OnSettingsChanged();
        headersPanel.Controls.Add(CreateLabeledNud("Rows:", out _nudGridRows, 0, 50, 0, 1));
        headersPanel.Controls.Add(CreateLabeledNud("Cols:", out _nudGridCols, 0, 20, 0, 1));
        _nudGridRows.ValueChanged += (s, e) => OnSettingsChanged();
        _nudGridCols.ValueChanged += (s, e) => OnSettingsChanged();
        gridFlow.Controls.Add(headersPanel);

        _gridPanel = CreateColorCodedPanel("📏 Grid Cell Settings", Color.FromArgb(254, 249, 230), Color.FromArgb(230, 190, 120), gridFlow);
        flowGrid.Controls.Add(_gridPanel);

        // Sub-Panel 5: Lined Page Rules (Light Lavender/Blue)
        var linedFlow = new FlowLayoutPanel { AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };
        var linedSettings = new FlowLayoutPanel { AutoSize = true, WrapContents = false };
        linedSettings.Controls.Add(CreateLabeledNud("Line H (mm):", out _nudLineHeight, 2, 30, 8, 0.5m));
        linedSettings.Controls.Add(CreateColorButton("Color:", out _btnLineColor, Color.FromArgb(180, 200, 220)));
        linedSettings.Controls.Add(CreateLabeledNud("Width:", out _nudLineWidth, 0.1m, 3, 0.5m, 0.1m));
        _nudLineHeight.ValueChanged += (s, e) => OnSettingsChanged();
        _nudLineWidth.ValueChanged += (s, e) => OnSettingsChanged();
        linedFlow.Controls.Add(linedSettings);

        _chkShowMarginLine = new CheckBox { Text = "Show Margin Line", AutoSize = true, Padding = new Padding(5, 5, 10, 5), Checked = true };
        _chkShowMarginLine.CheckedChanged += (s, e) => OnSettingsChanged();
        linedFlow.Controls.Add(_chkShowMarginLine);

        _linedPanel = CreateColorCodedPanel("≡ Lined Page Rules", Color.FromArgb(240, 244, 255), Color.FromArgb(160, 180, 220), linedFlow);
        _linedPanel.Visible = false;
        flowGrid.Controls.Add(_linedPanel);

        tpGrid.Controls.Add(flowGrid);

        // --- TAB 3: STYLE ---
        var tpStyle = new TabPage("Style") { AutoScroll = true, Padding = new Padding(10), BackColor = Color.White };
        var flowStyle = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };

        var styleInner = new FlowLayoutPanel { AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };

        var colorPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(0, 5, 0, 0) };
        colorPanel.Controls.Add(CreateColorButton("Border:", out _btnBorderColor, Color.FromArgb(40, 40, 40)));
        colorPanel.Controls.Add(CreateLabeledNud("W:", out _nudBorderWidth, 0.1m, 5, 1.5m, 0.1m));
        colorPanel.Controls.Add(CreateColorButton("Guide:", out _btnGuideColor, Color.FromArgb(180, 60, 60)));
        colorPanel.Controls.Add(CreateLabeledNud("W:", out _nudGuideWidth, 0.1m, 3, 0.6m, 0.1m));
        _nudBorderWidth.ValueChanged += (s, e) => OnSettingsChanged();
        _nudGuideWidth.ValueChanged += (s, e) => OnSettingsChanged();
        styleInner.Controls.Add(colorPanel);

        var advancedGridPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(0, 5, 0, 0) };
        advancedGridPanel.Controls.Add(CreateColorButton("Cell BG:", out _btnCellBgColor, Color.White));
        advancedGridPanel.Controls.Add(CreateColorButton("Page BG:", out _btnPageBgColor, Color.White));
        advancedGridPanel.Controls.Add(CreateLabeledNud("Corner:", out _nudCornerRadius, 0, 20, 0, 0.5m));
        advancedGridPanel.Controls.Add(CreateLabeledNud("Padding:", out _nudInnerPadding, 0, 10, 0, 0.5m));
        _nudCornerRadius.ValueChanged += (s, e) => OnSettingsChanged();
        _nudInnerPadding.ValueChanged += (s, e) => OnSettingsChanged();
        styleInner.Controls.Add(advancedGridPanel);

        var styleTogglePanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(0, 5, 0, 0) };
        _chkDashed = new CheckBox { Text = "Dashed Guides", AutoSize = true, Checked = true, Padding = new Padding(5, 5, 5, 5) };
        _chkDashed.CheckedChanged += (s, e) => OnSettingsChanged();
        _chkDotted = new CheckBox { Text = "Dotted Guides", AutoSize = true, Checked = false, Padding = new Padding(5, 5, 5, 5) };
        _chkDotted.CheckedChanged += (s, e) => OnSettingsChanged();
        _chkDiagonals = new CheckBox { Text = "Diagonal (米)", AutoSize = true, Padding = new Padding(5, 5, 5, 5) };
        _chkDiagonals.CheckedChanged += (s, e) => OnSettingsChanged();
        styleTogglePanel.Controls.Add(_chkDashed);
        styleTogglePanel.Controls.Add(_chkDotted);
        styleTogglePanel.Controls.Add(_chkDiagonals);
        styleInner.Controls.Add(styleTogglePanel);

        var shadingPanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(0, 5, 0, 0) };
        _chkCellShading = new CheckBox { Text = "Cell Shading", AutoSize = true, Padding = new Padding(5, 5, 10, 5) };
        _chkCellShading.CheckedChanged += (s, e) => OnSettingsChanged();
        shadingPanel.Controls.Add(_chkCellShading);
        shadingPanel.Controls.Add(CreateColorButton("Shade Color:", out _btnCellShadingColor, Color.FromArgb(250, 248, 240)));
        styleInner.Controls.Add(shadingPanel);

        flowStyle.Controls.Add(CreateColorCodedPanel("🎨 Visual Style & Colors", Color.FromArgb(248, 240, 250), Color.FromArgb(200, 150, 220), styleInner));

        tpStyle.Controls.Add(flowStyle);

        // --- TAB 4: COVER / ENDING ---
        var tpCover = new TabPage("Cover/Ending") { AutoScroll = true, Padding = new Padding(10), BackColor = Color.White };
        var flowCover = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };

        // Sub-Panel 6: Cover Image & Standard Text (Light Soft Teal)
        var coverTextInner = new FlowLayoutPanel { AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };

        var imagePanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(0, 0, 0, 10) };
        imagePanel.Controls.Add(new Label { Text = "Cover Image:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), AutoSize = true, Padding = new Padding(0, 5, 5, 0) });
        _btnSelectCoverImage = new Button { Text = "Select Image...", Width = 110, Height = 25, BackColor = Color.FromArgb(240, 240, 245), FlatStyle = FlatStyle.Flat };
        _btnSelectCoverImage.Click += BtnSelectCoverImage_Click;
        _btnClearCoverImage = new Button { Text = "Clear", Width = 60, Height = 25, BackColor = Color.FromArgb(240, 240, 245), FlatStyle = FlatStyle.Flat };
        _btnClearCoverImage.Click += (s, e) =>
        {
            if (_selectedPageIndex >= 0)
            {
                var page = _project.Pages[_selectedPageIndex];
                page.Elements.RemoveAll(x => x is CustomImageElement);
                OnSettingsChanged();
            }
        };
        imagePanel.Controls.Add(_btnSelectCoverImage);
        imagePanel.Controls.Add(_btnClearCoverImage);
        coverTextInner.Controls.Add(imagePanel);

        coverTextInner.Controls.Add(CreateLabeledText("Title:", out _txtTitle, "汉字书写练习", 200));
        coverTextInner.Controls.Add(CreateLabeledText("Subtitle:", out _txtSubtitle, "Chinese Writing Practice", 200));
        coverTextInner.Controls.Add(CreateLabeledText("Author:", out _txtAuthor, "", 200));
        coverTextInner.Controls.Add(CreateLabeledText("Notes:", out _txtNotes, "", 200, true));
        foreach (TextBox txt in new[] { _txtTitle, _txtSubtitle, _txtAuthor, _txtNotes })
            txt.TextChanged += (s, e) => OnSettingsChanged();

        var fontSizePanel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(0, 5, 0, 0) };
        fontSizePanel.Controls.Add(new Label { Text = "Sizes:", Font = new Font("Segoe UI", 8F, FontStyle.Italic), AutoSize = true, Padding = new Padding(0, 5, 5, 0) });
        fontSizePanel.Controls.Add(CreateLabeledNud("Title:", out _nudTitleFontSize, 8, 72, 36, 1));
        fontSizePanel.Controls.Add(CreateLabeledNud("Sub:", out _nudSubtitleFontSize, 6, 36, 14, 1));
        fontSizePanel.Controls.Add(CreateLabeledNud("Auth:", out _nudAuthorFontSize, 6, 36, 14, 1));
        fontSizePanel.Controls.Add(CreateLabeledNud("Notes:", out _nudNotesFontSize, 6, 24, 10, 1));
        foreach (var nud in new[] { _nudTitleFontSize, _nudSubtitleFontSize, _nudAuthorFontSize, _nudNotesFontSize })
            nud.ValueChanged += (s, e) => OnSettingsChanged();
        coverTextInner.Controls.Add(fontSizePanel);

        _coverPanel = CreateColorCodedPanel("🖼️ Cover Image & Standard Text", Color.FromArgb(235, 248, 248), Color.FromArgb(130, 200, 200), coverTextInner);
        flowCover.Controls.Add(_coverPanel);

        // Sub-Panel 7: Custom Text Blocks (Light Soft Coral/Peach)
        _customTextFlow = new FlowLayoutPanel { AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };

        var customHeader = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(0, 0, 0, 5) };
        customHeader.Controls.Add(new Label { Text = "Dynamic Blocks:", Font = new Font("Segoe UI", 9F, FontStyle.Bold), AutoSize = true, Padding = new Padding(0, 5, 5, 0) });
        _btnAddCustomText = new Button { Text = "➕ Add Text Block", Width = 140, Height = 25, BackColor = Color.FromArgb(80, 140, 190), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnAddCustomText.FlatAppearance.BorderSize = 0;
        _btnAddCustomText.Click += BtnAddCustomText_Click;
        customHeader.Controls.Add(_btnAddCustomText);
        _customTextFlow.Controls.Add(customHeader);

        flowCover.Controls.Add(CreateColorCodedPanel("📝 Custom Text Blocks", Color.FromArgb(255, 243, 235), Color.FromArgb(240, 160, 120), _customTextFlow));

        tpCover.Controls.Add(flowCover);

        // Add all tabs
        _settingsTabControl.TabPages.Add(tpLayout);
        _settingsTabControl.TabPages.Add(tpGrid);
        _settingsTabControl.TabPages.Add(tpStyle);
        _settingsTabControl.TabPages.Add(tpCover);

        panel.Controls.Add(_settingsTabControl);

        _settingsFlow = new FlowLayoutPanel();
        return panel;
    }

    private FlowLayoutPanel CreateLabeledNud(string label, out NumericUpDown nud, decimal min, decimal max, decimal value, decimal increment)
    {
        var panel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(2), Margin = new Padding(0) };
        var lbl = new Label { Text = label, AutoSize = true, Padding = new Padding(0, 5, 5, 0), Anchor = AnchorStyles.Left | AnchorStyles.Top };
        nud = new NumericUpDown { Minimum = min, Maximum = max, Value = value, Increment = increment, Width = 55, DecimalPlaces = increment < 1 ? 1 : 0 };
        panel.Controls.Add(lbl);
        panel.Controls.Add(nud);
        return panel;
    }

    private FlowLayoutPanel CreateLabeledCombo(string label, out ComboBox cbo, string[] items)
    {
        var panel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(2), Margin = new Padding(0) };
        var lbl = new Label { Text = label, AutoSize = true, Padding = new Padding(0, 5, 5, 0), Anchor = AnchorStyles.Left | AnchorStyles.Top };
        cbo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
        cbo.Items.AddRange(items);
        if (items.Length > 0) cbo.SelectedIndex = 0;
        panel.Controls.Add(lbl);
        panel.Controls.Add(cbo);
        return panel;
    }

    private FlowLayoutPanel CreateLabeledText(string label, out TextBox txt, string defaultValue, int width, bool multiline = false)
    {
        var panel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(2), Margin = new Padding(0) };
        var lbl = new Label { Text = label, AutoSize = true, Padding = new Padding(0, 5, 5, 0), Anchor = AnchorStyles.Left | AnchorStyles.Top };
        txt = new TextBox { Width = width, Text = defaultValue };
        if (multiline) { txt.Multiline = true; txt.Height = 60; }
        panel.Controls.Add(lbl);
        panel.Controls.Add(txt);
        return panel;
    }

    private FlowLayoutPanel CreateColorButton(string label, out Button btn, Color initialColor)
    {
        var panel = new FlowLayoutPanel { AutoSize = true, WrapContents = false, Padding = new Padding(2), Margin = new Padding(0) };
        var lbl = new Label { Text = label, AutoSize = true, Padding = new Padding(0, 5, 5, 0), Anchor = AnchorStyles.Left | AnchorStyles.Top };
        var button = new Button { Width = 70, Height = 23, BackColor = initialColor, FlatStyle = FlatStyle.Flat };
        button.FlatAppearance.BorderColor = Color.Gray;
        button.Tag = initialColor;
        button.Click += (s, e) =>
        {
            using var dlg = new ColorDialog { Color = (Color)button.Tag! };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                button.BackColor = dlg.Color;
                button.Tag = dlg.Color;
                OnSettingsChanged();
            }
        };
        panel.Controls.Add(lbl);
        panel.Controls.Add(button);
        btn = button;
        return panel;
    }

    private void RefreshPageList()
    {
        _pageList.Items.Clear();
        for (int i = 0; i < _project.Pages.Count; i++)
            _pageList.Items.Add($"{i + 1}. {_project.Pages[i].DisplayName}");
        
        if (_lblPageCount != null)
            _lblPageCount.Text = $"{_project.Pages.Count} page{(_project.Pages.Count != 1 ? "s" : "")}";
    }

    private void PageList_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _selectedPageIndex = _pageList.SelectedIndex;
        if (_selectedPageIndex < 0 || _selectedPageIndex >= _project.Pages.Count) { _preview.PageSettings = null; return; }
        LoadPageToUI(_project.Pages[_selectedPageIndex]);
    }

    private void LoadPageToUI(PageSettings page)
    {
        _updatingControls = true;
        try
        {
            _cboPageType.SelectedIndex = page.Type switch
            {
                PageType.TianzigeGrid => 0, PageType.JiugonggeGrid => 1,
                PageType.EssayGrid => 2, PageType.Vocabulary => 3,
                PageType.Lined => 4, PageType.Blank => 5, PageType.Custom => 6, _ => 0
            };
            var pageSize = PageSizeInfo.All.FirstOrDefault(p => p.Name == page.PageSizeName);
            _cboPageSize.SelectedIndex = pageSize != null ? Array.IndexOf(PageSizeInfo.All.ToArray(), pageSize) : 1;
            _chkLandscape.Checked = page.Landscape;
            _nudMarginTop.Value = (decimal)page.MarginTopMm;
            _nudMarginBottom.Value = (decimal)page.MarginBottomMm;
            _nudMarginLeft.Value = (decimal)page.MarginLeftMm;
            _nudMarginRight.Value = (decimal)page.MarginRightMm;
            _nudCellSize.Value = (decimal)page.CellSizeMm;
            _nudCellSpacing.Value = (decimal)page.CellSpacingMm;
            _nudLineSpacing.Value = (decimal)page.LineSpacingMm;
            _nudColSpacing.Value = (decimal)page.ColumnSpacingMm;
            _chkDashed.Checked = page.Style.DashedGuides;
            _chkDotted.Checked = page.Style.DottedGuides;
            _chkDiagonals.Checked = page.Style.DrawDiagonals;
            _btnBorderColor.BackColor = page.Style.BorderColor;
            _btnBorderColor.Tag = page.Style.BorderColor;
            _btnGuideColor.BackColor = page.Style.GuideColor;
            _btnGuideColor.Tag = page.Style.GuideColor;
            _nudBorderWidth.Value = (decimal)page.Style.BorderWidthPt;
            _nudGuideWidth.Value = (decimal)page.Style.GuideWidthPt;
            _nudLineHeight.Value = (decimal)page.LineHeightMm;
            _btnLineColor.BackColor = page.LineColor;
            _btnLineColor.Tag = page.LineColor;
            _txtTitle.Text = page.Title;
            _txtSubtitle.Text = page.Subtitle;
            _txtAuthor.Text = page.AuthorName;
            _txtNotes.Text = page.Notes;
            
            // Load new properties
            if (_nudTitleFontSize != null) _nudTitleFontSize.Value = (decimal)page.TitleFontSizePt;
            if (_nudSubtitleFontSize != null) _nudSubtitleFontSize.Value = (decimal)page.SubtitleFontSizePt;
            if (_nudAuthorFontSize != null) _nudAuthorFontSize.Value = (decimal)page.AuthorFontSizePt;
            if (_nudNotesFontSize != null) _nudNotesFontSize.Value = (decimal)page.NotesFontSizePt;
            if (_nudLineWidth != null) _nudLineWidth.Value = (decimal)page.LineWidthPt;
            if (_chkShowMarginLine != null) _chkShowMarginLine.Checked = page.ShowMarginLine;
            if (_nudCornerRadius != null) _nudCornerRadius.Value = (decimal)page.Style.CornerRadiusPt;
            if (_nudInnerPadding != null) _nudInnerPadding.Value = (decimal)page.Style.CellInnerPaddingMm;
            if (_chkCellShading != null) _chkCellShading.Checked = page.Style.ShowCellShading;
            if (_btnCellShadingColor != null) { _btnCellShadingColor.BackColor = page.Style.CellShadingColor; _btnCellShadingColor.Tag = page.Style.CellShadingColor; }
            if (_btnCellBgColor != null) { _btnCellBgColor.BackColor = page.Style.CellBackground; _btnCellBgColor.Tag = page.Style.CellBackground; }
            if (_btnPageBgColor != null) { _btnPageBgColor.BackColor = page.Style.PageBackground; _btnPageBgColor.Tag = page.Style.PageBackground; }
            if (_txtColumnHeaders != null) _txtColumnHeaders.Text = page.ColumnHeaders;
            if (_txtPageLabel != null) _txtPageLabel.Text = page.PageLabel;
            if (_nudGridRows != null) _nudGridRows.Value = page.GridRowsOverride;
            if (_nudGridCols != null) _nudGridCols.Value = page.GridColsOverride;

            RefreshCustomTextUI(page);

            UpdatePanelVisibility();
            _preview.PageSettings = page;
            _preview.Invalidate();
        }
        finally { _updatingControls = false; }
    }

    private void UpdatePanelVisibility()
    {
        var pageType = _cboPageType.SelectedIndex switch
        {
            0 => PageType.TianzigeGrid, 1 => PageType.JiugonggeGrid,
            2 => PageType.EssayGrid, 3 => PageType.Vocabulary,
            4 => PageType.Lined, 5 => PageType.Blank, 6 => PageType.Custom, _ => PageType.TianzigeGrid
        };
        _gridPanel.Visible = pageType is PageType.TianzigeGrid or PageType.JiugonggeGrid or PageType.EssayGrid or PageType.Vocabulary;
        _linedPanel.Visible = pageType == PageType.Lined;
        _coverPanel.Visible = true; // Always visible on Cover/Ending tab
    }

    private void OnSettingsChanged()
    {
        if (_updatingControls || _selectedPageIndex < 0 || _selectedPageIndex >= _project.Pages.Count) return;
        var page = _project.Pages[_selectedPageIndex];
        page.Type = _cboPageType.SelectedIndex switch
        {
            0 => PageType.TianzigeGrid, 1 => PageType.JiugonggeGrid,
            2 => PageType.EssayGrid, 3 => PageType.Vocabulary,
            4 => PageType.Lined, 5 => PageType.Blank, 6 => PageType.Custom, _ => PageType.TianzigeGrid
        };
        page.Landscape = _chkLandscape.Checked;
        page.MarginTopMm = (float)_nudMarginTop.Value;
        page.MarginBottomMm = (float)_nudMarginBottom.Value;
        page.MarginLeftMm = (float)_nudMarginLeft.Value;
        page.MarginRightMm = (float)_nudMarginRight.Value;
        page.CellSizeMm = (float)_nudCellSize.Value;
        page.CellSpacingMm = (float)_nudCellSpacing.Value;
        page.LineSpacingMm = (float)_nudLineSpacing.Value;
        page.ColumnSpacingMm = (float)_nudColSpacing.Value;
        page.Style.DashedGuides = _chkDashed.Checked;
        page.Style.DottedGuides = _chkDotted.Checked;
        page.Style.DrawDiagonals = _chkDiagonals.Checked;
        page.Style.BorderColor = (Color)_btnBorderColor.Tag!;
        page.Style.GuideColor = (Color)_btnGuideColor.Tag!;
        page.Style.BorderWidthPt = (float)_nudBorderWidth.Value;
        page.Style.GuideWidthPt = (float)_nudGuideWidth.Value;
        page.LineHeightMm = (float)_nudLineHeight.Value;
        page.LineColor = (Color)_btnLineColor.Tag!;
        page.Title = _txtTitle.Text;
        page.Subtitle = _txtSubtitle.Text;
        page.AuthorName = _txtAuthor.Text;
        page.Notes = _txtNotes.Text;
        
        // Save new properties
        if (_nudTitleFontSize != null) page.TitleFontSizePt = (float)_nudTitleFontSize.Value;
        if (_nudSubtitleFontSize != null) page.SubtitleFontSizePt = (float)_nudSubtitleFontSize.Value;
        if (_nudAuthorFontSize != null) page.AuthorFontSizePt = (float)_nudAuthorFontSize.Value;
        if (_nudNotesFontSize != null) page.NotesFontSizePt = (float)_nudNotesFontSize.Value;
        if (_nudLineWidth != null) page.LineWidthPt = (float)_nudLineWidth.Value;
        if (_chkShowMarginLine != null) page.ShowMarginLine = _chkShowMarginLine.Checked;
        if (_nudCornerRadius != null) page.Style.CornerRadiusPt = (float)_nudCornerRadius.Value;
        if (_nudInnerPadding != null) page.Style.CellInnerPaddingMm = (float)_nudInnerPadding.Value;
        if (_chkCellShading != null) page.Style.ShowCellShading = _chkCellShading.Checked;
        if (_btnCellShadingColor != null) page.Style.CellShadingColor = (Color)_btnCellShadingColor.Tag!;
        if (_btnCellBgColor != null) page.Style.CellBackground = (Color)_btnCellBgColor.Tag!;
        if (_btnPageBgColor != null) page.Style.PageBackground = (Color)_btnPageBgColor.Tag!;
        if (_txtColumnHeaders != null) page.ColumnHeaders = _txtColumnHeaders.Text;
        if (_txtPageLabel != null) page.PageLabel = _txtPageLabel.Text;
        if (_nudGridRows != null) page.GridRowsOverride = (int)_nudGridRows.Value;
        if (_nudGridCols != null) page.GridColsOverride = (int)_nudGridCols.Value;
        
        UpdatePanelVisibility();
        RefreshPageList();
        _pageList.SelectedIndex = _selectedPageIndex;
        _preview.Invalidate();
    }

    private void OnPageSizeChanged()
    {
        if (_updatingControls || _selectedPageIndex < 0 || _selectedPageIndex >= _project.Pages.Count) return;
        var page = _project.Pages[_selectedPageIndex];
        if (_cboPageSize.SelectedIndex >= 0 && _cboPageSize.SelectedIndex < PageSizeInfo.All.Count)
        {
            var size = PageSizeInfo.All[_cboPageSize.SelectedIndex];
            page.PageSizeName = size.Name;
            page.PageWidthMm = size.WidthMm;
            page.PageHeightMm = size.HeightMm;
        }
        OnSettingsChanged();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        _project.Pages.Add(new PageSettings { Type = PageType.TianzigeGrid });
        RefreshPageList();
        _pageList.SelectedIndex = _project.Pages.Count - 1;
        _lblStatus.Text = $"Added page {_project.Pages.Count}";
    }

    private void BtnDuplicate_Click(object? sender, EventArgs e)
    {
        if (_selectedPageIndex < 0) return;
        _project.Pages.Insert(_selectedPageIndex + 1, _project.Pages[_selectedPageIndex].Clone());
        RefreshPageList();
        _pageList.SelectedIndex = _selectedPageIndex + 1;
    }

    private void BtnRemove_Click(object? sender, EventArgs e)
    {
        if (_selectedPageIndex < 0) return;
        if (MessageBox.Show("Remove this page?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            _project.Pages.RemoveAt(_selectedPageIndex);
            RefreshPageList();
            if (_project.Pages.Count > 0) _pageList.SelectedIndex = Math.Min(_selectedPageIndex, _project.Pages.Count - 1);
        }
    }

    private void BtnMoveUp_Click(object? sender, EventArgs e)
    {
        if (_selectedPageIndex <= 0) return;
        var p = _project.Pages[_selectedPageIndex];
        _project.Pages.RemoveAt(_selectedPageIndex);
        _project.Pages.Insert(_selectedPageIndex - 1, p);
        RefreshPageList();
        _pageList.SelectedIndex = _selectedPageIndex - 1;
    }

    private void BtnMoveDown_Click(object? sender, EventArgs e)
    {
        if (_selectedPageIndex < 0 || _selectedPageIndex >= _project.Pages.Count - 1) return;
        var p = _project.Pages[_selectedPageIndex];
        _project.Pages.RemoveAt(_selectedPageIndex);
        _project.Pages.Insert(_selectedPageIndex + 1, p);
        RefreshPageList();
        _pageList.SelectedIndex = _selectedPageIndex + 1;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        using var dlg = new SaveFileDialog { Filter = "Tianzige Project (*.tzp)|*.tzp", FileName = _project.Name + ".tzp" };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            _project.SaveToFile(dlg.FileName);
            _lblStatus.Text = $"Saved: {dlg.FileName}";
        }
    }

    private void BtnLoad_Click(object? sender, EventArgs e)
    {
        using var dlg = new OpenFileDialog { Filter = "Tianzige Project (*.tzp)|*.tzp" };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            try
            {
                _project = Project.LoadFromFile(dlg.FileName);
                RefreshPageList();
                if (_project.Pages.Count > 0) _pageList.SelectedIndex = 0;
                _lblStatus.Text = $"Loaded: {dlg.FileName}";
            }
            catch (Exception ex) { MessageBox.Show($"Load failed: {ex.Message}"); }
        }
    }

    private void BtnGenerate_Click(object? sender, EventArgs e)
    {
        if (_project.Pages.Count == 0) { MessageBox.Show("No pages!"); return; }
        using var dlg = new SaveFileDialog { Filter = "PDF (*.pdf)|*.pdf", FileName = _project.Name + ".pdf" };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            try
            {
                _lblStatus.Text = "Generating...";
                Cursor = Cursors.WaitCursor;
                PdfGenerator.Generate(_project, dlg.FileName);
                Cursor = Cursors.Default;
                _lblStatus.Text = $"PDF: {dlg.FileName}";
                if (MessageBox.Show("PDF generated! Open it?", "Success", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo { FileName = dlg.FileName, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                Cursor = Cursors.Default;
                MessageBox.Show($"PDF failed: {ex.Message}");
            }
        }
    }

    private void BtnApplyToAll_Click(object? sender, EventArgs e)
    {
        if (_selectedPageIndex < 0) return;
        if (MessageBox.Show("Apply current page settings to all pages?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
            return;
        
        var source = _project.Pages[_selectedPageIndex];
        for (int i = 0; i < _project.Pages.Count; i++)
        {
            if (i != _selectedPageIndex)
            {
                _project.Pages[i] = source.Clone();
            }
        }
        RefreshPageList();
        _pageList.SelectedIndex = _selectedPageIndex;
        _lblStatus.Text = "Settings applied to all pages";
    }

    private void BtnResetDefaults_Click(object? sender, EventArgs e)
    {
        if (_selectedPageIndex < 0) return;
        if (MessageBox.Show("Reset this page to default settings?", "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
            return;
        
        var page = _project.Pages[_selectedPageIndex];
        var defaults = new PageSettings { Type = page.Type };
        defaults.Style = new GridStyle();
        _project.Pages[_selectedPageIndex] = defaults;
        LoadPageToUI(defaults);
        _preview.Invalidate();
        _lblStatus.Text = "Page reset to defaults";
    }

    private void BtnMultiply_Click(object? sender, EventArgs e)
    {
        if (_selectedPageIndex < 0) return;

        int count = ShowMultiplyDialog();
        if (count <= 0) return;
        
        var source = _project.Pages[_selectedPageIndex];
        for (int i = 0; i < count; i++)
        {
            _project.Pages.Insert(_selectedPageIndex + 1 + i, source.Clone());
        }

        RefreshPageList();
        _pageList.SelectedIndex = _selectedPageIndex;
        _lblStatus.Text = $"Duplicated page {count} times";
    }

    private int ShowMultiplyDialog()
    {
        using var form = new Form
        {
            Text = "Multiply Page",
            Size = new Size(300, 150),
            StartPosition = FormStartPosition.CenterParent,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false
        };

        var lbl = new Label { Text = "How many times should this page be duplicated?", AutoSize = true, Location = new Point(10, 15) };
        var nud = new NumericUpDown { Minimum = 1, Maximum = 100, Value = 1, Location = new Point(10, 40), Width = 100 };
        var btnOk = new Button { Text = "OK", DialogResult = DialogResult.OK, Location = new Point(100, 75) };
        var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(185, 75) };

        form.Controls.AddRange(new Control[] { lbl, nud, btnOk, btnCancel });
        form.AcceptButton = btnOk;
        form.CancelButton = btnCancel;

        if (form.ShowDialog() == DialogResult.OK)
            return (int)nud.Value;
        return 0;
    }

    private void BtnSelectCoverImage_Click(object? sender, EventArgs e)
    {
        if (_selectedPageIndex < 0) return;
        using var dlg = new OpenFileDialog { Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp", Title = "Select Cover Image" };
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            try
            {
                byte[] imageBytes = File.ReadAllBytes(dlg.FileName);
                string base64 = Convert.ToBase64String(imageBytes);

                var page = _project.Pages[_selectedPageIndex];
                page.Elements.RemoveAll(x => x is CustomImageElement);
                page.Elements.Add(new CustomImageElement { Base64Image = base64 });

                OnSettingsChanged();
            }
            catch (Exception ex) { MessageBox.Show($"Failed to load image: {ex.Message}"); }
        }
    }

    private void BtnAddCustomText_Click(object? sender, EventArgs e)
    {
        if (_selectedPageIndex < 0) return;
        var page = _project.Pages[_selectedPageIndex];
        page.Elements.Add(new CustomTextElement { Text = "New Text Block" });
        RefreshCustomTextUI(page);
        OnSettingsChanged();
    }

    private void RefreshCustomTextUI(PageSettings page)
    {
        _customTextFlow.SuspendLayout();

        // Clear all existing dynamically generated blocks (skip the first header block)
        while (_customTextFlow.Controls.Count > 1)
        {
            var ctrl = _customTextFlow.Controls[1];
            _customTextFlow.Controls.RemoveAt(1);
            ctrl.Dispose();
        }

        var texts = page.Elements.OfType<CustomTextElement>().ToList();
        if (texts.Count > 0)
        {
            for (int i = 0; i < texts.Count; i++)
            {
                var txt = texts[i];

                var panel = new Panel { AutoSize = true, BorderStyle = BorderStyle.FixedSingle, Padding = new Padding(5), Margin = new Padding(0, 5, 0, 5) };
                var flow = new FlowLayoutPanel { AutoSize = true, WrapContents = false, FlowDirection = FlowDirection.TopDown };

                // Text Input
                var txtInput = new TextBox { Width = 250, Text = txt.Text, Multiline = true, Height = 40 };
                txtInput.TextChanged += (s, e) => { txt.Text = txtInput.Text; OnSettingsChanged(); };
                flow.Controls.Add(txtInput);

                // Controls (Font Size, Top/Bottom Margin)
                var ctrlFlow = new FlowLayoutPanel { AutoSize = true, WrapContents = false };

                var nudSize = new NumericUpDown { Width = 50, Minimum = 6, Maximum = 100, Value = (decimal)txt.FontSizePt };
                nudSize.ValueChanged += (s, e) => { txt.FontSizePt = (float)nudSize.Value; OnSettingsChanged(); };

                var nudTop = new NumericUpDown { Width = 50, Minimum = 0, Maximum = 100, Value = (decimal)txt.Y_Mm };
                nudTop.ValueChanged += (s, e) => { txt.Y_Mm = (float)nudTop.Value; OnSettingsChanged(); };

                var btnDel = new Button { Text = "X", Width = 30, ForeColor = Color.Red };
                btnDel.Click += (s, e) => {
                    page.Elements.Remove(txt);
                    RefreshCustomTextUI(page);
                    OnSettingsChanged();
                };

                ctrlFlow.Controls.Add(new Label { Text = "Size:", AutoSize = true, Padding = new Padding(0, 5, 0, 0) });
                ctrlFlow.Controls.Add(nudSize);
                ctrlFlow.Controls.Add(new Label { Text = "Gap:", AutoSize = true, Padding = new Padding(10, 5, 0, 0) });
                ctrlFlow.Controls.Add(nudTop);
                ctrlFlow.Controls.Add(btnDel);

                flow.Controls.Add(ctrlFlow);
                panel.Controls.Add(flow);
                _customTextFlow.Controls.Add(panel);
            }
        }

        _customTextFlow.ResumeLayout(true);
        _customTextFlow.PerformLayout();
        _coverPanel.PerformLayout();
    }

    private void BtnNew_Click(object? sender, EventArgs e)
    {
        if (MessageBox.Show("Create a new project? Unsaved changes will be lost.", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            _project = Project.CreateDefault();
            RefreshPageList();
            if (_pageList.Items.Count > 0)
                _pageList.SelectedIndex = 0;
            _lblStatus.Text = "New project created";
        }
    }
}

internal class CustomColorTable : ProfessionalColorTable
{
    public override Color ToolStripGradientBegin => Color.FromArgb(250, 250, 255);
    public override Color ToolStripGradientMiddle => Color.FromArgb(245, 245, 250);
    public override Color ToolStripGradientEnd => Color.FromArgb(240, 240, 245);
}