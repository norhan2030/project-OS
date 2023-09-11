using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    internal class VirtualDisk
    {
        Directory root = new Directory("k",0x10,5);
        //Mini_FAT.SetClusterPointer(5,-1);
        static FileStream Files;
        public static void initialize_Disk(string path)
        {
            if (!File.Exists(path))
            {
                Files = File.Create(path);
                byte[] fcluster = new byte[1024];

                for (int i = 0; i < 1024; i++)
                {
                    fcluster[i] = 0;
                }
                Mini_FAT.PrepareFAT();
                writeCluster(0, fcluster);
                Mini_FAT.writeFat();
            }
            if (File.Exists(path))
            {
                Files = File.Open(path, FileMode.Open);
                Mini_FAT.readFat();
            }
        }
        public static void writeCluster(int clusterindex, byte[] fcluster)//write 1024 byte in one itration
        {
            Files.Seek(clusterindex * 1024, SeekOrigin.Begin);
            for (int i = 0; i < clusterindex; i++)
            {
                Files.WriteByte(fcluster[i]);
            }
            Files.Flush();//مش هتستنى خطوات الميمورى لا تروح تكتب في الفايل علي طول

        }
        public static byte[] readCluster(int clusterindex)
        {
            byte[] readclu = new byte[1024];
            Files.Seek(clusterindex * 1024, SeekOrigin.Begin);
            Files.Read(readclu, 0, readclu.Length);
            return readclu;
        }
    }
}
