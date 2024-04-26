using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Manina.Windows.Forms;



namespace AI绘图法典tag纠错
{
    public delegate void 刷新缩略图(string path);
    public delegate void 用Item刷新缩略图(ImageListViewItem path);


    public class 控制ImageListView
    {
       

        public static string 历史记录 = "";
        public static 用Item刷新缩略图 刷新选中tag缩略图;
        public static 刷新缩略图 切换页面;
        public static 刷新缩略图 刷新选中ListBox缩略图;  
        public static System.Drawing.Size 图标最小大小 = new System.Drawing.Size(120, 1);
        public static System.Drawing.Size 图标最大大小 = new System.Drawing.Size(120, 70);
        public static int ListBoxHeight = 70;
        public static int ImageListViewHeight = 70;
        public static int ImageListViewHeightbig = 70;
        public static int GroupBoxHeight = 100;
        public static int addHeight = 86;
        public static int addAllHeight = 0;
        // public static int bbaddHeight = 30;
        public static ImageListView 拖拽离开的ImageListView = null; 
        public static List<ImageListViewItem> 冻结选中项目;//冻结选中项目

        public static bool 三正一负 = true;



        bool 正在变更选中 = false;
         

        public ImageListView tag栏位列表ImageListView; 
         

         

        public 控制ImageListView(ImageListView bb)
        {
            tag栏位列表ImageListView = bb;

            tag栏位列表ImageListView.AllowDrag = true;
            tag栏位列表ImageListView.AllowDrop = true;
            tag栏位列表ImageListView.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));

