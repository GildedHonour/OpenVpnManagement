using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace OpenVpnManagement {
    public class Manager {
        enum Signal {
            Hup,
            Term,
            Usr1,
            Usr2
        }
        
        private String ovpnFileName; //todo
        private String host;
        private int port;
        private int timeout;
        private Socket socket; 

        public void Manager(String host = "localhost", int port = 1194, int timeout = 10) {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try {
                socket.Connect(host, port);
            } catch (Exception ex) {
                //todo
            }
        }

        public String GetStatus() {
            var a = this.SendCommand("status");
        }

        public String GetStats() {
            var a = this.SendCommand("load-stats"); //todo? doesn't exist?
        }

        public String GetVersion() {
            return this.SendCommand("version");
        }

        public String GetGetPid() {
            return this.SendCommand("pid");
        }

        public String SendSignal(Signal sgn) {
            return this.SendCommand(string.Format("SIG{}", sgn.ToString().ToUpper());        
        }

        public String Mute() {
            return this.SendCommand("pid");
        }

        public String GetEcho() {
            return this.SendCommand("echo");
        }

        public String GetHelp() {
            return this.SendCommand("help");
        }

        public String Kill() {

        }

        public String Net() {

        }



        public String GetState() {
            return this.SendCommand("state");
        }

        public String GetState(int n) {
            return this.SendCommand("state " + n.ToString());
        }

        public String GetStateAll() {
            return this.SendCommand("state all");
        }

        public String SetStateOn() {
            return this.SendCommand("state on");
        }

        public String SetStateOnAll() {
            return this.SendCommand("state on all");
        }

        public String SetStateOff() {
            return this.SendCommand("state off");
        }

        public String GetLog() {
            return this.SendCommand("state off");
        }


        private String SendCommand(String cmd) {
            byte[] buffer = Encoding.Default.GetBytes(cmd);
            socket.Send(buffer, 0, buffer.Length, 0);
            buffer = new byte[255];
            int rec = socket.Receive(buffer, 0, buffer.Length, 0);

            return rec.ToString();
        }
    }
}
