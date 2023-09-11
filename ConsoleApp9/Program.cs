using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp9
{
    internal class Program
    {
        public static Directory CurrentDirectory = new Directory();
        public static string CurrentPath;
        public static void MD(string directory_name)
        {
            if (CurrentDirectory.searchDirectory(directory_name) == -1)
            {
                Directory_Entry d = new Directory_Entry(directory_name, 0x10, 0);
                Program.CurrentDirectory.Directoryfiles.Add(d);
                Program.CurrentDirectory.writeDirectory();
                if (Program.CurrentDirectory.parent != null)
                {
                    Program.CurrentDirectory.parent.update_content(d);
                    Program.CurrentDirectory.parent.writeDirectory();
                }
            }
            else
            {
                Console.WriteLine("A subdirectory or file test already exists.");
            }
        }
        public static void RD(string directory_name)
        {
            if (CurrentDirectory.searchDirectory(directory_name) != -1)
            {
                int index = Program.CurrentDirectory.searchDirectory(directory_name);
                if (Program.CurrentDirectory.Directoryfiles[index].dir_attr != 0x0)
                {
                    CurrentDirectory.deleteDirectory();
                }
                else
                {
                    Console.WriteLine("The directory name is invalid");
                }
            }
            else
            {
                Console.WriteLine("the system cannot find the file specified.");
            }
        }
        public static void CD(string directory_name)
        {
            if (CurrentDirectory.searchDirectory(directory_name) != -1)
            {
                //Directory.SetCurrentDirectory(directory_name);
                Console.WriteLine(Program.CurrentDirectory);
            }
            else
            {
                Console.WriteLine("The system cannot find the specified file.");
            }
        }
        public static void DIR(string file_name)
        {
            if (CurrentDirectory.searchDirectory(file_name) != -1)
            {
                int FileCounter = 0;
                int DirectoryCounter = 0;
                int SizeOfFilesCounter = 0;
                //size of dir =0 
                Console.WriteLine("Directory of : " + Program.CurrentPath);
                for (int i = 0; i < Program.CurrentDirectory.Directoryfiles.Count; i++)
                {

                    if (Program.CurrentDirectory.Directoryfiles[i].dir_attr == 0x0)
                    {
                        //file
                        FileCounter++;
                        SizeOfFilesCounter += Program.CurrentDirectory.Directoryfiles[i].dir_filesize;
                        if (Program.CurrentDirectory.Directoryfiles[i].dir_name == file_name)
                        {

                            Console.WriteLine("\t" + Program.CurrentDirectory.Directoryfiles[i].dir_filesize + Program.CurrentDirectory.Directoryfiles[i].dir_name);
                        }
                    }
                    else
                    {
                        //Directory
                        DirectoryCounter++;
                        if (Program.CurrentDirectory.Directoryfiles[i].dir_name == file_name)
                        {
                            Console.WriteLine("<Dir>" + Program.CurrentDirectory.Directoryfiles[i].dir_name);
                        }
                    }
                }
                Console.WriteLine(FileCounter + "File(s)" + SizeOfFilesCounter + "bytes");
                Console.WriteLine(DirectoryCounter + "Dir(s)" + Mini_FAT.GetFreeSpace());
            }
            else
            {
                Console.WriteLine("File not found");
            }
        }
        public static void DIR()
        {
            int FileCounter = 0;
            int DirectoryCounter = 0;
            int SizeOfFilesCounter = 0;
            //size of dir =0 
            Console.WriteLine("Directory of : " + Program.CurrentPath);
            for (int i = 0; i < Program.CurrentDirectory.Directoryfiles.Count; i++)
            {
                if (Program.CurrentDirectory.Directoryfiles[i].dir_attr == 0x0)
                {
                    //file
                    Console.WriteLine("\t" + Program.CurrentDirectory.Directoryfiles[i].dir_filesize + Program.CurrentDirectory.Directoryfiles[i].dir_name);
                    FileCounter++;
                    SizeOfFilesCounter += Program.CurrentDirectory.Directoryfiles[i].dir_filesize;
                }
                else
                {
                    //Directory
                    Console.WriteLine("<Dir>" + Program.CurrentDirectory.Directoryfiles[i].dir_name);
                    DirectoryCounter++;
                }
            }
            Console.WriteLine(FileCounter + "File(s)" + SizeOfFilesCounter + "bytes");
            Console.WriteLine(DirectoryCounter + "Dir(s)" + Mini_FAT.GetFreeSpace());
        }
        public static void IMPORT(string file_path)
        {
            //add file from computer to virtual disk
            //current directory=directory table
            if (File.Exists(file_path))
            {
                string content = File.ReadAllText(file_path);
                int size = content.Length;
                int name_start = file_path.LastIndexOf("\\");
                string name;
                name = file_path.Substring(name_start + 1);
                int index = Program.CurrentDirectory.searchDirectory(name);
                if (index == -1)
                {
                    //not exist
                    //file=>att=0x0
                    int f_c;
                    if (size > 0)
                    {
                        f_c = Mini_FAT.avaibleCluster();
                    }
                    else
                    {
                        f_c = 0;
                    }
                    File_Entry o1 = new File_Entry(name, 0x0, f_c, size, Program.CurrentDirectory, content);
                    o1.write();
                    Directory_Entry o2 = new Directory_Entry(name, 0x0, f_c, size);
                    Program.CurrentDirectory.Directoryfiles.Add(o2);
                    CurrentDirectory.writeDirectory();
                }
                else
                {
                    Console.WriteLine("the file aready exist");
                }
            }
            else
            {
                Console.WriteLine("this file not exist");
            }
        }
        public static void Export(string source, string distination)
        {
            //from virtual disk to computer
            int index = Program.CurrentDirectory.searchDirectory(source);
            if (index != -1)
            {
                //file exist
                if (System.IO.Directory.Exists(distination))
                {
                    //distination exist
                    int f_c = Program.CurrentDirectory.Directoryfiles[index].dir_firstCluster;
                    int size = Program.CurrentDirectory.Directoryfiles[index].dir_filesize;
                    string content = null;
                    File_Entry f = new File_Entry(source, 0x0, f_c, size, Program.CurrentDirectory, content);
                    f.read();
                    StreamWriter StreamWriter = new StreamWriter(distination + source);
                    StreamWriter.Write(f.content_file);
                    //f.Flush();
                    //f.Close();
                }
                else
                {
                    Console.WriteLine("the system can not find the path spicified in computer disk");
                }
            }
            else
            {
                Console.WriteLine("this file not exist in virtaul disk");
            }
        }
        public static void TYPE(string name)
        {
            int index = Program.CurrentDirectory.searchDirectory(name);
            if (index != -1)
            {
                //exist
                if (Program.CurrentDirectory.Directoryfiles[index].dir_attr == 0x0)
                {
                    //file
                    int f_c = Program.CurrentDirectory.Directoryfiles[index].dir_firstCluster;
                    int size = Program.CurrentDirectory.Directoryfiles[index].dir_filesize;
                    string content = null;
                    File_Entry o3 = new File_Entry(name, 0x0, f_c, size, Program.CurrentDirectory, content);
                    o3.read();
                    Console.WriteLine(o3.content_file);
                }
                else
                {
                    //directory
                    Console.WriteLine("Access is denied");
                }
            }
            else
            {
                Console.WriteLine("the system can not find the file specified");
            }
        }
        public static void RENAME(string oldname, string newname)
        {
            int index = Program.CurrentDirectory.searchDirectory(oldname);
            if (index != -1)
            {
                //exist
                int index1 = Program.CurrentDirectory.searchDirectory(newname);
                if (index1 == -1)
                {
                    //not exist
                    Directory_Entry f = new Directory_Entry();
                    //Program.CurrentDirectory.Directoryfiles[index];
                    f.dir_name = newname;
                    Program.CurrentDirectory.Directoryfiles.RemoveAt(index);
                    Program.CurrentDirectory.Directoryfiles.Add(f);
                    CurrentDirectory.writeDirectory();
                }
                else
                {
                    Console.WriteLine("a duplicate file name exist,or the file cannot be found.");
                }
            }
            else
            {
                Console.WriteLine("the system cannot find the file specified.");
            }
        }
        public static void COPY(string source, string distination)
        {
            if (CurrentPath != distination)
            {
                int index = Program.CurrentDirectory.searchDirectory(source);
                if (index != -1)
                {
                    int name_start = distination.LastIndexOf("\\");
                    string name;
                    name = distination.Substring(name_start + 1);
                    int index1 = Program.CurrentDirectory.searchDirectory(name);
                    if (index1 == -1)
                    {
                        //file not exist
                        int f_c = Program.CurrentDirectory.Directoryfiles[index].dir_firstCluster;
                        int size = Program.CurrentDirectory.Directoryfiles[index].dir_filesize;
                        Directory_Entry f = new Directory_Entry(name, 0x0, size);
                        Directory d = new Directory();
                        d.Directoryfiles.Add(f);
                    }
                    else
                    {
                        //file aready exist
                        //want to overwrite or no
                        Console.WriteLine("want to overwrite? Y/N");
                        string s = Console.ReadLine();
                        if (s == "y")
                        { }
                        else if (s == "n")
                        {
                            int f_c = Program.CurrentDirectory.Directoryfiles[index].dir_firstCluster;
                            int size = Program.CurrentDirectory.Directoryfiles[index].dir_filesize;
                            Directory_Entry f = new Directory_Entry(name, 0x0, size);
                            Directory d = new Directory();
                            d.Directoryfiles.Add(f);
                        }
                        else
                        {
                            Console.WriteLine("want to overwrite? Y/N");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("the system can not find the file spicified");
                }
            }
            else
            {
                Console.WriteLine("the system can not copy this file in the same current path");
            }

        }
        public static void DEL(string name)
        {
            int index = Program.CurrentDirectory.searchDirectory(name);
            if (index != -1)
            {
                //file exist
                int att = Program.CurrentDirectory.Directoryfiles[index].dir_attr;
                if (att == 0x0)
                {
                    //file
                    Console.WriteLine("Are you sure(Y/N)?");
                    string y = Console.ReadLine();
                    if (y == "Y")
                    {
                        int f_c = Program.CurrentDirectory.Directoryfiles[index].dir_firstCluster;
                        int size = Program.CurrentDirectory.Directoryfiles[index].dir_filesize;
                        string content = null;
                        File_Entry f = new File_Entry(name, 0x0, f_c, size, Program.CurrentDirectory, content);
                        f.delete();
                    }
                }
                else
                {
                    //not file
                    Console.WriteLine("the system cannot find the file specified");
                }
            }
            else
            {
                //ont exist
                Console.WriteLine("could not find");
            }
        }

        public static void Main(string[] args)
        {

            while (true)
            {


                Console.Write("c:/windows/system32>");
                string str = Console.ReadLine();
                char[] sperator = { ' ' };
                string[] strlist = str.Split(sperator);
                int length = strlist.Length;

                if (str == "help")
                {
                    Console.WriteLine("cd - Change the current default directory to.If the argument is not present, report the current directory.If the directory does not exist an appropriate error should be reported.");
                    Console.WriteLine("cls - Clear the screen.");
                    Console.WriteLine("dir - List the contents of directory .");
                    Console.WriteLine("quit - Quit the shell.");
                    Console.WriteLine("copy - Copies one or more files to another location");
                    Console.WriteLine("del - Deletes one or more files.");
                    Console.WriteLine("help - Provides Help information for commands.");
                    Console.WriteLine("md - Creates a directory.");
                    Console.WriteLine("rd - Removes a directory.");
                    Console.WriteLine("rd - Removes a directory.");
                    Console.WriteLine("rename - Renames a file.");
                    Console.WriteLine("type - Displays the contents of a text file.");
                    Console.WriteLine("import – import text file(s) from your computer");
                    Console.WriteLine("export – export text file(s) to your computer");

                }
                else if (str == "help cd")
                {
                    Console.WriteLine("Displays the name of or changes the current directory.\n CHDIR[/ D][drive:][path]\nCHDIR[..]\nCD[/ D][drive:][path]\nCD[..]\n..Specifies that you want to change to the parent directory.\nType CD drive : to display the current directory in the specified drive.\nType CD without parameters to display the current driveand directory.\nUse the / D switch to change current drive in addition to changing current\ndirectory for a drive.\nIf Command Extensions are enabled CHDIR changes as follows :\nThe current directory string is converted to use the same case as\nthe on disk names.So CD C : TEMP would actually set the current\ndirectory to C : \n Temp if that is the case on disk.\nCHDIR command does not treat spaces as delimiters, so it is possible to\nCD into a subdirectory name that contains a space without surrounding\nthe name with quotes.");
                }
                else if (str == "help cls")
                {
                    Console.WriteLine("Clear the screen.");
                }
                else if (str == "help dir")
                {
                    Console.WriteLine("Displays a list of files and subdirectories in a directory.\nDIR[drive:][path][filename][/ A [[:]attributes] ][/ B][/ C][/ D][/ L][/ N]\n[/ O [[:]sortorder] ][/ P][/ Q][/ R][/ S][/ T [[:]timefield] ][/ W][/ X][/ 4]\n[drive:][path][filename]\nSpecifies drive, directory, and /or files to list.");
                }
                else if (str == "help quit")
                {
                    Console.WriteLine("Quit the shell.");
                }
                else if (str == "help copy")
                {
                    Console.WriteLine("Copies one or more files to another location.\n COPY[/ D][/ V][/ N][/ Y | /-Y][/ Z][/ L][/ A | /B] source[/ A | /B]\n[+source[/ A | /B][+...]][destination[/ A | /B]]");
                }
                else if (str == "help md")
                {
                    Console.WriteLine("Creates a directory.\nMKDIR[drive:]path\nMD[drive:]path\nIf Command Extensions are enabled MKDIR changes as follows :\nMKDIR creates any intermediate directories in the path, if needed.");
                }
                else if (str == "help del")
                {
                    Console.WriteLine("Deletes one or more files.\nDEL[/ P][/ F][/ S][/ Q][/ A [[:]attributes] ] names\nERASE[/ P][/ F][/ S][/ Q][/ A [[:]attributes] ] names\nIf Command Extensions are enabled DEL and ERASE change as follows:\nThe display semantics of the / S switch are reversed in that it shows\nyou only the files that are deleted, not the ones it could not find.");
                }
                else if (str == "help rd")
                {
                    Console.WriteLine("Removes (deletes) a directory.\nRMDIR[/ S][/ Q][drive:]path\nRD[/ S][/ Q][drive:]path");
                }
                else if (str == "help rename")
                {
                    Console.WriteLine("Renames a file or files.\nRENAME[drive:][path]filename1 filename2.\nREN[drive:][path]filename1 filename2.\nNote that you cannot specify a new drive or path for your destination file.");
                }
                else if (str == "help import")
                {
                    Console.WriteLine(" import text file(s) from your computer");
                }
                else if (str == "help export")
                {
                    Console.WriteLine("export text file(s) to your computer");
                }
                else if (str == "cls")
                {
                    Console.Clear();
                }
                else if (str == "exit")
                {
                    Environment.Exit(0);
                }
                else if (strlist[0] == "MD" && length == 2)
                {
                    MD(strlist[1]);
                }
                else if (str == "MD")
                {
                    Console.WriteLine("the syntax of the command is incorrect.");
                }
                else if (strlist[0] == "RD" && length == 2)
                {
                    RD(strlist[1]);
                }
                else if (str == "RD")
                {
                    Console.WriteLine("the syntax of the command is incorrect.");
                }
                else if (strlist[0] == "CD" && length == 2)
                {
                    CD(strlist[1]);
                }
                else if (str == "CD")
                {
                    Console.WriteLine(Program.CurrentDirectory);
                }
                else if (str == "DIR")
                {
                    DIR();
                }
                else if (strlist[0] == "DIR" && length == 2)
                {
                    DIR(strlist[1]);
                }
                else if (strlist[0] == "IMPORT")
                {
                    IMPORT(strlist[1]);
                }
                else if (strlist[0] == "Export" && length == 3)
                {
                    Export(strlist[1], strlist[2]);
                }
                else if (strlist[0] == "TYPE" && length == 2)
                {
                    TYPE(strlist[1]);
                }
                else if (str == "TYPE")
                {
                    Console.WriteLine("The syntax of the command is incorrect");
                }
                else if (strlist[0] == "RENAME" && length == 3)
                {
                    RENAME(strlist[1], strlist[2]);
                }
                else if (str == "RENAME")
                {
                    Console.WriteLine("The syntax of the command is incorrect");
                }
                else if (strlist[0] == "COPY" && length == 3)
                {
                    COPY(strlist[1], strlist[2]);
                }
                else if (str == "COPY")
                {
                    Console.WriteLine("The syntax of the command is incorrect");
                }
                else if (strlist[0] == "DEL" && length == 2)
                {
                    DEL(strlist[1]);
                }
                else if (str == "DEL")
                {
                    Console.WriteLine("The syntax of the command is incorrect");
                }
                else
                {
                    Console.WriteLine(str + "is not recognized as an internal or external command,operable program or batch file.");
                }
            }
        }

    }
}
