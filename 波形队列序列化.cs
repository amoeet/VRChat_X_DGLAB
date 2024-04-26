using AI绘图法典tag纠错;
using Manina.Windows.Forms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 郊狼蓝牙测试
{
    [Serializable]
    public class 波形队列序列化
    {


        public List<string> 波形路径FileName;
        public List<string> 波形名称Text;
        public List<string> 波形队列User包含全部格式的英文Tag;
         

        public 波形队列序列化(控制ImageListView a)
        {
            波形路径FileName = new List<string>();
            波形名称Text=new List<string>();    
            波形队列User包含全部格式的英文Tag = new List<string>();
            foreach ( ImageListViewItem aa in a.tag栏位列表ImageListView.Items  )
            {
                波形路径FileName.Add (aa.FileName);
                波形名称Text.Add(aa.Text );
                波形队列User包含全部格式的英文Tag.Add(aa.User包含全部格式的英文Tag);
            } 
        }

        public void 重设控制ImageListView(ref 控制ImageListView a)
        {
            a.tag栏位列表ImageListView.Items.Clear(); 
            for ( int i = 0;i< 波形队列User包含全部格式的英文Tag.Count;i++)
            { 
                Random ss = new Random();
                string bb = ss.Next(0, 9999).ToString();
                ImageListViewItem at = new ImageListViewItem(波形路径FileName[i], 波形名称Text[i], 波形队列User包含全部格式的英文Tag[i], "不使用");
                a.tag栏位列表ImageListView.Items.Add(at); 
            }  
        }
        public static T Clone<T>(T item) where T : class
        {
            T result = default(T);
            if (null != item)

            {

                MemoryStream ms = new MemoryStream();

                BinaryFormatter bf = new BinaryFormatter();

                bf.Serialize(ms, item);

                ms.Seek(0, SeekOrigin.Begin);

                result = bf.Deserialize(ms) as T;//网上抄的代码总是把这句写在最后，奇葩的大家都是一顿copy

            }

            return result;

        }
    }



    [Serializable]
    public class 郊狼存档
    {


        public 郊狼存档()
        {
            DG波形列表 = new Dictionary<string, string>();
            DG波形队列 = new Dictionary<string, 波形队列序列化>();

        }


        public Dictionary<string, string> DG波形列表  ;

        public Dictionary<string, 波形队列序列化> DG波形队列  ;

    }



}
