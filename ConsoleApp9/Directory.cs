using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    internal class Directory: Directory_Entry
    {
        public Directory parent;
        //كل فولدر عبارة عن directory-entry
        public List<Directory_Entry> Directoryfiles = new List<Directory_Entry>();//list with all directory
        //int f = Mini_FAT.avaibleCluster;
        public Directory(string dir_name, byte dir_attr, byte[] dir_empty, int dir_firstCluster, int dir_filesize, Directory parentt) : base(dir_name, dir_attr, dir_empty, dir_firstCluster, dir_filesize)
        {

            if (parentt != null)
                parent = parentt;

        }
        public Directory(string dir_name, byte dir_attr, int dir_firstCluster) : base(dir_name, dir_attr, dir_firstCluster)
        {
        }
        public Directory()
        {
            Console.WriteLine("new dir is created");
        }
        public void writeDirectory()
        {
            Directory_Entry old_dir = GetDirectory_Entry();
            //convert all folder to array of byte each index is directory-entry with size 32
            byte[] diesorfilesBytes = new byte[Directoryfiles.Count * 32];
            //convert each index to array of byte
            for (int i = 0; i < Directoryfiles.Count; i++)
            {
                //Converter.Directory_EntryToBytes
                byte[] b = Directory_Entry_ToBytes(this.Directoryfiles[i]);

                for (int j = i * 32, k = 0; k < b.Length; k++, j++)
                    diesorfilesBytes[j] = b[k];
            }
            //each array have size of 1024
            List<byte[]> bytesclu = split(diesorfilesBytes);
            //هجيب فيه رقم الاندكس الفاضي اللي هكتب فيه
            int clusterindex;
            if (this.dir_firstCluster != 0)
            {
                clusterindex = this.dir_firstCluster;
            }
            else
            {
                clusterindex = Mini_FAT.avaibleCluster();
                this.dir_firstCluster = clusterindex;
            }
            int lastcluster = -1;
            for (int i = 0; i < bytesclu.Count; i++)
            {
                if (clusterindex != -1)
                {
                    VirtualDisk.writeCluster(clusterindex, bytesclu[i]);
                    Mini_FAT.SetClusterPointer(clusterindex, -1);
                    if (lastcluster != -1)
                        Mini_FAT.SetClusterPointer(lastcluster, clusterindex);
                    lastcluster = clusterindex;
                    clusterindex = Mini_FAT.avaibleCluster();
                }
            }
            
            Directory_Entry new_dir = GetDirectory_Entry();
            if (this.parent != null)
            {
                this.parent.updatecontent(old_dir, new_dir);
                this.parent.writeDirectory();
                Mini_FAT.writeFat();
            }
        }
        public static List<byte[]> split(byte[] bytes)
        {
            List<byte[]> lc = new List<byte[]>();
            int num_arrays = bytes.Length / 1024;
            for (int i = 0; i < num_arrays; i++)
            {
                byte[] b = new byte[1024];
                for (int j = i * 1024, k = 0; k < 1024; j++, k++)
                {
                    b[k] = bytes[j];
                }
                lc.Add(b);
            }
            return lc;
        }
        public static byte[] Directory_Entry_ToBytes(Directory_Entry directory)
        {
            byte[] bytes = new byte[32];
            for (int i = 0; i < directory.dir_name.Length; i++)
            {
                bytes[i] = (byte)directory.dir_name[i];
            }
            bytes[11] = directory.dir_attr;

            for (int i = 0, j = 12; i < directory.dir_empty.Length; j++, i++)
            {
                bytes[j] = directory.dir_empty[i];

            }
            byte[] dir_F_C = BitConverter.GetBytes(directory.dir_firstCluster);
            for (int i = 0, j = 24; i < dir_F_C.Length; j++, i++)
            {
                bytes[j] = dir_F_C[i];

            }
            byte[] dir_F_L = BitConverter.GetBytes(directory.dir_filesize);
            for (int i = 0, j = 28; i < dir_F_L.Length; j++, i++)
            {
                bytes[j] = dir_F_L[i];

            }
            return bytes;
        }
        public Directory_Entry GetDirectory_Entry()
        {
            Directory_Entry me = new Directory_Entry(this.dir_name, this.dir_attr, this.dir_firstCluster);//////this.dir_name
            return me;

        }
        public void Create_Directory_Entry()
        {
            Directory me = new Directory(this.dir_name, this.dir_attr, this.dir_firstCluster);//////this.dir_name
        }
        public void readDirectory()
        {
            Directoryfiles = new List<Directory_Entry>();
            if (this.dir_firstCluster != 0)
            {
                //byte[] Rciuster = new byte[1000];////////directory that  will read
                int clusterindex = dir_firstCluster;
                int next = Mini_FAT.GetClusterPointer(clusterindex);
                List<byte> Ls = new List<byte>();
                do
                {
                    Ls.AddRange(VirtualDisk.readCluster(clusterindex));
                    if (clusterindex != -1)
                        next = Mini_FAT.GetClusterPointer(clusterindex + 1);
                    clusterindex = next;
                }
                while (clusterindex != -1);
                for (int i = 0; i < Ls.Count; i++)
                {
                    byte[] b = new byte[32];
                    for (int j = i * 32, c = 0; c < b.Length && j < Ls.Count; c++, j++)
                    {
                        b[c] = Ls[j];
                    }
                    Directoryfiles.Add(byte_to_Directory_Entry(b));
                }

            }
        }

        public static Directory_Entry byte_to_Directory_Entry(Byte[] bytes)
        {
            char[] name = new char[11];
            for (int i = 0; i < name.Length; i++)
            {
                name[i] = (char)bytes[i];
            }
            byte attr = bytes[11];
            byte[] empty = new byte[12];

            for (int i = 0, j = 12; i < empty.Length; j++, i++)
            {
                empty[i] = bytes[j];

            }
            byte[] first_c = new byte[4];
            for (int i = 0, j = 24; i < first_c.Length; j++, i++)
            {
                first_c[i] = bytes[j];

            }
            int firstcluster = BitConverter.ToInt32(first_c, 0);
            byte[] file_s = new byte[4];
            for (int i = 0, j = 28; i < file_s.Length; j++, i++)
            {
                file_s[i] = bytes[j];
            }
            int filesize = BitConverter.ToInt32(file_s, 0);
            Directory_Entry d = new Directory_Entry(new string(name), attr, firstcluster);
            d.dir_empty = empty;
            d.dir_filesize = filesize;
            return d;
        }
        void updatecontent(Directory_Entry old_dir, Directory_Entry new_dir)
        {

            int index = searchDirectory(old_dir.dir_name);
            if (index != -1)
            {
                delete_entry(old_dir);
                add_entry(new_dir);
            }
        }
        public void update_content(Directory_Entry d)
        {
            int index = searchDirectory(d.dir_name);
            if (index != -1)
            {
                delete_entry(d);
                add_entry(d);
            }
        }
        public int searchDirectory(string name)
        {

            for (int i = 0; i < Directoryfiles.Count; i++)
            {
                if (Directoryfiles[i].dir_name.Equals(name))
                    return i;
            }
            return -1;
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
        public bool Can_Add_entry(Directory_Entry d)
        {
            bool enable = false;
            int size = (Directoryfiles.Count - 1) * 32;//amount of byte
            int clusters = size * 1024;//amount of cluster
            int stay = size % 1024;//remain
            if (stay > 0)
                clusters++;
            clusters = clusters + d.dir_filesize / 1024;
            int stay1 = d.dir_filesize % 1024;//file
            if (stay1 > 0)
                clusters++;
            if (getsizeonDisk() + Mini_FAT.avaibleCluster() >= clusters)//getsizeDisk will be empty when we write list//بيتصفر
                enable = true;

            return enable;
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
        public void deleteDirectory()
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
            //        Program.currentpath = Program.CurrentDirectory.readDirectory();
            //    }
            //}
            Mini_FAT.writeFat();

        }
        public void delete_entry(Directory_Entry d)
        {
            readDirectory();
            int index = searchDirectory(d.dir_name);
            Directoryfiles.RemoveAt(index);
            writeDirectory();
        }
        public void add_entry(Directory_Entry d)
        {
            Directoryfiles.Add(d);
            writeDirectory();
        }
    }
}
