using MPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace summerWork
{
    internal static class Program
    {
        public static MPI.Environment mpiEnvironment;
        public static Intracommunicator comm;
        public static int size;
        public static int rank;

        [STAThread]
        static void Main()
        {
            string[] args = System.Environment.GetCommandLineArgs();
            mpiEnvironment = new MPI.Environment(ref args);
            comm = Communicator.world;
            size = comm.Size;
            rank = comm.Rank;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            mpiEnvironment.Dispose();
        }
    }
}
