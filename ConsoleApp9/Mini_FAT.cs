using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    internal class Mini_FAT
    {
        static int[] FAT = new int[1024];
        public static void PrepareFAT()
        {

            for (int i = 0; i < 1024; i++)
            {
                if (i == 0 || i == 4)
                {
                    FAT[i] = -1;
                }
                else if (i == 1 || i == 2 || i == 3)
                {
                    FAT[i] = i++;
                }
                else
                {
                    FAT[i] = 0;
                }
            }
        }
        public static int avaibleCluster()//first free cluster in fat
        {
            for (int i = 5; i < 1024; i++)
            {
                if (FAT[i] == 0)
                    return i;
            }
            return -1;
        }
        public static void writeFat()
        {
            byte[] bytes = new byte[4 * 1024]; //
            Buffer.BlockCopy(FAT, 0, bytes, 0, bytes.Length);//convert content of int array to byte

            for (int i = 0; i < 4; i++)
            {
                byte[] cluster = new byte[1024];
                CopyCntent(bytes, cluster, i * 1024, (i + 1) * 1024);
                VirtualDisk.writeCluster(i + 1, cluster);
            }
        }
        public static void readFat()
        {
            byte[] B = new byte[4096];
            for (int i = 0; i < 4; i++)
            {
                byte[] b = VirtualDisk.readCluster(i + 1);
                sotre(B, (i * 1024), b);
            }
            Buffer.BlockCopy(B, 0, FAT, 0, B.Length);//convert from byte to int
            for (int i = 0; i < FAT.Length; i++)
                Console.Write(FAT[i]);
            Console.WriteLine();
        }
        public static void sotre(byte[] B, int offset, byte[] b)
        {
            int c = 0;
            for (int i = offset; i < offset + 1024; i++)
            {
                B[i] = b[c];
                c++;
            }
        }
        private static void CopyCntent(byte[] src, byte[] dest, int start, int End)
        {
            int c = 0;
            for (int i = start; i < End; i++)
            {
                dest[c] = src[i];
                c++;
            }
        }
        public static void SetClusterPointer(int clusterindex, int pointer)
        {
            FAT[clusterindex] = pointer;
        }
        public static int GetClusterPointer(int clusterindex)
        {
            return clusterindex;
        }
        public static int number_of_Empty_Cluster()
        {
            int counter = 0;
            for (int c = 0; c < FAT.Length; c++)
            {
                if (FAT[c] == 0)
                    counter++;
            }
            return counter;
        }
        public static int GetFreeSpace()
        {
            return avaibleCluster() * 1024;
        }

    }



}

