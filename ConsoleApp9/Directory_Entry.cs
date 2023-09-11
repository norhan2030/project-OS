using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    internal class Directory_Entry
    {
        public string dir_name;
        public byte dir_attr;
        public byte[] dir_empty = new byte[12];
        public int dir_firstCluster;
        public int dir_filesize;
        public Directory_Entry()
        {

        }
        public Directory_Entry(string directory_name, byte directory_attr, byte[] directory_empty, int directory_firstCluster, int directory_filesize)
        {
            // dir_name=directory_name;
            dir_attr = directory_attr;
            if (directory_attr == 0x0)
            {
                if (directory_name.Length <= 7)
                {
                    for (int i = 0; i < directory_name.Length; i++)
                    {

                        this.dir_name = directory_name;
                    }
                }
                else
                    Console.WriteLine("please enter name that has length less than or equal 7");
            }
            else
            {
                if(directory_name.Length <= 11)
                {
                    this.dir_name = directory_name;
                }
                else
                    Console.WriteLine("please enter name that has length less than or equal 11");
            }
            dir_empty = directory_empty;
            dir_firstCluster = directory_firstCluster;
            dir_filesize = directory_filesize;
        }
        public Directory_Entry(string directory_name, byte directory_attry, int directory_firstCluster)
        {
            dir_name = directory_name;
            dir_attr = directory_attry;
            dir_firstCluster = directory_firstCluster;


        }
        public Directory_Entry(string directory_name, byte directory_attry, int directory_firstCluster, int directory_filesize)
        {
            dir_name = directory_name;
            dir_attr = directory_attry;
            dir_firstCluster = directory_firstCluster;
            dir_filesize = directory_filesize;
        }
    }
}