            tag栏位列表ImageListView.Name = "tags" ;
            tag栏位列表ImageListView.PersistentCacheDirectory = "";
            tag栏位列表ImageListView.PersistentCacheSize = ((long)(100));
            //tag栏位列表ImageListView.Size = new System.Drawing.Size(844, ImageListViewHeight + addHeight + addAllHeight);
            tag栏位列表ImageListView.TabIndex = 2;
            tag栏位列表ImageListView.ThumbnailSize = 图标最小大小;
            tag栏位列表ImageListView.UseWIC = true;
            tag栏位列表ImageListView.MouseEnter += ImageListView_MouseEnter;
            tag栏位列表ImageListView.MouseLeave += ImageListView_MouseLeave;
            tag栏位列表ImageListView.DragLeave += ImageListView_DragLeave;
            tag栏位列表ImageListView.DropComplete += ImageListView_DropComplete;
            tag栏位列表ImageListView.SelectionChanged += ImageListView_选中变更时;
            tag栏位列表ImageListView.AutoRotateThumbnails = false;
            tag栏位列表ImageListView.ItemClick += Tag栏位列表ImageListView_ItemClick;
            tag栏位列表ImageListView.ItemDoubleClick += Tag栏位列表ImageListView_ItemDoubleClick;
            tag栏位列表ImageListView.ItemHover += Tag栏位列表ImageListView_ItemHover;
            tag栏位列表ImageListView.MouseDown += Tag栏位列表ImageListView_MouseDown;

             


        }


        private void Tag栏位列表ImageListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                // 鼠标中键按下的代码
                切换页面("");
            }
        }

        private void Tag栏位列表ImageListView_ItemHover(object sender, ItemHoverEventArgs e)
        {
            if (e!=null&& e.Item != null)
                刷新选中tag缩略图(e.Item);
            else
                刷新选中ListBox缩略图("");
        }

        private void Tag栏位列表ImageListView_ItemDoubleClick(object sender, ItemClickEventArgs e)
        {
            if (e.Item.Text.Contains("::"))
                return;
            if (e.Buttons == MouseButtons.Left)
            {
                //鼠标左键双击
                //增加权重 
                e.Item.User包含全部格式的英文Tag = 单个Tag.逐一设置标签权重(e.Item.User包含全部格式的英文Tag, 0.1f, 0, 0, 0);
                //单个词条 aa = new 单个词条(e.Item.User单一词条的string);
               // e.Item.Text = e.Item.User包含全部格式的英文Tag.Replace(aa.英文tag, aa.中文翻译);
                e.Item.Text = 单个Tag.翻转冒号前后(e.Item.Text);
            }
            else if (e.Buttons == MouseButtons.Right)
            {
                // 鼠标右键双击
                //减少权重 
                e.Item.User包含全部格式的英文Tag = 单个Tag.逐一设置标签权重(e.Item.User包含全部格式的英文Tag, -0.1f, 0, 0, 0);
                //单个词条 aa = new 单个词条(e.Item.User单一词条的string);
              // e.Item.Text = e.Item.User包含全部格式的英文Tag.Replace(aa.英文tag, aa.中文翻译);
                e.Item.Text = 单个Tag.翻转冒号前后(e.Item.Text);

            }
            刷新选中tag缩略图(e.Item);
        }

        private void Tag栏位列表ImageListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.Buttons == MouseButtons.Left)
            {
                // 鼠标左键单击

            }
            else if (e.Buttons == MouseButtons.Right)
            {
                // 鼠标右键单击

            }


        }
        public static ImageListViewItem newSelectedItem = null;
        public List<string> 自动填充的节点列表ListBox = new List<string>();

        private void ImageListView_选中变更时(object sender, EventArgs e)
        {

            ImageListView listbox = sender as ImageListView;

            if (!正在变更选中 && 冻结选中项目 != null && 冻结选中项目.Count > 0)
            {
                正在变更选中 = true;
                listbox.ClearSelection();
                foreach (ImageListViewItem a in 冻结选中项目)
                {
                    a.Selected = true;
                }
                正在变更选中 = false;
            }
        }



        private void ImageListView_DropComplete(object sender, DropCompleteEventArgs e)
        {
            ImageListView listbox = sender as ImageListView;

            历史记录 +=
                "【拖拽到：】" + listbox.Name +
                "【数量：】" + e.Items.Count() + "\r\n";
            int indexx = e.Index;
            bool insetxx = false;
            if (listbox.Items.Count != e.Items.Count())
                insetxx = true;
            bool 找到词 = false;
            if (拖拽离开的ImageListView != null && 拖拽离开的ImageListView != listbox)
            {
                历史记录 +=
                "【完成从：】" + 拖拽离开的ImageListView.Name +
                "【拖拽到：】" + listbox.Name +
                "【数量：】" + e.Items.Count() + "\r\n";
                foreach (ImageListViewItem a in e.Items)
                {
                    历史记录 += "【foreach】" + a.FileName + "\r\n";
                    for (int i = 0; i < 拖拽离开的ImageListView.Items.Count; i++)
                    {
                        历史记录 += "【i】" + i.ToString() + "\r\n";
                        历史记录 += "【确认 】" + a.FileName + "【 a和拖拽离开 】" + 拖拽离开的ImageListView.Items[i].FileName + "\r\n";
                        if (a.FileName == 拖拽离开的ImageListView.Items[i].FileName)
                        {
                            找到词 = true;
                            历史记录 += "【确认成功】" + "\r\n";
                            if (insetxx)
                            {
                                //此部分为修正拖拽无法传递txt的问题。

                                for (int ii = 0; ii < listbox.Items.Count; ii++)
                                {
                                    if (listbox.Items[ii].FileName == a.FileName)
                                    {
                                        历史记录 += "【删除错误】" + listbox.Items[ii].FileName + "\r\n";
                                        listbox.Items.RemoveAt(ii);

                                    }
                                }
                                listbox.Items.Insert(indexx, new ImageListViewItem(
                                    拖拽离开的ImageListView.Items[i].FileName,
                                    拖拽离开的ImageListView.Items[i].Text,
                                    拖拽离开的ImageListView.Items[i].User包含全部格式的英文Tag,
                                    拖拽离开的ImageListView.Items[i].User单一词条的string
                                    ));
                                历史记录 += "【新增插入：】" + 拖拽离开的ImageListView.Items[i].FileName + 拖拽离开的ImageListView.Items[i].Text + "\r\n";
                                //indexx++; 
                            }
                            else
                            {
                                for (int ii = 0; ii < listbox.Items.Count; ii++)
                                {
                                    if (listbox.Items[ii].FileName == a.FileName)
                                    {
                                        历史记录 += "【删除错误】" + listbox.Items[ii].FileName + "\r\n";
                                        listbox.Items.RemoveAt(ii);
                                    }
                                }

                                listbox.Items.Add(new ImageListViewItem(
                                        拖拽离开的ImageListView.Items[i].FileName,
                                        拖拽离开的ImageListView.Items[i].Text,
                                        拖拽离开的ImageListView.Items[i].User包含全部格式的英文Tag,
                                        拖拽离开的ImageListView.Items[i].User单一词条的string
                                        ));
                                历史记录 += "【新增添加：】" + 拖拽离开的ImageListView.Items[i].FileName + 拖拽离开的ImageListView.Items[i].Text + "\r\n";

                            }
                            历史记录 += "【移除单项】" + 拖拽离开的ImageListView.Items[i].FileName + "\r\n";
                            拖拽离开的ImageListView.Items.RemoveAt(i);
                            历史记录 += "【还剩余:】" + 拖拽离开的ImageListView.Items.Count.ToString() + "\r\n";

                        }
                    }
                    if (!找到词)
                    {
                        历史记录 += "【！】检测到多余【！】";
                        List<ImageListViewItem> del = new List<ImageListViewItem>();
                        //确认是否存在重复项目
                        /*
                        foreach (Tag栏位 a1 in Tag栏位.我的栏位)
                        {
                            foreach (ImageListViewItem aa in a1.tag栏位列表ImageListView.Items)
                            {
                                foreach (ImageListViewItem bb in listbox.Items)
                                {
                                    if (aa.FileName == bb.FileName && aa.Text != bb.Text)
                                    {
                                        del.Add(bb);
                                    }
                                }
                            }
                        }
                        */
                        for (int i = 0; i < del.Count; i++)
                        {
                            历史记录 += "【！】删除【！】" + del[i].FileName;
                            listbox.Items.Remove(del[i]);
                        }

                    }
                }
                历史记录 += "【清空:】" + 拖拽离开的ImageListView.Items.Count.ToString() + "\r\n";
                历史记录 += "##【结束】" + 拖拽离开的ImageListView.Name + "\r\n";
                历史记录 += "\r\n";
                拖拽离开的ImageListView = null;
            }
            else
            {
                历史记录 += "【！】检测到多余【！】";
                List<ImageListViewItem> del = new List<ImageListViewItem>();
                //确认是否存在重复项目
                /*
                foreach (Tag栏位 a1 in Tag栏位.我的栏位)
                {
                    foreach (ImageListViewItem aa in a1.tag栏位列表ImageListView.Items)
                    {
                        foreach (ImageListViewItem bb in listbox.Items)
                        {
                            if (aa.FileName == bb.FileName && aa.Text != bb.Text)
                            {
                                del.Add(bb);
                            }
                        }
                    }
                }
                */
                for (int i = 0; i < del.Count; i++)
                {
                    历史记录 += "【！】删除【！】" + del[i].FileName;
                    listbox.Items.Remove(del[i]);
                }
                拖拽离开的ImageListView = null;

            }
        }


        private void ImageListView_DragLeave(object sender, EventArgs e)
        {

            if (拖拽离开的ImageListView == null)
            {
                拖拽离开的ImageListView = sender as ImageListView;
                历史记录 += "##【创建：】" + 拖拽离开的ImageListView.Name + "\r\n";
                历史记录 += "【数量:】" + 拖拽离开的ImageListView.Items.Count.ToString() + "\r\n";
            }
        }

        private void ImageListView_MouseLeave(object sender, EventArgs e)
        {

            ImageListView listbox = sender as ImageListView;
            //listbox.ClearSelection();
            //指定最小化(栏位.我的栏位, listbox);
        }

        private void ImageListView_MouseEnter(object sender, EventArgs e)
        { 
            ImageListView listbox = sender as ImageListView;
            // 指定最大化(栏位.我的栏位, listbox);
        }

    }
}
