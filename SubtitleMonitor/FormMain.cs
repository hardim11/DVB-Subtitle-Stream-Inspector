using Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SubtitleMonitor
{
    public partial class FormMain : Form
    {

        const string TITLEBAR = "DVB Subtitle Explorer";
        string _currentFile = "";

        public FormMain()
        {
            InitializeComponent();

            this.Text = TITLEBAR;
        }

        private void DoIt(string Filepath)
        {
            // check that the file is there
            if (!(File.Exists(Filepath)))
            {
                MessageBox.Show("File not found \"" + Filepath + "\"");
                return;
            }

            this.splitContainerMaIN.Visible = false;
            this.progressBarLoading.Value = this.progressBarLoading.Minimum;
            this.progressBarLoading.Visible = true;

            _currentFile = Filepath;

            this.Text = TITLEBAR + " - Opening " + _currentFile;

            // run in background
            BackGroundWorkerInitInfo initInfo = new BackGroundWorkerInitInfo { SourceFile = Filepath, CreateHtmlFiles = this.checkBoxCreateHTML.Checked };
            this.backgroundWorkerMain.RunWorkerAsync(initInfo);
        }

        private void PopulateTreeView(TransportStreamParser TsParser)
        {
            this.textBoxDesc.Text = "";
            this.listViewDetails.Items.Clear();
            this.treeViewMain.Nodes.Clear();
            foreach (KeyValuePair<int, List<DvbSubPes>> aPid in TsParser.SubtitlesLookup)
            {
                TreeNode pidNode = this.treeViewMain.Nodes.Add("PID " + aPid.Key.ToString());
                // would be nice to add descriptors
                //TODO add descriptors


                //do the PES
                foreach (DvbSubPes aDvbPes in aPid.Value)
                {
                    TreeNode pesNode = pidNode.Nodes.Add("PTS " + aDvbPes.PresentationTimestampToString() + " (" +  aDvbPes.SubtitleSegments.Count.ToString() + " segments)");

                    // add the bitmap?
                    // this causes a huge memory footprint, need a better way
                    //pesNode.Tag = aDvbPes.GetImageFull();
                    pesNode.Tag = aDvbPes;

                    foreach (SubtitleSegment aSubtitleSegment in aDvbPes.SubtitleSegments)
                    {
                        TreeNode segmentNode = pesNode.Nodes.Add(aSubtitleSegment.SegmentTypeDescription);
                        segmentNode.Tag = aSubtitleSegment;

                        switch (aSubtitleSegment.SegmentType)
                        {
                            case SubtitleSegment.PageCompositionSegment:
                                segmentNode.Tag = aSubtitleSegment.PageComposition;
                                break;
                            case SubtitleSegment.RegionCompositionSegment:
                                segmentNode.Tag = aSubtitleSegment.RegionComposition;
                                break;
                            case SubtitleSegment.ClutDefinitionSegment:
                                segmentNode.Tag = aSubtitleSegment.ClutDefinition;
                                break;
                            case SubtitleSegment.ObjectDataSegment:
                                segmentNode.Tag = aSubtitleSegment.ObjectData;
                                break;
                            case SubtitleSegment.DisplayDefinitionSegment:
                                segmentNode.Tag = aSubtitleSegment.DisplayDefinition;
                                break;
                            case SubtitleSegment.EndOfDisplaySetSegment:
                                segmentNode.Tag = aSubtitleSegment.EndOfDisplaySet;
                                break;
                            default:
                                Console.WriteLine("Unknown Segment Type");
                                break;
                        }
                    }
                }
            }
        }

        private string GetHtmlPid(KeyValuePair<int, List<DvbSubPes>> SubtitlePid)
        {
            string res = "<table border=\"1\">\r\n";
            res += "<tr><th>PID: " + SubtitlePid.Key.ToString() + "</th><td>&nbsp;</td></tr>\r\n";
            res += "<tr><td>&nbsp;</td><td colsp=\"1\"><table border=\"1\">";
            foreach (var aDvbPes in SubtitlePid.Value)
            {
                res += "<tr><th>PTS: " + aDvbPes.PresentationTimestamp.ToString() + "</th><td><table border=\"1\" width=\"100%\">\r\n";

                foreach (SubtitleSegment aSubtitleSegment in aDvbPes.SubtitleSegments)
                {
                    res += "<tr><td>" + aSubtitleSegment.SegmentTypeDescription + "</td><td>Page ID:" + aSubtitleSegment.PageId.ToString() + "</td></tr>\r\n";
                    res += "<tr><td>&nbsp;</td><td>";
                    switch (aSubtitleSegment.SegmentType)
                    {
                        case SubtitleSegment.PageCompositionSegment:
                            res += aSubtitleSegment.PageComposition.ToHtml();
                            break;
                        case SubtitleSegment.RegionCompositionSegment:
                            res += aSubtitleSegment.RegionComposition.ToHtml();
                            break;
                        case SubtitleSegment.ClutDefinitionSegment:
                            res += aSubtitleSegment.ClutDefinition.ToHtml();
                            break;
                        case SubtitleSegment.ObjectDataSegment:
                            res += aSubtitleSegment.ObjectData.ToHtml();
                            break;
                        case SubtitleSegment.DisplayDefinitionSegment:
                            res += aSubtitleSegment.DisplayDefinition.ToHtml();
                            break;
                        case SubtitleSegment.EndOfDisplaySetSegment:
                            res += aSubtitleSegment.EndOfDisplaySet.ToHtml();
                            break;
                        default:
                            Console.WriteLine("Unknown Segment Type");
                            break;
                    }


                    res += "</td></tr>\r\n";
                }
                res += "</table></td>";
            }
            res += "</table></td></tr>\r\n";
            res += "</table>\r\n";
            return res;
        }

        private void UpdateProgress(long position, long total, string statusMessage)
        {
            var percent = (int)Math.Round(position * 100.0 / total);
            Console.WriteLine(statusMessage + " progess "  + percent.ToString());
            this.progressBarLoading.Value = percent;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
        }

        private void treeViewMain_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeViewItemChange();
        }

        private void TreeViewItemChange()
        { 
            this.listViewDetails.BeginUpdate();
            try
            {
                if (this.treeViewMain.SelectedNode == null) { return; }

                if (this.treeViewMain.SelectedNode.Tag != null)
                {
                    string tagType = this.treeViewMain.SelectedNode.Tag.GetType().ToString();
                    switch (tagType)
                    {
                        case "Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream.PageCompositionSegment":
                            SwitchDetailsDescriptionFrame(false);
                            ((PageCompositionSegment)this.treeViewMain.SelectedNode.Tag).PopulateListViewDetails(this.listViewDetails);
                            break;
                        case "Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream.RegionCompositionSegment":
                            SwitchDetailsDescriptionFrame(false);
                            ((RegionCompositionSegment)this.treeViewMain.SelectedNode.Tag).PopulateListViewDetails(this.listViewDetails);
                            break;
                        case "Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream.ClutDefinitionSegment":
                            SwitchDetailsDescriptionFrame(false);
                            ((ClutDefinitionSegment)this.treeViewMain.SelectedNode.Tag).PopulateListViewDetails(this.listViewDetails);
                            break;
                        case "Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream.ObjectDataSegment":
                            SwitchDetailsDescriptionFrame(false);
                            ((ObjectDataSegment)this.treeViewMain.SelectedNode.Tag).PopulateListViewDetails(this.listViewDetails);
                            break;
                        case "Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream.DisplayDefinitionSegment":
                            SwitchDetailsDescriptionFrame(false);
                            ((DisplayDefinitionSegment)this.treeViewMain.SelectedNode.Tag).PopulateListViewDetails(this.listViewDetails);
                            break;
                        case "Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream.EndOfDisplaySetSegment":
                            SwitchDetailsDescriptionFrame(false);
                            ((EndOfDisplaySetSegment)this.treeViewMain.SelectedNode.Tag).PopulateListViewDetails(this.listViewDetails);
                            break;
                        //case "System.Drawing.Bitmap":
                        //    SwitchDetailsDescriptionFrame(true);
                        //    this.listViewDetails.Items.Clear();
                        //    Bitmap tmp = (Bitmap)this.treeViewMain.SelectedNode.Tag;

                        //    this.pictureBoxSubs.Image = tmp;

                        //    break;
                        case "Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream.DvbSubPes":
                            SwitchDetailsDescriptionFrame(true);
                            this.listViewDetails.Items.Clear();
                            DvbSubPes tmp2 = (DvbSubPes)this.treeViewMain.SelectedNode.Tag;

                            Image old = this.pictureBoxSubs.Image;
                            this.pictureBoxSubs.Image = tmp2.GetImageFull(this.checkBoxShowObjectBorder.Checked);
                            if (old != null)
                            {
                                old.Dispose();
                            }
                            break;
                        default:
                            Console.WriteLine("Unknown Segment Type");
                            this.listViewDetails.Items.Clear();
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("tag null");
                }
            }
            finally
            {
                this.listViewDetails.EndUpdate();
            } 
        }

        private void SwitchDetailsDescriptionFrame(bool ShowImage = false)
        {
            this.pictureBoxSubs.Visible = ShowImage;
            this.textBoxDesc.Visible = !ShowImage;
        }

        private void listViewDetails_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (this.listViewDetails.SelectedItems.Count < 1)
            {
                this.textBoxDesc.Text = "";
            }
            else
            {
                this.textBoxDesc.Text = this.listViewDetails.SelectedItems[0].SubItems[2].Text;
            }
        }

        private void backgroundWorkerMain_DoWork(object sender, DoWorkEventArgs e)
        {
            BackGroundWorkerInitInfo initInfo = (BackGroundWorkerInitInfo)e.Argument;

            TransportStreamParser tsParser = new TransportStreamParser();
            tsParser.Parse(initInfo.SourceFile, (pos, total) => UpdateProgressBackground(pos, total, "Parsing Transport Stream file. Please wait..."));
            if (initInfo.CreateHtmlFiles)
            {
                foreach (KeyValuePair<int, List<DvbSubPes>> item in tsParser.SubtitlesLookup)
                {
                    string res = "<html><body>";
                    res += GetHtmlPid(item);
                    res += "</body></html>";
                    //TODO change to "next to video file"
                    string outFile = Path.GetDirectoryName(initInfo.SourceFile) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(initInfo.SourceFile) + "_(PID" + item.Key.ToString() + ").html";
                    File.WriteAllText(outFile, res);
                }
            }

            e.Result = tsParser;
        }

        private void UpdateProgressBackground(long position, long total, string statusMessage)
        {
            var percent = (int)Math.Round(position * 100.0 / total);
            this.backgroundWorkerMain.ReportProgress(percent);
        }

        private void backgroundWorkerMain_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.progressBarLoading.Value = e.ProgressPercentage;
        }

        private void backgroundWorkerMain_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            TransportStreamParser tsParser = (TransportStreamParser)e.Result;
            PopulateTreeView(tsParser);
            this.splitContainerMaIN.Visible = true;
            this.progressBarLoading.Value = this.progressBarLoading.Minimum;
            this.panelAbout.Visible = false;

            this.Text = TITLEBAR + " - " + _currentFile;
        }

        private void buttonOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = "ts";
            dlg.Filter = "Transport Stream Files|*.ts|All Files|*.*";
            dlg.Multiselect = false;
            if (dlg.ShowDialog() != DialogResult.OK) { return; }

            DoIt(dlg.FileName);

        }

        private void LaunchLink(LinkLabel Link)
        {
            // Navigate to a URL.
            System.Diagnostics.Process.Start(Link.Text);
        }

        private void linkLabelSubtitleEdit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LaunchLink((LinkLabel)sender);
        }

        private void linkLabelDVBSpec_Click(object sender, EventArgs e)
        {
            LaunchLink((LinkLabel)sender);
        }

        private void linkLabelDvbAnalyser_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LaunchLink((LinkLabel)sender);
        }

        private void linkLabelStreaGuru_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LaunchLink((LinkLabel)sender);
        }

        private void checkBoxShowObjectBorder_CheckedChanged(object sender, EventArgs e)
        {
            // repaint if already showing an image
            if (this.treeViewMain.SelectedNode == null) { return; }

            if (this.treeViewMain.SelectedNode.Tag != null)
            {
                if (this.treeViewMain.SelectedNode.Tag.GetType().ToString() == "Nikse.SubtitleEdit.Core.ContainerFormats.TransportStream.DvbSubPes")
                {
                    TreeViewItemChange();
                }
            }
        }
    }
}
