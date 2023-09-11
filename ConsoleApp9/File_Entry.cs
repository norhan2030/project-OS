using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    internal class File_Entry: Directory_Entry
    {
        public string content_file;
        Directory parent;
        public File_Entry(string dir_name, byte dir_attr, int dir_firstCluster, Directory parentt) : base(dir_name, dir_attr, dir_firstCluster)
        {
            content_file = string.Empty;
            if (parentt != null)
                parent = parentt;
        }
        public File_Entry(string dir_name, byte dir_attr, int dir_firstCluster, int dir_filesize, Directory parentt, string content) : base(dir_name, dir_attr, dir_firstCluster)
        {
            content_file = string.Empty;
            if (parentt != null)
                parent = parentt;
        }
        public void read()
        {
            if (this.dir_firstCluster != 0)
            {
                content_file = string.Empty;
                int cluster = this.dir_firstCluster;
                int next = Mini_FAT.GetClusterPointer(cluster);
                List<byte> ls = new List<byte>();
                do
                {
                    ls.AddRange(VirtualDisk.readCluster(cluster));
                    cluster = next;
                    if (cluster != -1)
                        next = Mini_FAT.GetClusterPointer(cluster);
                }
                while (next != -1);
                content_file = byte_To_string(ls.ToArray());
            }

        }
        public static string byte_To_string(byte[] bytes)
        {
            string str = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                str += (char)bytes[i];
            }
            return str;
        }
        public static List<byte[]> split(byte[] bytes)
        {
            List<byte[]> ls = new List<byte[]>();
            int num_arrays = bytes.Length / 1024;
            for (int i = 0; i < num_arrays; i++)
            {
                byte[] b = new byte[1024];
                for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                {
                    b[k] = bytes[j];
                }
                ls.Add(b);
            }
            return ls;
        }
        public void write()
        {
            byte[] content_file_byte = string_to_bytes(content_file);
            List<byte[]> byte_list = split(content_file_byte);
            int clusterIndex;
            if (this.dir_firstCluster != 0)
            {
                clusterIndex = this.dir_firstCluster;
            }
            else
            {
                clusterIndex = Mini_FAT.avaibleCluster();
                this.dir_firstCluster = clusterIndex;
            }
            int lastCluster = -1;
            for (int i = 0; i < byte_list.Count; i++)
            {
                if (clusterIndex != -1)
                {
                    VirtualDisk.writeCluster(clusterIndex, byte_list[i]);
                    Mini_FAT.SetClusterPointer(clusterIndex, -1);
                    if (lastCluster != -1)
                        Mini_FAT.SetClusterPointer(lastCluster, clusterIndex);
                    lastCluster = clusterIndex;
                    clusterIndex = Mini_FAT.avaibleCluster();
                }
            }
        }
        public static byte[] string_to_bytes(string s)
        {
            byte[] bytes = new byte[s.Length];
            for (int i = 0; i < s.Length; i++)
            {
                bytes[i] = (byte)s[i];
            }
            return bytes;
        }
        
        public void delete()
        {


            clearmyCluster();

            if (this.parent != null)
            {
                this.parent.delete_entry(GetDirectory_Entry());
            }
            //if(Program.CurrentDirectory==this)
            //{
            //    if(this.parent !=null)
            //    {
            //        Program.CurrentDirectory = this.parent;
            //        Program.CurrentPath = Program.CurrentDirectory.readDirectory();
            //    }
            //}
            Mini_FAT.writeFat();

        }
        public Directory_Entry GetDirectory_Entry()
        {
            Directory_Entry me = new Directory_Entry(this.dir_name, this.dir_attr, this.dir_firstCluster);//////this.dir_name
            return me;

        }
        public int getsizeonDisk()
        {
            int size = 0;
            if (this.dir_firstCluster != 0)
            {
                int cluster = this.dir_firstCluster;
                int next = Mini_FAT.GetClusterPointer(cluster);
                do
                {
                    size++;
                    cluster = next;
                    if (cluster != -1)
                        next = Mini_FAT.GetClusterPointer(cluster);
                }
                while (cluster != -1);
            }
            return size;
        }
        void clearmyCluster()//empty me cluster
        {
            if (this.dir_filesize != 0)
            {
                int cluster = this.dir_filesize;
                int next = Mini_FAT.GetClusterPointer(cluster);
                if (cluster == 5 && next == 0)//root
                    return;
                do
                {
                    Mini_FAT.SetClusterPointer(cluster, 0);
                    cluster = next;
                    if (cluster != -1)
                        next = Mini_FAT.GetClusterPointer(cluster);
                }
                while (cluster != -1);
            }
        }
    }
}
